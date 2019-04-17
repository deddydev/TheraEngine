using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using TheraEngine.Components.Scene;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Editor
{
#if EDITOR
    public delegate void DelPropertyChange(EditorState state, string propertyValue, object oldValue, object newValue);
    public delegate void DelHighlightingChange(bool isHighlighted);
    public delegate void DelSelectedChange(bool isSelected);
    public class EditorState : TObject
    {
        public EditorState(IObject obj) => Object = obj;

        public static event DelPropertyChange PropertyChanged;
        public static event DelHighlightingChange HighlightingChanged;
        public static event DelSelectedChange SelectedChanged;

        private Dictionary<string, List<object>> _changedProperties = new Dictionary<string, List<object>>();
        private bool _highlighted = false;
        private bool _selected = false;

        private static EditorState _selectedState, _highlightedState;
        public static EditorState SelectedState
        {
            get => _selectedState;
            set
            {
                _selectedState?.OnSelectedChanged(false);
                _selectedState = value;
                _selectedState?.OnSelectedChanged(true);
            }
        }
        public static EditorState HighlightedState
        {
            get => _highlightedState;
            set
            {
                _highlightedState?.OnHighlightedChanged(false);
                _highlightedState = value;
                _highlightedState?.OnHighlightedChanged(true);
            }
        }

        //Contains information about the default value for each member.
        public Dictionary<string, object> MemberDefaults { get; set; }

        public IObject Object { get; internal set; }
        public bool HasChanges => _changedProperties.Count > 0;
        public bool Highlighted
        {
            get => _highlighted;
            set
            {
                if (_highlighted == value)
                    return;
                HighlightedState = value ? this : null;
            }
        }
        public bool Selected
        {
            get => _selected;
            set
            {
                if (_selected == value)
                    return;
                SelectedState = value ? this : null;
            }
        }
        public TreeNode TreeNode { get; set; }
        public bool IsDirty { get; set; }
        public List<LocalValueChange> ChangedValues { get; } = new List<LocalValueChange>();
        public static List<EditorState> DirtyStates { get; } = new List<EditorState>();
        public bool DisplayInActorTree { get; set; } = true;

        private new void OnSelectedChanged(bool selected)
        {
            if (Object == this)
                return;

            _selected = selected;
            Object?.OnSelectedChanged(_selected);
            SelectedChanged?.Invoke(_selected);
        }
        private new void OnHighlightedChanged(bool highlighted)
        {
            if (Object == this)
                return;

            _highlighted = highlighted;
            Object?.OnHighlightChanged(_highlighted);
            HighlightingChanged?.Invoke(highlighted);
        }
        public GlobalValueChange AddChanges(params LocalValueChange[] changes)
        {
            GlobalValueChange globalChange = new GlobalValueChange
            {
                ChangedStates = new List<(EditorState State, int ChangeIndex)>()
            };
            foreach (LocalValueChange change in changes)
            {
                globalChange.ChangedStates.Add((this, ChangedValues.Count));
                change.GlobalChange = globalChange;
                ChangedValues.Add(change);
            }
            IsDirty = true;
            return globalChange;
        }
        //public void AddChange(object oldValue, object newValue, IList list, int index, GlobalValueChange change)
        //{
        //    ChangedValues.Add(new ListValueChange()
        //    {
        //        GlobalChange = change,
        //        OldValue = oldValue,
        //        NewValue = newValue,
        //        List = list,
        //        Index = index,
        //    });
        //    IsDirty = true;
        //}
        //public void AddChange(object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo, GlobalValueChange change)
        //{
        //    ChangedValues.Add(new PropertyValueChange()
        //    {
        //        GlobalChange = change,
        //        OldValue = oldValue,
        //        NewValue = newValue,
        //        PropertyOwner = propertyOwner,
        //        PropertyInfo = propertyInfo,
        //    });
        //    IsDirty = true;
        //}
        //public void AddChange(object oldValue, object newValue, IDictionary dicOwner, object key, bool isKey, GlobalValueChange change)
        //{
        //    ChangedValues.Add(new DictionaryValueChange()
        //    {
        //        GlobalChange = change,
        //        OldValue = oldValue,
        //        NewValue = newValue,
        //        DictionaryOwner = dicOwner,
        //        Key = key,
        //        IsKey = isKey,
        //    });
        //    IsDirty = true;
        //}

        private static Dictionary<int, StencilTest> 
            _highlightedMaterials = new Dictionary<int, StencilTest>(), 
            _selectedMaterials = new Dictionary<int, StencilTest>();
        internal static void RegisterHighlightedMaterial(TMaterial m, bool highlighted, BaseScene scene)
        {
            //if (m == null)
            //    return;
            //if (highlighted)
            //{
            //    if (_highlightedMaterials.ContainsKey(m.UniqueID))
            //    {
            //        //m.RenderParams.StencilTest.BackFace.Ref |= 1;
            //        //m.RenderParams.StencilTest.FrontFace.Ref |= 1;
            //        return;
            //    }
            //    _highlightedMaterials.Add(m.UniqueID, m.RenderParams.StencilTest);
            //    m.RenderParams.StencilTest = OutlinePassStencil;
            //}
            //else
            //{
            //    if (!_highlightedMaterials.ContainsKey(m.UniqueID))
            //    {
            //        //m.RenderParams.StencilTest.BackFace.Ref &= ~1;
            //        //m.RenderParams.StencilTest.FrontFace.Ref &= ~1;
            //        return;
            //    }
            //    StencilTest t = _highlightedMaterials[m.UniqueID];
            //    _highlightedMaterials.Remove(m.UniqueID);
            //    m.RenderParams.StencilTest = _selectedMaterials.ContainsKey(m.UniqueID) ? _selectedMaterials[m.UniqueID] : t;
            //}
        }

        public static void RegisterSelectedMesh(TMaterial m, bool selected, BaseScene scene)
        {
            //if (m == null)
            //    return;
            //if (selected)
            //{
            //    if (_selectedMaterials.ContainsKey(m.UniqueID))
            //    {
            //        //m.RenderParams.StencilTest.BackFace.Ref |= 2;
            //        //m.RenderParams.StencilTest.FrontFace.Ref |= 2;
            //        return;
            //    }
            //    else
            //    {
            //        _selectedMaterials.Add(m.UniqueID, m.RenderParams.StencilTest);
            //        m.RenderParams.StencilTest = OutlinePassStencil;
            //        //m.RenderParams.StencilTest.BackFace.Ref |= 2;
            //        //m.RenderParams.StencilTest.FrontFace.Ref |= 2;
            //    }
            //}
            //else
            //{
            //    if (!_selectedMaterials.ContainsKey(m.UniqueID))
            //    {
            //        //m.RenderParams.StencilTest.BackFace.Ref &= ~2;
            //        //m.RenderParams.StencilTest.FrontFace.Ref &= ~2;
            //        return;
            //    }
            //    StencilTest t = _selectedMaterials[m.UniqueID];
            //    _selectedMaterials.Remove(m.UniqueID);
            //    m.RenderParams.StencilTest = t;
            //}
        }

        //public static StencilTest NormalPassStencil = new StencilTest()
        //{
        //    Enabled = ERenderParamUsage.Enabled,
        //    //BothFailOp = EStencilOp.Keep,
        //    //StencilPassDepthFailOp = EStencilOp.Keep,
        //    //BothPassOp = EStencilOp.Replace,
        //    BackFace = new StencilTestFace()
        //    {
        //        Func = EComparison.Always,
        //        Ref = 0,
        //        WriteMask = 0,
        //        ReadMask = 0,
        //    },
        //    FrontFace = new StencilTestFace()
        //    {
        //        Func = EComparison.Always,
        //        Ref = 0,
        //        WriteMask = 0,
        //        ReadMask = 0,
        //    },
        //};
        public static StencilTest OutlinePassStencil = new StencilTest()
        {
            Enabled = ERenderParamUsage.Enabled,
            BackFace = new StencilTestFace()
            {
                BothFailOp = EStencilOp.Keep,
                StencilPassDepthFailOp = EStencilOp.Replace,
                BothPassOp = EStencilOp.Replace,
                Func = EComparison.Always,
                Ref = 1,
                WriteMask = 0xFF,
                ReadMask = 0xFF,
            },
            FrontFace = new StencilTestFace()
            {
                BothFailOp = EStencilOp.Keep,
                StencilPassDepthFailOp = EStencilOp.Replace,
                BothPassOp = EStencilOp.Replace,
                Func = EComparison.Always,
                Ref = 1,
                WriteMask = 0xFF,
                ReadMask = 0xFF,
            },
        };
        //public static TMaterial FocusedMeshMaterial;
        //private static void M_PreRendered(BaseRenderableMesh mesh, Matrix4 matrix, Matrix3 normalMatrix, TMaterial material, BaseRenderableMesh.PreRenderCallback callback)
        //{
        //    callback.ShouldRender = false;
        //    TMaterial m = mesh.CurrentLOD.Manager.GetRenderMaterial(material);
        //    StencilTest prev = m.RenderParams.StencilTest;
        //    m.RenderParams.StencilTest = NormalPassStencil;
        //    mesh.Render(m, false, false);
        //    mesh.Render(FocusedMeshMaterial, false, false);
        //    m.RenderParams.StencilTest = prev;
        //}
        //static EditorState()
        //{
        //    FocusedMeshMaterial = TMaterial.CreateLitColorMaterial(Color.Yellow, true);
        //    FocusedMeshMaterial.AddShader(Engine.LoadEngineShader("StencilExplode.gs", EShaderMode.Geometry));
        //    FocusedMeshMaterial.RenderParams.StencilTest = OutlinePassStencil;
        //    FocusedMeshMaterial.RenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
        //}
    }
    public class EngineEditorState
    {
        /// <summary>
        /// Used to determine if the editor is editing the game currently instead of simulating gameplay.
        /// </summary>
        public bool InEditMode { get; set; } = true;
        public CameraComponent PinnedCameraComponent { get; set; }
    }

    /// <summary>
    /// Contains information pertaining to a change in a global setting.
    /// </summary>
    public class GlobalValueChange
    {
        public List<(EditorState State, int ChangeIndex)> ChangedStates { get; set; }
        //public EditorState State { get; set; }
        //public int ChangeIndex { get; set; }

        public void ApplyNewValue()
        {
            foreach (var (State, ChangeIndex) in ChangedStates)
                State.ChangedValues[ChangeIndex].ApplyNewValue();
        }
        public void ApplyOldValue()
        {
            foreach (var (State, ChangeIndex) in ChangedStates)
                State.ChangedValues[ChangeIndex].ApplyOldValue();
        }

        public void DestroySelf()
        {
            for (int i = 0; i < ChangedStates.Count; ++i)
            {
                var (State, ChangeIndex) = ChangedStates[i];

                State.ChangedValues.RemoveAt(ChangeIndex);

                //Update all local changes after the one that was just removed
                //Their global state's change index needs to be decremented to match the new index
                for (int x = ChangeIndex; x < State.ChangedValues.Count; ++x)
                    --ChangeIndex;

                if (State.ChangedValues.Count == 0)
                    State.IsDirty = false;
            }
        }

        public string AsUndoString()
        {
            string s = "";
            for (int i = 0; i < ChangedStates.Count; ++i)
            {
                var (State, ChangeIndex) = ChangedStates[i];
                if (i > 0)
                    s += ", ";
                s += $"({State.ChangedValues[ChangeIndex].DisplayChangeAsUndo()}";
            }
            return s;
        }
        public string AsRedoString()
        {
            string s = "";
            for (int i = 0; i < ChangedStates.Count; ++i)
            {
                var (State, ChangeIndex) = ChangedStates[i];
                if (i > 0)
                    s += ", ";
                s += $"({State.ChangedValues[ChangeIndex].DisplayChangeAsRedo()}";
            }
            return s;
        }
        public override string ToString() => AsRedoString();
    }
    /// <summary>
    /// Contains information pertaining to a change on a specific object.
    /// </summary>
    public abstract class LocalValueChange
    {
        public LocalValueChange(object oldValue, object newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public GlobalValueChange GlobalChange { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }

        public abstract void ApplyNewValue();
        public abstract void ApplyOldValue();
        public abstract string DisplayChangeAsUndo();
        public abstract string DisplayChangeAsRedo();

        public override string ToString() => DisplayChangeAsRedo();
    }
    public class LocalValueChangeIList : LocalValueChange
    {
        public LocalValueChangeIList(object oldValue, object newValue, IList list, int index) : base(oldValue, newValue)
        {
            List = list;
            Index = index;
        }

        public IList List { get; set; }
        public int Index { get; set; }

        public override void ApplyNewValue()
            => List[Index] = NewValue;
        public override void ApplyOldValue()
            => List[Index] = OldValue;

        public override string DisplayChangeAsRedo()
        {
            return string.Format("{0}[{1}] {2} -> {3}",
                List.ToString(), Index.ToString(),
              OldValue?.ToString() ?? "null", NewValue?.ToString() ?? "null");
        }

        public override string DisplayChangeAsUndo()
        {
            return string.Format("{0}[{1}] {2} <- {3}",
                List.ToString(), Index.ToString(),
                OldValue?.ToString() ?? "null", NewValue?.ToString() ?? "null");
        }
    }
    public class LocalValueChangeField : LocalValueChange
    {
        public LocalValueChangeField(object oldValue, object newValue, object fieldOwner, FieldInfo fieldInfo) : base(oldValue, newValue)
        {
            FieldOwner = fieldOwner;
            FieldInfo = fieldInfo;
        }

        public object FieldOwner { get; set; }
        public FieldInfo FieldInfo { get; set; }

        public override void ApplyNewValue()
            => FieldInfo.SetValue(FieldOwner, NewValue);
        public override void ApplyOldValue()
            => FieldInfo.SetValue(FieldOwner, OldValue);

        public override string DisplayChangeAsRedo()
        {
            return string.Format("{0}.{1} {2} -> {3}",
              FieldOwner.ToString(), FieldInfo.Name.ToString(),
              OldValue == null ? "null" : OldValue.ToString(),
              NewValue == null ? "null" : NewValue.ToString());
        }

        public override string DisplayChangeAsUndo()
        {
            return string.Format("{0}.{1} {2} <- {3}",
              FieldOwner.ToString(), FieldInfo.Name.ToString(),
              OldValue == null ? "null" : OldValue.ToString(),
              NewValue == null ? "null" : NewValue.ToString());
        }
    }
    public class LocalValueChangeProperty : LocalValueChange
    {
        public LocalValueChangeProperty(object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo) : base(oldValue, newValue)
        {
            PropertyOwner = propertyOwner;
            PropertyInfo = propertyInfo;
        }

        public object PropertyOwner { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

        public override void ApplyNewValue()
            => PropertyInfo.SetValue(PropertyOwner, NewValue);
        public override void ApplyOldValue()
            => PropertyInfo.SetValue(PropertyOwner, OldValue);

        public override string DisplayChangeAsRedo()
        {
            return string.Format("{0}.{1} {2} -> {3}",
              PropertyOwner.ToString(), PropertyInfo.Name.ToString(),
              OldValue == null ? "null" : OldValue.ToString(),
              NewValue == null ? "null" : NewValue.ToString());
        }

        public override string DisplayChangeAsUndo()
        {
            return string.Format("{0}.{1} {2} <- {3}",
              PropertyOwner.ToString(), PropertyInfo.Name.ToString(),
              OldValue == null ? "null" : OldValue.ToString(),
              NewValue == null ? "null" : NewValue.ToString());
        }
    }
    public class LocalValueChangeIDictionary : LocalValueChange
    {
        public IDictionary DictionaryOwner { get; set; }
        public object KeyForValue { get; set; }
        public bool ValueIsKey { get; set; }

        public LocalValueChangeIDictionary(object oldValue, object newValue, IDictionary dictionary, object keyForValue, bool valueIsKey) : base(oldValue, newValue)
        {
            DictionaryOwner = dictionary;
            KeyForValue = keyForValue;
            ValueIsKey = valueIsKey;
        }

        public override void ApplyNewValue()
        {
            if (!ValueIsKey)
                DictionaryOwner[KeyForValue] = NewValue;
            else
            {
                if (!DictionaryOwner.Contains(OldValue))
                    return;
                object value = DictionaryOwner[OldValue];
                if (DictionaryOwner.Contains(NewValue))
                    DictionaryOwner[NewValue] = value;
                else
                    DictionaryOwner.Add(NewValue, value);
                DictionaryOwner.Remove(OldValue);
            }
        }
        public override void ApplyOldValue()
        {
            if (!ValueIsKey)
                DictionaryOwner[KeyForValue] = OldValue;
            else
            {
                if (!DictionaryOwner.Contains(NewValue))
                    return;
                object value = DictionaryOwner[NewValue];
                if (DictionaryOwner.Contains(OldValue))
                    DictionaryOwner[OldValue] = value;
                else
                    DictionaryOwner.Add(OldValue, value);
                DictionaryOwner.Remove(NewValue);
            }
        }

        public override string DisplayChangeAsRedo()
        {
            return string.Format("{0}.{1} {2} -> {3}",
              DictionaryOwner.ToString(), KeyForValue.ToString(),
              OldValue == null ? "null" : OldValue.ToString(),
              NewValue == null ? "null" : NewValue.ToString());
        }

        public override string DisplayChangeAsUndo()
        {
            return string.Format("{0}.{1} {2} <- {3}",
              DictionaryOwner.ToString(), KeyForValue.ToString(),
              OldValue == null ? "null" : OldValue.ToString(),
              NewValue == null ? "null" : NewValue.ToString());
        }
    }
#endif
}
