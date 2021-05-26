using MIConvexHull;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TheraEngine.Components;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Mesh;
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
        private bool _showCaptureSpheres = true;

        [Category(SceneComponent.RenderingCategoryName)]
        public IRenderInfo3D RenderInfo { get; } = new RenderInfo3D(false, true)
        {
#if EDITOR
            EditorVisibilityMode = EEditorVisibility.VisibleOnlyWhenSelected
#endif
        };
        
        [Category(SceneComponent.RenderingCategoryName)]
        public bool ShowPrefilterTexture
        {
            get => _showPrefilterTexture;
            set
            {
                _showPrefilterTexture = value;
                foreach (IBLProbeComponent probe in RootComponent.ChildSockets)
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
                foreach (IBLProbeComponent probe in RootComponent.ChildSockets)
                    probe.RenderInfo.IsVisible = _showCaptureSpheres;
            }
        }

        public IBLProbeGridActor() : base(true)
        {
            //ProbeBounds = new BoundingBox(100.0f);
            //ProbesPerMeter = new Vec3(0.01f);
            Initialize();
            RootComponent.ChildSockets.PostAddedRange += ChildComponents_PostAddedRange;
            RootComponent.ChildSockets.PostAdded += ChildComponents_PostAdded;
            _rc = new RenderCommandMethod3D(ERenderPass.TransparentForward, Render);
        }

        private void ChildComponents_PostAdded(ISocket item) => Link(item);
        private void ChildComponents_PostAddedRange(IEnumerable<ISocket> items) => Link(items);

        //public IBLProbeGridActor(BoundingBox bounds, Vec3 probesPerMeter) : base(true)
        //{
        //    ProbeBounds = bounds;
        //    ProbesPerMeter = probesPerMeter;
        //    Initialize();
        //}

        protected override void OnSpawnedPostComponentSpawn()
        {
            base.OnSpawnedPostComponentSpawn();

            IScene3D r3d = OwningScene3D;
            if (r3d != null && r3d.IBLProbeActor is null)
                r3d.IBLProbeActor = this;
        }
        protected override void OnDespawned()
        {
            base.OnDespawned();

            IScene3D r3d = OwningScene3D;
            if (r3d != null && r3d.IBLProbeActor == this)
                r3d.IBLProbeActor = null;
        }

        public void AddProbe(Vec3 position)
        {
            IBLProbeComponent probe = new IBLProbeComponent()
            {
                Translation = position,
            };
            RootComponent.ChildSockets.Add(probe);
            probe.WorldTransformChanged += Link;
        }
        
        public void SetFrequencies(BoundingBox bounds, Vec3 probesPerMeter)
        {
            RootComponent.ChildSockets.Clear();

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

            RootComponent.ChildSockets.AddRange(comps);
        }

        private class DelaunayTriVertex : IVertex
        {
            private ISocket _probe;

            public int Index => _probe?.ParentSocketChildIndex ?? -1;
            public double[] Position { get; set; }

            public DelaunayTriVertex(Vec3 point)
            {
                Position = new double[]
                {
                    point.X,
                    point.Y,
                    point.Z,
                };
            }
            public DelaunayTriVertex(ISocket probe) : this(probe?.WorldMatrix.Translation ?? Vec3.Zero)
            {
                _probe = probe;
                //_probe.WorldTransformChanged += _probe_WorldTransformChanged;
            }

            //private void _probe_WorldTransformChanged()
            //{
            //    Position[0] = _probe.WorldPoint.X;
            //    Position[1] = _probe.WorldPoint.Y;
            //    Position[2] = _probe.WorldPoint.Z;
            //}

            public override string ToString() => (_probe?.WorldMatrix.Translation ?? Vec3.Zero).ToString();
        }

        ITriangulation<DelaunayTriVertex, DefaultTriangulationCell<DelaunayTriVertex>> _cells;

        private void Link(ISocket comp)
        {
            if (RootComponent.ChildSockets.Count < 5)
                return;

            if (comp is IBLProbeComponent probe)
            {
                probe.Capture();
                probe.GenerateIrradianceMap();
                probe.GeneratePrefilterMap();
            }

            List<DelaunayTriVertex> points = RootComponent.ChildSockets.Select(x => new DelaunayTriVertex(x)).ToList();
            _cells = Triangulation.CreateDelaunay<DelaunayTriVertex, DefaultTriangulationCell<DelaunayTriVertex>>(points);
        }
        private void Link(IEnumerable<ISocket> comps)
        {
            if (RootComponent.ChildSockets.Count < 5)
                return;

            foreach (ISocket comp in comps)
            {
                if (comp is IBLProbeComponent probe)
                {
                    probe.Capture();
                    probe.GenerateIrradianceMap();
                    probe.GeneratePrefilterMap();
                }
            }

            List<DelaunayTriVertex> points = RootComponent.ChildSockets.Select(x => new DelaunayTriVertex(x)).ToList();
            _cells = Triangulation.CreateDelaunay<DelaunayTriVertex, DefaultTriangulationCell<DelaunayTriVertex>>(points);
        }

        protected override void OnSpawnedPreComponentSpawn()
        {
            //SetFrequencies(ProbeBounds, ProbesPerMeter);
            base.OnSpawnedPreComponentSpawn();
        }

        //public void CaptureAll()
        //{
        //    foreach (IBLProbeComponent comp in RootComponent.ChildComponents)
        //    {
        //        //comp.Capture();
        //        //comp.GenerateIrradianceMap();
        //        //comp.GeneratePrefilterMap();
        //    }
        //}
        public void InitAll(int colorResolution, bool captureDepth = false, int depthResolution = 1)
        {
            foreach (IBLProbeComponent comp in RootComponent.ChildSockets)
            {
                comp.SetCaptureResolution(colorResolution, captureDepth, depthResolution);
            }
        }
        public void InitAndCaptureAll(int colorResolution, bool captureDepth = false, int depthResolution = 1)
        {
            //if (ThreadSafeBlockingInvoke(
            //    (Action<int, bool, int>)InitAndCaptureAll, 
            //    EPanelType.Rendering, 
            //    colorResolution, captureDepth, depthResolution))
            //    return;

            foreach (IBLProbeComponent comp in RootComponent.ChildSockets)
                comp.FullCapture(colorResolution, captureDepth, depthResolution);
        }

        public async Task InitAndCaptureAllAsync(int colorResolution, bool captureDepth = false, int depthResolution = 1)
        {
            await Task.Run(() => InitAndCaptureAll(colorResolution, captureDepth, depthResolution));
        }

        private readonly RenderCommandMethod3D _rc;
        public void AddRenderables(RenderPasses passes, ICamera camera)
        {
            passes.Add(_rc);
        }
        private void Render(bool shadowPass)
        {
            if (_cells is null)
                return;

            foreach (var cell in _cells.Cells)
                for (int i = 0; i < 4; ++i)
                    for (int x = 0; x < 4; ++x)
                    {
                        if (i == x)
                            continue;

                        var p = cell.Vertices[i].Position;
                        Vec3 pos1 = new Vec3((float)p[0], (float)p[1], (float)p[2]);

                        var p2 = cell.Vertices[x].Position;
                        Vec3 pos2 = new Vec3((float)p2[0], (float)p2[1], (float)p2[2]);

                        Engine.Renderer.RenderLine(pos1, pos2, Color.Black, false, 1.0f);
                    }
        }
    }
}
