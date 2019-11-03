﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core;
using TheraEngine.Core.Files;
using TheraEngine.Worlds;

namespace TheraEditor.Windows.Forms
{
    public class UIWorldManager : WorldManager
    {
        public UIWorldManager()
        {
            World = new World(new WorldSettings() { TwoDimensional = true });
        }
    }
    public abstract class DockableRenderableFileEditor<TFile, TRenderHandler> : DockableFileEditor<TFile>
        where TFile : class, IFileObject
        where TRenderHandler : class, IUIRenderHandler
    {
        public RenderPanel<TRenderHandler> RenderPanel { get; private set; }
        public abstract bool ShouldHideCursor { get; }
        public int WorldManagerId { get; private set; }
        
        public DockableRenderableFileEditor()
        {
            InitializeComponent();
        }

        protected override void DestroyHandle()
        {
            base.DestroyHandle();
        }

        protected void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            if (File != null)
                Editor.Instance.PropertyGridForm.PropertyGrid.TargetObject = File;
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            Engine.Instance.DomainProxySet += Instance_ProxySet;
            Engine.Instance.DomainProxyUnset += Instance_ProxyUnset;
            Instance_ProxySet(Engine.DomainProxy);

            RenderPanel.LinkToWorldManager(WorldManagerId);
            RenderPanel.RenderHandler.FormShown();
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            RenderPanel.RenderHandler.FormClosed();
            RenderPanel.UnlinkFromWorldManager();

            Engine.Instance.DomainProxySet -= Instance_ProxySet;
            Engine.Instance.DomainProxyUnset -= Instance_ProxyUnset;
            Instance_ProxyUnset(null);

            base.OnFormClosed(e);
        }
        private void Instance_ProxyUnset(EngineDomainProxy proxy)
        {
            proxy.UnregisterWorldManager(WorldManagerId);
            proxy.ReleaseSponsor(this);
        }
        private void Instance_ProxySet(EngineDomainProxy proxy)
        {
            WorldManagerId = proxy.RegisterWorldManager<UIWorldManager>();
            proxy.SponsorObject(this);
        }
        private void RenderPanel_MouseEnter(object sender, EventArgs e)
        {
            if (ShouldHideCursor)
                Cursor.Hide();
        }
        private void RenderPanel_MouseLeave(object sender, EventArgs e)
        {
            if (ShouldHideCursor)
                Cursor.Show();
        }
        private void InitializeComponent()
        {
            RenderPanel = new RenderPanel<TRenderHandler>();

            SuspendLayout();

            RenderPanel.AllowDrop = false;
            RenderPanel.Dock = DockStyle.Fill;
            RenderPanel.Location = new System.Drawing.Point(0, 0);
            RenderPanel.Margin = new Padding(0);
            RenderPanel.Name = "RenderPanel";
            RenderPanel.MouseEnter += RenderPanel_MouseEnter;
            RenderPanel.MouseLeave += RenderPanel_MouseLeave;
            Controls.Add(RenderPanel);

            ResumeLayout(false);
        }
    }
}
