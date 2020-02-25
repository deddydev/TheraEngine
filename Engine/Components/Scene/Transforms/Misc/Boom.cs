using Extensions;
using System;
using System.Drawing;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
using TheraEngine.Physics.ShapeTracing;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Components.Scene.Transforms
{
    public delegate void DelBoomLengthChange(float newLength);

    [TFileDef("Boom Component")]
    public class BoomComponent : RTComponent, I3DRenderable
    {
        public event DelBoomLengthChange CurrentDistanceChanged;

        private TCollisionSphere _traceShape = TCollisionSphere.New(0.3f);
        private float _currentLength = 0.0f;
        private Vec3 _startPoint = Vec3.Zero;
        
        public float TraceRadius
        {
            get => _traceShape.Radius;
            set => _traceShape.Radius = value;
        }

        [TSerialize]
        public float InterpSpeed { get; set; } = 15.0f;
        [TSerialize]
        public float MaxLength { get; set; } = 300.0f;
        [TSerialize]
        public LocalFileRef<TCollisionObject> IgnoreCast { get; set; } = null;
        [TSerialize]
        public IRenderInfo3D RenderInfo { get; protected set; } = new RenderInfo3D(false, true)
        {
#if EDITOR
            EditorVisibilityMode = EEditorVisibility.VisibleOnlyWhenSelected
#endif
        };

        public BoomComponent() : base() { _rc = new RenderCommandMethod3D(ERenderPass.OpaqueForward, Render); }

        private void Render(bool shadowPass)
        {
            Engine.Renderer.RenderSphere(WorldPoint, _traceShape.Radius, false, Color.Black);
            Engine.Renderer.RenderLine(ParentWorldMatrix.Translation, WorldPoint, Color.Black);
        }

#if EDITOR
        protected internal override void OnSelectedChanged(bool selected)
        {
            RenderInfo.IsVisible = selected;
        }
#endif

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            Matrix4
                r = _rotation.GetMatrix(),
                ir = _rotation.GetInverseMatrix();
            Matrix4
                t = _translation.AsTranslationMatrix(),
                it = (-_translation).AsTranslationMatrix();
            Matrix4 
                translation = Matrix4.CreateTranslation(0.0f, 0.0f, _currentLength),
                invTranslation = Matrix4.CreateTranslation(0.0f, 0.0f, -_currentLength);

            localTransform = r * t * translation;
            inverseLocalTransform = invTranslation * it * ir;
        }

        public override void OnSpawned()
        {
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
            RenderInfo.LinkScene(this, OwningScene3D);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
            RenderInfo.UnlinkScene();
            base.OnDespawned();
        }

        private void Tick(float delta)
        {
            Matrix4 startMatrix = ParentWorldMatrix * Rotation.GetMatrix() * Translation.AsTranslationMatrix();
            _startPoint = startMatrix.Translation;
            Matrix4 endMatrix = startMatrix * Matrix4.CreateTranslation(new Vec3(0.0f, 0.0f, MaxLength));
            Vec3 testEnd = endMatrix.Translation;

            ShapeTraceClosest result = new ShapeTraceClosest(_traceShape, startMatrix, endMatrix, (ushort)ETheraCollisionGroup.Camera, (ushort)ETheraCollisionGroup.All, IgnoreCast);

            Vec3 newEndPoint;
            if (result.Trace(OwningWorld))
                newEndPoint = result.HitPointWorld;
            else
                newEndPoint = testEnd;
            float newLength = (newEndPoint - _startPoint).LengthFast;
            if (!newLength.EqualTo(_currentLength, 0.001f))
            {
                if (newLength < _currentLength)
                    _currentLength = newLength; //Moving closer to the character, meaning something is obscuring the view. Need to jump to the right position.
                else //Nothing is now obscuring the view, so we can lerp out quickly to give the appearance of a clean camera zoom out
                    _currentLength = Interp.Lerp(_currentLength, newLength, delta, 15.0f);

                RecalcLocalTransform();
                CurrentDistanceChanged?.Invoke(_currentLength);
            }
        }
        private RenderCommandMethod3D _rc;
        public void AddRenderables(RenderPasses passes, ICamera camera)
        {
            passes.Add(_rc);
        }
    }
}
