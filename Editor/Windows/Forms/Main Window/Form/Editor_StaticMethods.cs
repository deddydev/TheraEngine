using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Windows.Forms
{
    public partial class Editor : TheraForm, IMappableShortcutControl
    {
        /// <summary>
        /// This will possess and unpossess the necessary viewports and pawns corresponding to the given editor control.
        /// </summary>
        /// <param name="control">The editor control that the user is focused on.</param>
        public static void SetActiveEditorControl(IEditorRenderHandler control)
        {
            if (ActiveRenderForm == control)
                return;

            var world = ActiveRenderForm?.World;
            if (world != null)
                world.CurrentGameMode = null;
            
            ActiveRenderForm = control;

            world = ActiveRenderForm?.World;
            if (world != null)
                world.CurrentGameMode = ActiveRenderForm.GameMode;
            
            Engine.PrintLine("Set active render form: " + ActiveRenderForm?.ToString() ?? "null");
        }
        public static LocalFileRef<EditorSettings> GetSettingsRef() 
            => Instance.Project?.EditorSettingsOverrideRef ?? Instance.DefaultSettingsRef;
        public static EditorSettings GetSettings()
        {
            var fref = GetSettingsRef();
            var settings = fref?.GetInstance();
            AppDomainHelper.Sponsor(settings);
            return settings;
        }
        
        /// <summary>
        /// Promps the user to create an instance of T using user-chosen derived type, constructor and parameters.
        /// </summary>
        /// <typeparam name="T">The object type to create.</typeparam>
        /// <returns>A newly created instance of T.</returns>
        public static T UserCreateInstanceOf<T>(bool allowDerivedTypes = true, IWin32Window window = null)
            => (T)UserCreateInstanceOf(TypeProxy.TypeOf<T>(), allowDerivedTypes, window);
        /// <summary>
        /// Creates an instance of elementType using user-chosen derived type, constructor and parameters.
        /// </summary>
        /// <param name="type">The object type to create.</param>
        /// <returns>A newly created instance of elementType.</returns>
        public static object UserCreateInstanceOf(TypeProxy type, bool allowDerivedTypes = true, IWin32Window window = null)
        {
            //if (type.IsPrimitive)
            //    return type.GetDefaultValue();

            //if (type == typeof(string))
            //    return string.Empty;

            if (type.IsGenericTypeDefinition)
            {
                using (GenericsSelector gs = new GenericsSelector(type))
                {
                    if (gs.ShowDialog(window ?? Instance) == DialogResult.OK)
                        type = gs.FinalClassType;
                    else
                        return null;
                }
            }

            using (ObjectCreator creator = new ObjectCreator())
            {
                if (creator.Initialize(type, allowDerivedTypes))
                    creator.ShowDialog(window ?? Instance);

                object o = creator.ConstructedObject;
                if (o is IObject iobj)
                    iobj.ConstructedProgrammatically = false;
                return o;
            }
        }
        public static void SetPropertyGridObject(IFileObject obj)
        {
            Instance.PropertyGridForm.PropertyGrid.TargetObject = obj;
        }

        [DllImport("user32.dll")]
        private static extern int ShowCursor(bool bShow);

        internal static void ShowCursor()
        {
            if (Instance.InvokeRequired)
            {
                Instance.Invoke((Action)ShowCursor);
                return;
            }
            Cursor.Show();
            //while (ShowCursor(true) < 0) ;
        }
        internal static void HideCursor()
        {
            if (Instance.InvokeRequired)
            {
                Instance.Invoke((Action)HideCursor);
                return;
            }
            Cursor.Hide();
            //while (ShowCursor(false) >= 0) ;
        }
        
        private static async void CheckUpdates()
        {
            try
            {
                AssemblyName editorVer = Assembly.GetExecutingAssembly().GetName();
                //AssemblyName engineVer = typeof(Engine).Assembly.GetName();
                await Github.Updater.TryInstallUpdate(editorVer);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
        }
    }
}
