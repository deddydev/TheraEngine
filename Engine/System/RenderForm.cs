﻿using TheraEngine;
using System.Threading;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.IO;
using TheraEngine.Worlds;
using TheraEngine.Input;

namespace TheraEngine
{
    public partial class RenderForm : Form
    {
        public RenderForm(Game game)
        {
            Engine.SetGame(game);
            InitializeComponent();
            Engine.Initialize(renderPanel1);

            Text = game.Name;
            if (!string.IsNullOrEmpty(game.IconPath) && File.Exists(game.IconPath))
                Icon = new Icon(game.IconPath);
            
            switch (game.UserSettings.WindowBorderStyle)
            {
                case WindowBorderStyle.None:
                    FormBorderStyle = FormBorderStyle.None;
                    break;
                case WindowBorderStyle.Fixed:
                    FormBorderStyle = FormBorderStyle.FixedSingle;
                    break;
                case WindowBorderStyle.Sizable:
                    FormBorderStyle = FormBorderStyle.Sizable;
                    break;
            }

            if (game.UserSettings.FullScreen)
                WindowState = FormWindowState.Maximized;
            
            Application.ApplicationExit += Application_ApplicationExit;
        }
        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            Engine.ShutDown();
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Engine.Run();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            renderPanel1.UnregisterTick();
        }
    }
}
