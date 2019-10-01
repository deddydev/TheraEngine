using Extensions;
using System;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Maths;
using TheraEngine.Timers;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableWelcomeWindow : DockContent
    {
        public DockableWelcomeWindow()
        {
            InitializeComponent();
            label2.Font = Engine.MakeFont("origicide", 24.0f, FontStyle.Regular);
            TitleStartLoc = label2.Location;
            TitleHoverLoc = label2.Location;
            TitleHoverLoc.Y += 20;
            //splitContainer1.Panel2Collapsed = true;

            //            const string page = @"
            //<html>
            //<head>
            //    <title></title>
            //    <meta http-equiv='X-UA-Compatible' content='IE=11'/>
            //    <script src=""https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js""></script>
            //    <script>
            //        var channelID = ""UCekkiovQ0NxqzraXseicDPQ"";
            //        $.getJSON('https://api.rss2json.com/v1/api.json?rss_url=https%3A%2F%2Fwww.youtube.com%2Ffeeds%2Fvideos.xml%3Fchannel_id%3D' + channelID, function(data) 
            //        {
            //            var link = data.items[0].link;
            //            var id = link.substr(link.indexOf(""="") + 1);
            //            $(""#youtube_video"").attr(""src"",""https://youtube.com/embed/"" + id + ""?controls=0&showinfo=0&rel=0&autoplay=1"");
            //        });
            //    </script>
            //    <style>
            //        body
            //        {
            //            margin: 0;
            //            padding: 0;
            //            background-color: #555555;
            //        }
            //        .video_container
            //        {
            //	        position:relative;
            //	        padding-bottom:56.25%;
            //	        padding-top:30px;
            //	        height:0;
            //	        overflow:hidden;
            //        }
            //        .video_container iframe, .video_container object, .video_container embed
            //        {
            //	        position:absolute;
            //	        top:0;
            //	        left:0;
            //	        width:100%;
            //	        height:100%;
            //        }
            //        #youtube_video
            //        {
            //            width: 100%;
            //        }
            //    </style>
            //    </head>
            //    <body>
            //        <div class=""video_container"">
            //            <iframe id=""youtube_video"" frameborder=""0"" allow=""autoplay; encrypted - media"" allowfullscreen></iframe>
            //        </div>
            //    </body>
            //</html>";
            //            webBrowser1.DocumentText = page;
            //webBrowser1.ScriptErrorsSuppressed = true;
            //webBrowser1.AllowNavigation = true;
            //webBrowser1.Navigate("https://theradevgames.com");
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            label2.Location = new Point((int)(label2.Parent.Size.Width / 2.0f - label2.Size.Width / 2.0f) + 10, label2.Location.Y);
            TitleStartLoc = label2.Location;
            TitleHoverLoc = label2.Location;
            TitleHoverLoc.Y += 20;
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            EditorSettings settings = await Editor.DefaultSettingsRef.GetInstanceAsync();
            var recentFiles = settings?.RecentlyOpenedProjectPaths;
            //var recentFiles = Properties.Settings.Default.RecentFiles;
            if (recentFiles != null)
            {
                bool hasRecentFiles = recentFiles.Count > 0;
                splitContainer1.Panel2Collapsed = !hasRecentFiles;
                foreach (string path in recentFiles)
                    AddRecentFilePath(path);
            }
            else
                splitContainer1.Panel2Collapsed = true;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            btnOpen.Visible = btnNew.Visible = false;
            pnlCreateProj.Visible = true;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            pnlCreateProj.Visible = false;
            btnOpen.Visible = btnNew.Visible = true;
        }

        private void btnOpen_Click(object sender, EventArgs e)
            => Editor.Instance.OpenProject();
        
        private async void btnCreate_Click(object sender, EventArgs e)
        {
            TProject project = await TProject.CreateAsync(txtProjectDir.Text, txtName.Text);
            Editor.Instance.SetProject(project);
            Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = project;
        }
        private void btnBrowseProjectDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            var result = fbd.ShowDialog();
            if (result == DialogResult.OK)
                txtProjectDir.Text = fbd.SelectedPath;
        }
        
        public void AddRecentFilePath(string path)
        {
            RecentProjectInfoControl info = RecentProjectInfoControl.FromFile(path);
            recentProjects.Controls.Add(info);
        }
        private Color TitleStartColor = Color.FromArgb(224, 224, 224);
        private Color TitleHoverColor = Color.FromArgb(164, 218, 208);
        private Point TitleStartLoc, TitleHoverLoc;
        private EventHandler<FrameEventArgs> FadeEvent;
        private EventHandler<FrameEventArgs> LocEvent;
        private EventHandler<FrameEventArgs> LocEvent2;
        private bool PanelHovered = false;
        private bool TextHovered = false;
        private bool Dragging = false;
        private bool Resetting = false;
        private Point LastMousePosition;
        private bool IsTitleHovered => PanelHovered || TextHovered;
        private float CustomBounceTimeModifier(float time)
            => Interp.BounceTimeModifier(time, 7, 3.5);
        private void BeginHover()
        {
            label2.FadeForeColor(TitleHoverColor, 1.0f, ref FadeEvent, CustomBounceTimeModifier);
            label2.LerpLocation(TitleHoverLoc, 1.0f, ref LocEvent, CustomBounceTimeModifier);
        }
        private void EndHover()
        {
            label2.FadeForeColor(TitleStartColor, 0.7f, ref FadeEvent, Interp.CosineTimeModifier);
            label2.LerpLocation(TitleStartLoc, 0.7f, ref LocEvent, Interp.CosineTimeModifier);
        }
        private void label2_MouseEnter(object sender, EventArgs e)
        {
            bool prev = IsTitleHovered;
            TextHovered = true;
            if (IsTitleHovered != prev)
                BeginHover();
        }
        private void label2_MouseLeave(object sender, EventArgs e)
        {
            bool prev = IsTitleHovered;
            TextHovered = false;
            if (IsTitleHovered != prev)
                EndHover();
        }
        private void transparentPanel1_MouseEnter(object sender, EventArgs e)
        {
            bool prev = IsTitleHovered;
            PanelHovered = true;
            if (IsTitleHovered != prev)
                BeginHover();
        }
        private void label2_MouseDown(object sender, MouseEventArgs e)
        {
            LastMousePosition = Cursor.Position;
            Dragging = true;

            Engine.UnregisterTick(null, LocEvent, null);
            LocEvent = null;
        }
        private void label2_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
            Resetting = true;
            label2.LerpLocation(TitleStartLoc, 0.7f, ref LocEvent, Interp.CosineTimeModifier, OnResetComplete);
        }
        private void OnResetComplete()
        {
            Resetting = false;
        }
        private void label2_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging)
            {
                int xDiff = Cursor.Position.X - LastMousePosition.X;
                int yDiff = Cursor.Position.Y - LastMousePosition.Y;

                label2.Location = new Point(
                    label2.Location.X + xDiff,
                    label2.Location.Y + yDiff);

                LastMousePosition = Cursor.Position;
            }
        }
        private void transparentPanel1_MouseLeave(object sender, EventArgs e)
        {
            bool prev = IsTitleHovered;
            PanelHovered = false;
            if (IsTitleHovered != prev)
                EndHover();
        }
    }
}
