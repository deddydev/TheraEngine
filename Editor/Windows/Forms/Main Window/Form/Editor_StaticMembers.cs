using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Input;

namespace TheraEditor.Windows.Forms
{
    public partial class Editor : TheraForm, IMappableShortcutControl
    {
        public const string ContactURL = "https://www.theradevgames.com/contact/";
        public const string DocumentationURL = "https://github.com/TheraEngine/Documentation/wiki";
        
        /// <summary>
        /// Returns the one and only instance of the editor for this session.
        /// </summary>
        public static Editor Instance { get; private set; }

        public static IEditorControl ActiveRenderForm { get; private set; } = null;

        public static GlobalFileRef<EditorSettings> DefaultSettingsRef { get; }
            = new GlobalFileRef<EditorSettings>(Path.Combine(Application.StartupPath, "..", "..", "..", "Editor", "Config.xset"), () => new EditorSettings());
        
        public static Color BackgroundColor => Color.FromArgb(92, 93, 100);
        public static Color TitleBarColor => Color.FromArgb(92, 93, 100);
        public static Color TurquoiseColor => Color.FromArgb(60, 102, 100);
        public static Color TurquoiseColorLight => Color.FromArgb(150, 192, 192);
        public static Color TextColor => Color.FromArgb(224, 224, 224);
    }
}
