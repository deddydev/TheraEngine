using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing;
using TheraEngine;
using Core.Win32.Native;

namespace TheraEditor.Windows.Forms
{
    struct ItemType
    {
        public object ItemInfo;
        public bool IsFile;
    }

    public partial class FileFolderList : ListView
    {
        Win32 _win32 = new Win32();
        ImageList _il16 = new ImageList();
        ImageList _il32 = new ImageList();
        BackgroundWorker _bgIconLoader = new BackgroundWorker();
        List<ItemType> _paths = new List<ItemType>();
        bool _use16 = true;

        #region Properties
        string _defaultPath = "C:\\";
        public string DefaultPath
        {
            get => _defaultPath;
            set => _defaultPath = value;
        }

        string _selectedPath = string.Empty;
        public string SelectedPath => _selectedPath;
        
        bool _isSoloBrowser = true;
        public bool IsSoloBrowser
        {
            get => _isSoloBrowser;
            set => _isSoloBrowser = value;
        }
        #endregion

        public FileFolderList()
        {
            
        }

        public void Load()
        {
            _il16.ColorDepth = ColorDepth.Depth32Bit;
            _il32.ColorDepth = ColorDepth.Depth32Bit;
            _il32.ImageSize = new Size(32, 32);

            SmallImageList = _il16;
            LargeImageList = _il32;
            //View = System.Windows.Forms.View.Details;
            Activation = ItemActivation.TwoClick;
            MultiSelect = false;

            //if (View == System.Windows.Forms.View.Details)
            {
                Columns.Add("colName", "Name");
                Columns.Add("colType", "Type");
                Columns.Add("Size", 1, HorizontalAlignment.Right);
                Columns.Add("colDate", "Date");
            }

            _bgIconLoader.WorkerReportsProgress = true;
            _bgIconLoader.WorkerSupportsCancellation = true;
            _bgIconLoader.DoWork += new DoWorkEventHandler(bgIconLoader_DoWork);
            _bgIconLoader.ProgressChanged += new ProgressChangedEventHandler(bgIconLoader_ProgressChanged);
            _bgIconLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgIconLoader_RunWorkerCompleted);

            ItemActivate += new EventHandler(FileFolderList_ItemActivate);
            ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(FileFolderList_ItemSelectionChanged);

            Browse(_defaultPath);
        }

        void bgIconLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Invoke(new MethodInvoker(delegate
            {
                AutoColResize();
            }));
        }

        void bgIconLoader_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 1)
            {
                string fullname = (string)e.UserState;
                string name = Path.GetFileName((string)e.UserState);
                ListViewItem item = null;

                Invoke(new MethodInvoker(delegate
                {
                    item = FindItemWithText(name, false, 0, true);
                }));

                if (item != null)
                {
                    try
                    {
                        //if (use16)
                        //{
                        //    if (!il16.Images.ContainsKey(fullname))
                        //    {
                        //        il16.Images.Add(fullname, win32.GetIcon(fullname, true));
                        //    }
                        //}
                        //else
                        //{
                        //    if (!il32.Images.ContainsKey(fullname))
                        //    {
                        //        il32.Images.Add(fullname, win32.GetIcon(fullname, false));
                        //    }
                        //}

                        Engine.PrintLine(fullname);

                        item.ImageKey = fullname;
                    }
                    catch(Exception ex)
                    {
                        Engine.PrintLine(ex.Message);
                    }
                    finally
                    {
                        Application.DoEvents();
                    }
                }
            }
        }
        
        void bgIconLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            string fullname = string.Empty;

            foreach (ItemType item in _paths)
            {
                if (_bgIconLoader.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                #region // get associated icon
                if (!item.IsFile)
                {
                    DirectoryInfo di = (DirectoryInfo)item.ItemInfo;
                    if (_use16)
                    {
                        if (!_il16.Images.ContainsKey(di.FullName))
                        {
                            _il16.Images.Add(di.FullName, _win32.GetIcon(di.FullName, true));
                        }
                    }
                    else
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            if (!_il32.Images.ContainsKey(di.FullName))
                            {
                                _il32.Images.Add(di.FullName, _win32.GetIcon(di.FullName, false));
                            }
                        }));
                    }

                    //bgIconLoader.ReportProgress(1, di.FullName);
                    fullname = di.FullName;
                }
                else
                {
                    FileInfo fi = (FileInfo)item.ItemInfo;
                    if (_use16)
                    {
                        if (!_il16.Images.ContainsKey(fi.FullName))
                        {
                            _il16.Images.Add(fi.FullName, _win32.GetIcon(fi.FullName, true));
                        }
                    }
                    else
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            if (!_il32.Images.ContainsKey(fi.FullName))
                            {
                                _il32.Images.Add(fi.FullName, _win32.GetIcon(fi.FullName, false));
                            }
                        }));
                    }

                    //bgIconLoader.ReportProgress(1, fi.FullName);
                    fullname = fi.FullName;
                }
                #endregion

                #region // add to listview
                string name = Path.GetFileName(fullname);
                ListViewItem lvItem = null;

                Invoke(new MethodInvoker(delegate
                {
                    lvItem = FindItemWithText(name, false, 0, true);
                }));

                if (lvItem != null)
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        lvItem.ImageKey = fullname;

                        //if (View == System.Windows.Forms.View.Details)
                        {
                            if (item.IsFile)
                            {
                                lvItem.SubItems[1].Text = _win32.GetFileType(fullname);
                                lvItem.SubItems[2].Text = _win32.GetFileSize(fullname);
                            }
                        }
                    }));
                }
                #endregion
            }
        }

        void FileFolderList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (SelectedItems.Count <= 0)
                return;

            //try
            {
                ListViewItem item = SelectedItems[0];
                ItemType type = (ItemType)item.Tag;

                if (!type.IsFile)
                {
                    DirectoryInfo di = (DirectoryInfo)type.ItemInfo;
                    _selectedPath = di.FullName;
                }
                else
                {
                    FileInfo fi = (FileInfo)type.ItemInfo;
                    _selectedPath = fi.FullName;
                }
            }
            //catch { }
        }

        void FileFolderList_ItemActivate(object sender, EventArgs e)
        {
            if (SelectedItems.Count <= 0)
                return;

            ListViewItem thisItem = SelectedItems[0];
            ItemType type = (ItemType)thisItem.Tag;

            if (!type.IsFile)
            {
                DirectoryInfo di = (DirectoryInfo)type.ItemInfo;
                Browse(di.FullName);
            }
        }

        public void Browse(string path)
        {
            _bgIconLoader.CancelAsync();

            if (View == View.LargeIcon || View == View.Tile)
            {
                _use16 = false;
            }

            while (_bgIconLoader.IsBusy)
                Application.DoEvents();

            _paths.Clear();
            Items.Clear();

            BeginUpdate();

            #region // add "back" item if necessary
            if (_isSoloBrowser)
            {
                DirectoryInfo currentPath = new DirectoryInfo(path);
                if (currentPath.FullName.Length > 3)
                {
                    ListViewItem item = new ListViewItem("...")
                    {
                        Tag = new ItemType()
                        {
                            ItemInfo = currentPath.Parent,
                            IsFile = false,
                        }
                    };
                    Items.Add(item);
                }
            }
            #endregion

            #region // get folders
            foreach (string folder in Directory.GetDirectories(path))
            {
                DirectoryInfo di = new DirectoryInfo(folder);
                    
                if (di.Attributes.ToString().Contains("System"))
                    continue;

                //if (!il16.Images.ContainsKey(di.FullName))
                //{
                //    Icon ico32;
                //    il16.Images.Add(di.FullName, win32.GetIcon(di.FullName, out ico32));
                //    il32.Images.Add(di.FullName, ico32);
                //}

                ListViewItem item = new ListViewItem(di.Name)
                {
                    ImageKey = di.FullName,
                    Tag = new ItemType()
                    {
                        ItemInfo = di,
                        IsFile = false,
                    }
                };

                // add temp subitems if View was set to Details
                //if (View == System.Windows.Forms.View.Details)
                {
                    for (int i = 0; i < Columns.Count; i++)
                        item.SubItems.Add(string.Empty);

                    item.SubItems[3].Text = di.CreationTime.ToString();
                    // key should be "colType" but am not sure
                    // why it does not work
                    item.SubItems[1].Text = "File folder";
                }

                Items.Add(item);

                _paths.Add((ItemType)item.Tag);
            }
            #endregion

            #region // get files
            foreach (string file in Directory.GetFiles(path))
            {
                FileInfo fi = new FileInfo(file);

                if (fi.Attributes.ToString().Contains("System"))
                {
                    continue;
                }

                //if (!il16.Images.ContainsKey(fi.FullName))
                //{
                //    Icon ico32;
                //    il16.Images.Add(fi.FullName, win32.GetIcon(fi.FullName, out ico32));
                //    il32.Images.Add(fi.FullName, ico32);
                //}

                ListViewItem item = new ListViewItem(fi.Name)
                {
                    ImageKey = fi.FullName,
                    Tag = new ItemType()
                    {
                        ItemInfo = fi,
                        IsFile = true,
                    }
                };

                // add temp subitems if View was set to Details
                //if (View == System.Windows.Forms.View.Details)
                {
                    for (int i = 0; i < Columns.Count; i++)
                        item.SubItems.Add(string.Empty);

                    item.SubItems[3].Text = fi.CreationTime.ToString();
                }

                Items.Add(item);

                _paths.Add((ItemType)item.Tag);
            }
            #endregion

            EndUpdate();
            Refresh();

            Application.DoEvents();

            _bgIconLoader.RunWorkerAsync();
        }

        void AutoColResize()
        {
            foreach (ColumnHeader col in Columns)
            {
                //Invoke(new MethodInvoker(delegate
                //{
                col.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                //col.Width = -2;
                //}));
            }
        }
    }

    internal class Win32
    {
        static class FILE_ATTRIBUTE
        {
            public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        }

        static class SHGFI
        {
            public const uint SHGFI_TYPENAME = 0x000000400;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        }

        public string GetFileSize(string fullpath)
            => String.Format(new FileSizeFormatProvider(), "{0:fs}", new FileInfo(fullpath).Length);

        public string GetFileType(string fullpath)
        {
            uint dwFileAttributes = FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL;
            uint uFlags = SHGFI.SHGFI_TYPENAME | SHGFI.SHGFI_USEFILEATTRIBUTES;
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr n = NativeMethods.SHGetFileInfo(fullpath, dwFileAttributes, ref shinfo, (uint)Marshal.SizeOf(shinfo), uFlags);

            return shinfo.szTypeName;
        }

        public Icon GetIcon(string fullpath, bool use16)
        {
            Icon ico = null;
            IntPtr hImgSmall;
            IntPtr hImgLarge;
            SHFILEINFO shinfo = new SHFILEINFO();

            if (use16)
            {
                //Use this to get the small Icon
                hImgSmall = NativeMethods.SHGetFileInfo(fullpath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), NativeConstants.SHGFI_ICON | NativeConstants.SHGFI_SMALLICON);
                ico = Icon.FromHandle(shinfo.hIcon);
            }
            else
            {
                //Use this to get the large Icon
                hImgLarge = NativeMethods.SHGetFileInfo(fullpath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), NativeConstants.SHGFI_ICON | NativeConstants.SHGFI_LARGEICON);
                ico = Icon.FromHandle(shinfo.hIcon);
            }            

            return ico;
        }
    }

    internal class FileSizeFormatProvider : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter)) return this;
            return null;
        }

        private const string FileSizeFormat = "fs";
        private const Decimal OneKiloByte = 1024M;
        private const Decimal OneMegaByte = OneKiloByte * 1024M;
        private const Decimal OneGigaByte = OneMegaByte * 1024M;

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (format == null || !format.StartsWith(FileSizeFormat))
                return DefaultFormat(format, arg, formatProvider);

            if (arg is string)
                return DefaultFormat(format, arg, formatProvider);

            Decimal size;

            try
            {
                size = Convert.ToDecimal(arg);
            }
            catch (InvalidCastException)
            {
                return DefaultFormat(format, arg, formatProvider);
            }

            string suffix;
            if (size > OneGigaByte)
            {
                size /= OneGigaByte;
                suffix = " GB";
            }
            else if (size > OneMegaByte)
            {
                size /= OneMegaByte;
                suffix = " MB";
            }
            else if (size > OneKiloByte)
            {
                size /= OneKiloByte;
                suffix = " KB";
            }
            else
            {
                suffix = " Byte(s)";
            }

            string precision = format.Substring(2);
            if (String.IsNullOrEmpty(precision))
                precision = "0";
            return String.Format("{0:N" + precision + "}{1}", size, suffix);
        }

        private static string DefaultFormat(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg is IFormattable formattableArg)
                return formattableArg.ToString(format, formatProvider);
            
            return arg.ToString();
        }

    }
}
