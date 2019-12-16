﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Components;

namespace TheraEngine.Rendering.UI
{
    public class UIGridComponent : UIDockableComponent
    {
        private EventList<RowDefinition> _rows = new EventList<RowDefinition>();
        private EventList<ColumnDefinition> _columns = new EventList<ColumnDefinition>();

        public UIGridComponent()
        {

        }

        public int[,] Indices { get; set; }
        public bool InvertY { get; set; }

        public EventList<RowDefinition> Rows
        {
            get => _rows;
            set
            {
                if (SetBackingField(ref _rows, value,
                    () => _rows.CollectionChanged -= CollectionChanged,
                    () => _rows.CollectionChanged += CollectionChanged, false))
                    RegenerateIndices();
            }
        }
        public EventList<ColumnDefinition> Columns
        {
            get => _columns;
            set
            {
                if (SetBackingField(ref _columns, value,
                    () => _columns.CollectionChanged -= CollectionChanged,
                    () => _columns.CollectionChanged += CollectionChanged, false))
                    RegenerateIndices();
            }
        }
        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => RegenerateIndices();
        private void RegenerateIndices()
        {
            Indices = new int[Rows.Count, Columns.Count];
            for (int i = 0; i < ChildComponents.Count; ++i)
                if (ChildComponents[i] is IUIComponent uic && uic.ParentInfo is GridPlacementInfo info)
                    Indices[info.Row, info.Column] = i;
        }

        public override void ArrangeChildren(Vec2 translation, Vec2 parentBounds)
        {
            base.ArrangeChildren(translation, parentBounds);

            float y = 0.0f;
            for (int row = 0; row < Rows.Count; ++row)
            {
                float height = Rows[row].Height.GetResultingValue(parentBounds);

                float x = 0.0f;
                for (int col = 0; col < Columns.Count; ++col)
                {
                    float width = Columns[col].Width.GetResultingValue(parentBounds);

                    int index = Indices[row, col];

                    ISceneComponent comp = ChildComponents[index];
                    if (comp is IUIComponent uic)
                    {
                        if (comp is IUIBoundableComponent bc)
                        {
                            bc.ArrangeChildren(new Vec2(x, y), new Vec2(width, height));
                        }
                        else
                        {

                        }
                    }

                    x += width;
                }

                y += height;
            }
        }

        protected override void OnChildAdded(ISceneComponent item)
        {
            if (item is IUIComponent uic)
            {
                if (!(uic.ParentInfo is GridPlacementInfo info))
                    uic.ParentInfo = info = new GridPlacementInfo();

                info.PropertyChanging += Info_PropertyChanging;
                info.PropertyChanged += Info_PropertyChanged;
            }

            base.OnChildAdded(item);
        }
        protected override void OnChildRemoved(ISceneComponent item)
        {
            if (item is IUIComponent uic)
            {
                if (uic.ParentInfo is GridPlacementInfo info)
                {
                    info.PropertyChanging -= Info_PropertyChanging;
                    info.PropertyChanged -= Info_PropertyChanged;
                }

                uic.ParentInfo = null;
            }

            base.OnChildRemoved(item);
        }

        private void Info_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            GridPlacementInfo info = sender as GridPlacementInfo;
            switch (e.PropertyName)
            {
                case nameof(GridPlacementInfo.Row):

                    break;
                case nameof(GridPlacementInfo.Column):

                    break;
            }
        }

        private void Info_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GridPlacementInfo info = sender as GridPlacementInfo;
            switch (e.PropertyName)
            {
                case nameof(GridPlacementInfo.Row):

                    break;
                case nameof(GridPlacementInfo.Column):

                    break;
            }
        }

        public class GridPlacementInfo : UIParentAttachmentInfo
        {
            private int _row = 0;
            private int _column = 0;

            [Category("Grid")]
            public int Row
            {
                get => _row;
                set => SetBackingField(ref _row, value);
            }
            [Category("Grid")]
            public int Column
            {
                get => _column;
                set => SetBackingField(ref _column, value);
            }
        }
        public class RowDefinition
        {
            public SizeableElement Height { get; set; }
        }
        public class ColumnDefinition
        {
            public SizeableElement Width { get; set; }
        }
    }
}