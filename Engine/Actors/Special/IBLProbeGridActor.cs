using MIConvexHull;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TheraEngine.Actors.Types;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Actors
{
    public class IBLProbeGridActor : Actor<TranslationComponent>, I3DRenderable
    {
        private bool _showPrefilterTexture = false;
        private bool _showCaptureSpheres = false;

        [Category(SceneComponent.RenderingCategoryName)]
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(false, true);

        [Browsable(false)]
        public TShape CullingVolume => null;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }
        [Category(SceneComponent.RenderingCategoryName)]
        public bool ShowPrefilterTexture
        {
            get => _showPrefilterTexture;
            set
            {
                _showPrefilterTexture = value;
                foreach (IBLProbeComponent probe in RootComponent.ChildComponents)
                    probe.ShowPrefilterTexture = _showPrefilterTexture;
            }
        }
        [Category(SceneComponent.RenderingCategoryName)]
        public bool CaptureSpheresVisible
        {
            get => _showCaptureSpheres;
            set
            {
                _showCaptureSpheres = value;
                foreach (IBLProbeComponent probe in RootComponent.ChildComponents)
                    probe.RenderInfo.Visible = _showCaptureSpheres;
            }
        }

        public IBLProbeGridActor() : base(true)
        {
            //ProbeBounds = new BoundingBox(100.0f);
            //ProbesPerMeter = new Vec3(0.01f);
            Initialize();
            RootComponent.ChildComponents.PostAddedRange += ChildComponents_PostAddedRange;
            RootComponent.ChildComponents.PostAdded += ChildComponents_PostAdded;
            _rc = new RenderCommandMethod3D(ERenderPass.TransparentForward, Render);
        }

        private void ChildComponents_PostAdded(SceneComponent item) => Link();
        private void ChildComponents_PostAddedRange(IEnumerable<SceneComponent> items) => Link();

        //public IBLProbeGridActor(BoundingBox bounds, Vec3 probesPerMeter) : base(true)
        //{
        //    ProbeBounds = bounds;
        //    ProbesPerMeter = probesPerMeter;
        //    Initialize();
        //}

        protected override void OnSpawnedPostComponentSpawn()
        {
            base.OnSpawnedPostComponentSpawn();

            Scene3D r3d = OwningScene3D;
            if (r3d != null && r3d.IBLProbeActor == null)
                r3d.IBLProbeActor = this;

            OwningWorld.Scene.Add(this);
        }
        protected override void OnDespawned()
        {
            base.OnDespawned();

            Scene3D r3d = OwningScene3D;
            if (r3d != null && r3d.IBLProbeActor == this)
                r3d.IBLProbeActor = null;

            OwningWorld.Scene.Remove(this);
        }

        public void AddProbe(Vec3 position)
        {
            IBLProbeComponent probe = new IBLProbeComponent()
            {
                Translation = position,
            };
            RootComponent.ChildComponents.Add(probe);
            probe.WorldTransformChanged += Link;
        }
        
        public void SetFrequencies(BoundingBox bounds, Vec3 probesPerMeter)
        {
            RootComponent.ChildComponents.Clear();

            Vec3 extents = bounds.HalfExtents * 2.0f;
            IVec3 probeCount = new IVec3(
                (int)(extents.X * probesPerMeter.X),
                (int)(extents.Y * probesPerMeter.Y),
                (int)(extents.Z * probesPerMeter.Z));
            Vec3 localMin = bounds.Minimum;
            Vec3 probeInc = new Vec3(
                extents.X / probeCount.X,
                extents.Y / probeCount.Y,
                extents.Z / probeCount.Z);
            Vec3 baseInc = probeInc * 0.5f;

            IBLProbeComponent[] comps = new IBLProbeComponent[probeCount.X * probeCount.Y * probeCount.Z];

            int r = 0;
            for (int x = 0; x < probeCount.X; ++x)
                for (int y = 0; y < probeCount.Y; ++y)
                    for (int z = 0; z < probeCount.Z; ++z)
                    {
                        IBLProbeComponent comp = new IBLProbeComponent()
                        {
                            Translation = localMin + baseInc + new Vec3(x, y, z) * probeInc
                        };
                        comps[r++] = comp;
                    }

            RootComponent.ChildComponents.AddRange(comps);
        }

        private class DelaunayTriVertex : IVertex
        {
            private SceneComponent _probe;

            public double[] Position { get; set; }

            public DelaunayTriVertex(Vec3 point) { }
            public DelaunayTriVertex(SceneComponent probe)
            {
                _probe = probe;
                Vec3 point = probe.WorldPoint;
                Position = new double[]
                {
                    point.X,
                    point.Y,
                    point.Z,
                };
                //_probe.WorldTransformChanged += _probe_WorldTransformChanged;
            }

            //private void _probe_WorldTransformChanged()
            //{
            //    Position[0] = _probe.WorldPoint.X;
            //    Position[1] = _probe.WorldPoint.Y;
            //    Position[2] = _probe.WorldPoint.Z;
            //}

            public override string ToString() => _probe.WorldPoint.ToString();
        }

        ITriangulation<DelaunayTriVertex, DefaultTriangulationCell<DelaunayTriVertex>> _cells;

        private void Link()
        {
            if (RootComponent.ChildComponents.Count < 5)
                return;
            List<DelaunayTriVertex> points = RootComponent.ChildComponents.Select(x => new DelaunayTriVertex(x)).ToList();
            _cells = Triangulation.CreateDelaunay<DelaunayTriVertex, DefaultTriangulationCell<DelaunayTriVertex>>(points);
        }

        protected override void OnSpawnedPreComponentSpawn()
        {
            //SetFrequencies(ProbeBounds, ProbesPerMeter);
            base.OnSpawnedPreComponentSpawn();
        }

        public void CaptureAll()
        {
            foreach (IBLProbeComponent comp in RootComponent.ChildComponents)
            {
                //comp.Capture();
                //comp.GenerateIrradianceMap();
                //comp.GeneratePrefilterMap();
            }
        }
        public void InitAll(int colorResolution, bool captureDepth = false, int depthResolution = 1)
        {
            foreach (IBLProbeComponent comp in RootComponent.ChildComponents)
            {
                comp.SetCaptureResolution(colorResolution, captureDepth, depthResolution);
            }
        }
        public void InitAndCaptureAll(int colorResolution, bool captureDepth = false, int depthResolution = 1)
        {
            if (BaseRenderPanel.ThreadSafeBlockingInvoke((Action<int, bool, int>)InitAndCaptureAll, BaseRenderPanel.PanelType.Rendering, colorResolution, captureDepth, depthResolution))
                return;

            foreach (IBLProbeComponent comp in RootComponent.ChildComponents)
                comp.FullCapture(colorResolution, captureDepth, depthResolution);
        }

        public async Task InitAndCaptureAllAsync(int colorResolution, bool captureDepth = false, int depthResolution = 1)
        {
            await Task.Run(() => InitAndCaptureAll(colorResolution, captureDepth, depthResolution));
        }

        private readonly RenderCommandMethod3D _rc;
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            passes.Add(_rc);
        }
        private void Render()
        {
            //if (_cells == null)
            //    return;

            //foreach (var cell in _cells.Cells)
            //    for (int i = 0; i < 4; ++i)
            //        for (int x = 0; x < 4; ++x)
            //        {
            //            if (i == x)
            //                continue;

            //            var p = cell.Vertices[i].Position;
            //            Vec3 pos1 = new Vec3((float)p[0], (float)p[1], (float)p[2]);

            //            var p2 = cell.Vertices[x].Position;
            //            Vec3 pos2 = new Vec3((float)p2[0], (float)p2[1], (float)p2[2]);

            //            Engine.Renderer.RenderLine(pos1, pos2, Color.Black, false, 1.0f);
            //        }
        }
    }
}
