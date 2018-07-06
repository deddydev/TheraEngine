using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Editor
{
#if EDITOR
    public delegate void DelPropertyChange(EditorState state, string propertyValue, object oldValue, object newValue);
    public delegate void DelHighlightingChange(bool isHighlighted);
    public delegate void DelSelectedChange(bool isSelected);
    public class EditorState
    {
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
        
        public static event DelPropertyChange PropertyChanged;
        public static event DelHighlightingChange HighlightingChanged;
        public static event DelSelectedChange SelectedChanged;

        private void OnSelectedChanged(bool selected)
        {
            _selected = selected;
            _object.OnSelectedChanged(_selected);
            SelectedChanged?.Invoke(_selected);
        }
        private void OnHighlightedChanged(bool highlighted)
        {
            _highlighted = highlighted;
            _object.OnHighlightChanged(_highlighted);
            HighlightingChanged?.Invoke(highlighted);
        }

        public EditorState(TObject obj)
        {
            _object = obj;
        }

        private TObject _object;
        private TreeNode _treeNode;
        private Dictionary<string, List<object>> _changedProperties = new Dictionary<string, List<object>>();
        private bool _highlighted = false, _selected = false;

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
        
        public TreeNode TreeNode
        {
            get => _treeNode;
            set => _treeNode = value;
        }

        public bool IsDirty { get; set; }
        public List<LocalValueChange> ChangedValues { get; } = new List<LocalValueChange>();
        public abstract class LocalValueChange
        {
            public GlobalValueChange GlobalChange { get; set; }
            public object OldValue { get; set; }
            public object NewValue { get; set; }

            public abstract void ApplyNewValue();
            public abstract void ApplyOldValue();
            public abstract string DisplayChangeAsUndo();
            public abstract string DisplayChangeAsRedo();
        }
        public class ListValueChange : LocalValueChange
        {
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

            public override string ToString()
                => DisplayChangeAsRedo();
        }
        public class PropertyValueChange : LocalValueChange
        {
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

            public override string ToString() => DisplayChangeAsRedo();
        }
        public class DictionaryValueChange : LocalValueChange
        {
            public IDictionary DictionaryOwner { get; set; }
            public object Key { get; set; }
            public bool IsKey { get; set; }

            public override void ApplyNewValue()
            {
                if (!IsKey)
                    DictionaryOwner[Key] = NewValue;
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
                if (!IsKey)
                    DictionaryOwner[Key] = OldValue;
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
                  DictionaryOwner.ToString(), Key.ToString(),
                  OldValue == null ? "null" : OldValue.ToString(),
                  NewValue == null ? "null" : NewValue.ToString());
            }

            public override string DisplayChangeAsUndo()
            {
                return string.Format("{0}.{1} {2} <- {3}",
                  DictionaryOwner.ToString(), Key.ToString(),
                  OldValue == null ? "null" : OldValue.ToString(),
                  NewValue == null ? "null" : NewValue.ToString());
            }

            public override string ToString() => DisplayChangeAsRedo();
        }
        public class GlobalValueChange
        {
            public EditorState State { get; set; }
            public int ChangeIndex { get; set; }

            public void ApplyNewValue()
                => State.ChangedValues[ChangeIndex].ApplyNewValue();
            public void ApplyOldValue()
                => State.ChangedValues[ChangeIndex].ApplyOldValue();

            public void DestroySelf()
            {
                State.ChangedValues.RemoveAt(ChangeIndex);

                //Update all local changes after the one that was just removed
                //Their global state's change index needs to be decremented to match the new index
                for (int i = ChangeIndex; i < State.ChangedValues.Count; ++i)
                {
                    --State.ChangedValues[i].GlobalChange.ChangeIndex;
                }

                if (State.ChangedValues.Count == 0)
                    State.IsDirty = false;
            }

            public string AsUndoString()
            {
                return State.ChangedValues[ChangeIndex].DisplayChangeAsUndo();
            }
            public string AsRedoString()
            {
                return State.ChangedValues[ChangeIndex].DisplayChangeAsRedo();
            }
            public override string ToString()
            {
                return State.ChangedValues[ChangeIndex].DisplayChangeAsRedo();
            }
        }

        public void AddChange(object oldValue, object newValue, IList list, int index, GlobalValueChange change)
        {
            ChangedValues.Add(new ListValueChange()
            {
                GlobalChange = change,
                OldValue = oldValue,
                NewValue = newValue,
                List = list,
                Index = index,
            });
            IsDirty = true;
        }
        public void AddChange(object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo, GlobalValueChange change)
        {
            ChangedValues.Add(new PropertyValueChange()
            {
                GlobalChange = change,
                OldValue = oldValue,
                NewValue = newValue,
                PropertyOwner = propertyOwner,
                PropertyInfo = propertyInfo,
            });
            IsDirty = true;
        }
        public void AddChange(object oldValue, object newValue, IDictionary dicOwner, object key, bool isKey, GlobalValueChange change)
        {
            ChangedValues.Add(new DictionaryValueChange()
            {
                GlobalChange = change,
                OldValue = oldValue,
                NewValue = newValue,
                DictionaryOwner = dicOwner,
                Key = key,
                IsKey = isKey,
            });
            IsDirty = true;
        }

        private static Dictionary<int, StencilTest> 
            _highlightedMaterials = new Dictionary<int, StencilTest>(), 
            _selectedMaterials = new Dictionary<int, StencilTest>();
        internal static void RegisterHighlightedMaterial(TMaterial m, bool highlighted, Scene3D scene)
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

        public static void RegisterSelectedMesh(TMaterial m, bool selected, Scene3D scene)
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
    }
#endif
}
