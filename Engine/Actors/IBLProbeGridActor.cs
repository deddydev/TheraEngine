using MIConvexHull;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TheraEngine.Actors.Types;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Actors
{
    public class IBLProbeGridActor : Actor<TranslationComponent>
    {
        //public Vec3 ProbesPerMeter { get; internal set; }
        //public BoundingBox ProbeBounds { get; internal set; }

        public IBLProbeGridActor() : base(true)
        {
            //ProbeBounds = new BoundingBox(100.0f);
            //ProbesPerMeter = new Vec3(0.01f);
            Initialize();
        }
        //public IBLProbeGridActor(BoundingBox bounds, Vec3 probesPerMeter) : base(true)
        //{
        //    ProbeBounds = bounds;
        //    ProbesPerMeter = probesPerMeter;
        //    Initialize();
        //}

        public override void OnSpawnedPostComponentSetup()
        {
            base.OnSpawnedPostComponentSetup();

            if (OwningWorld.Scene.IBLProbeActor == null)
                OwningWorld.Scene.IBLProbeActor = this;
        }
        public override void OnDespawned()
        {
            base.OnDespawned();

            if (OwningWorld.Scene.IBLProbeActor == this)
                OwningWorld.Scene.IBLProbeActor = null;
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

            IBLProbeComponent[] comps = new IBLProbeComponent[1/*probeCount.X * probeCount.Y * probeCount.Z*/];

            int r = 0;
            //for (int x = 0; x < probeCount.X; ++x)
            //    for (int y = 0; y < probeCount.Y; ++y)
            //        for (int z = 0; z < probeCount.Z; ++z)
                    {
                        IBLProbeComponent comp = new IBLProbeComponent()
                        {
                            //Translation = localMin + baseInc + new Vec3(x, y, z) * probeInc
                        };
                        comps[r++] = comp;
                    }

            RootComponent.ChildComponents.AddRange(comps);

            //Link();
        }

        private class DelaunayTriVertex : IVertex
        {
            private SceneComponent _probe;

            public DelaunayTriVertex(Vec3 point)
            {
  
            }

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
            }

            public double[] Position { get; set; }

            public override string ToString()
            {
                return _probe.WorldPoint.ToString();
            }
        }
        private void Link()
        {
            List<DelaunayTriVertex> points = RootComponent.ChildComponents.Select(x => new DelaunayTriVertex(x)).ToList();
            var tri = Triangulation.CreateDelaunay<DelaunayTriVertex, DefaultTriangulationCell<DelaunayTriVertex>>(points);
            foreach (var cell in tri.Cells)
            {
                
            }
        }

        public override void OnSpawnedPreComponentSetup()
        {
            //SetFrequencies(ProbeBounds, ProbesPerMeter);
            base.OnSpawnedPreComponentSetup();
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
            {
                comp.SetCaptureResolution(colorResolution, captureDepth, depthResolution);
                comp.Capture();
                comp.GenerateIrradianceMap();
                comp.GeneratePrefilterMap();
            }
        }
    }
}
