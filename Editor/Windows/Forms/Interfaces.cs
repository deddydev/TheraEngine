using System;
using System.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Core.Files;
using TheraEngine.GameModes;
using TheraEngine.Rendering;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public interface IDockableFileEditorControl<T> : IFileEditorControl<T> where T : class, IFileObject
    {
        event FormClosedEventHandler FormClosed;

        void Show(DockPanel dockPanel, DockState document);
        bool Focus();
    }
    public interface IFileEditorControl<T> where T : class, IFileObject
    {
        T File { get; set; }

        void Save();
        void SaveAs();
        bool AllowFileClose();
    }
    public interface IFileEditorControl
    {
        IFileObject File { get; }

        void Save();
        void SaveAs();
        bool AllowFileClose();
    }
    public interface IEditorRenderHandler
    {
        /// <summary>
        /// The player index this control allows input from.
        /// </summary>
        ELocalPlayerIndex PlayerIndex { get; }
        /// <summary>
        /// The render panel with the viewport that will be possessed by the desired player.
        /// </summary>
        RenderContext RenderPanel { get; }
        /// <summary>
        /// The pawn the player will possess for editing purposes.
        /// </summary>
        IPawn EditorPawn { get; }
        /// <summary>
        /// The game mode used for this render form.
        /// </summary>
        IGameMode GameMode { get; }
        /// <summary>
        /// The world this render form is rendering.
        /// </summary>
        IWorld World { get; }
    }
}
