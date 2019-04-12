using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheraEditor.Windows.Forms.PropertyGrid;
using TheraEditor.Wrappers;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection;
using TheraEngine.Input;

namespace TheraEditor.Windows.Forms
{
    public partial class Editor : TheraForm, IMappableShortcutControl
    {
        public static void ResetTypeCaches()
        {
            TheraPropertyGrid.ReloadEditorTypes();
            FolderWrapper.LoadFileTypes();
            BaseObjectSerializer.ResetObjectSerializerCache(true);
            TFileObject.ClearThirdPartyTypeCache(true);
        }
        /// <summary>
        /// This will possess and unpossess the necessary viewports and pawns corresponding to the given editor control.
        /// </summary>
        /// <param name="control">The editor control that the user is focused on.</param>
        public static void SetActiveEditorControl(IEditorRenderableControl control)
        {
            if (ActiveRenderForm == control)
                return;

            //bool sameGameMode = ReferenceEquals(ActiveRenderForm?.GameMode, control?.GameMode);

            if (ActiveRenderForm?.GameMode != null)
            {
                //int index = (int)ActiveRenderForm.PlayerIndex;
                //if (index < ActiveRenderForm.GameMode.LocalPlayers.Count)
                //{
                //    LocalPlayerController c = ActiveRenderForm.GameMode.LocalPlayers[index];
                //    ActiveRenderForm.RenderPanel.UnregisterController(c);
                //    c.ControlledPawn = null;
                //}

                //if (!sameGameMode)
                //ActiveRenderForm.GameMode.EndGameplay();
                ActiveRenderForm.World.CurrentGameMode = null;
            }
            ActiveRenderForm = control;
            if (ActiveRenderForm?.GameMode != null)
            {
                //if (!sameGameMode)
                ActiveRenderForm.World.CurrentGameMode = ActiveRenderForm.GameMode;
                //ActiveRenderForm.GameMode.BeginGameplay();

                //int index = (int)control.PlayerIndex;W
                //if (index < ActiveRenderForm.GameMode.LocalPlayers.Count)
                //{
                //    LocalPlayerController c = ActiveRenderForm.GameMode.LocalPlayers[index];
                //    ActiveRenderForm.RenderPanel.GetOrAddViewport(control.PlayerIndex).RegisterController(c);
                //    c.ControlledPawn = ActiveRenderForm.EditorPawn;
                //}

                Engine.PrintLine("Set active render form: " + ActiveRenderForm.ToString());
            }
        }
        public static GlobalFileRef<EditorSettings> GetSettingsRef() => Instance.Project?.EditorSettingsOverrideRef ?? DefaultSettingsRef;
        public static EditorSettings GetSettings() => Instance.Project?.EditorSettingsOverrideRef?.File ?? DefaultSettingsRef?.File;
        
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

                return creator.ConstructedObject;
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
                await Github.Updater.CheckUpdates(editorVer);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
        }
    }
}
