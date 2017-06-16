using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds.Actors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Worlds.Actors.Types
{
    public enum TransformType
    {
        Scale,
        Rotate,
        Translate
    }
    public class EditorTransformTool3D : Actor<SkeletalMeshComponent>, IRenderable
    {
        public EditorTransformTool3D(SceneComponent modified)
        {
            _modified = modified;
        }
        protected override SkeletalMeshComponent OnConstruct()
        {
            Bone root = new Bone("Root")
            {
                ScaleByDistance = true
            };
            Bone screen = new Bone("Screen")
            {
                BillboardType = BillboardType.PerspectiveXY
            };
            root.ChildBones.Add(screen);
            Skeleton skel = new Skeleton(root);
            SkeletalMesh mesh = new SkeletalMesh("TransformTool");
            mesh.RigidChildren.Add(new SkeletalRigidSubMesh(Segment.Mesh(Vec3.Zero, Vec3.UnitX), Material.GetUnlitColorMaterial(Color.Red), "XAxis"));
            mesh.RigidChildren.Add(new SkeletalRigidSubMesh(Segment.Mesh(Vec3.Zero, Vec3.UnitY), Material.GetUnlitColorMaterial(Color.Green), "YAxis"));
            mesh.RigidChildren.Add(new SkeletalRigidSubMesh(Segment.Mesh(Vec3.Zero, Vec3.UnitZ), Material.GetUnlitColorMaterial(Color.Blue), "ZAxis"));
            mesh.RigidChildren.Add(new SkeletalRigidSubMesh(Circle3D.WireframeMesh(1.0f, Vec3.UnitX, Vec3.Zero, 30), Material.GetUnlitColorMaterial(Color.Red), "XRotation"));
            mesh.RigidChildren.Add(new SkeletalRigidSubMesh(Circle3D.WireframeMesh(1.0f, Vec3.UnitY, Vec3.Zero, 30), Material.GetUnlitColorMaterial(Color.Green), "YRotation"));
            mesh.RigidChildren.Add(new SkeletalRigidSubMesh(Circle3D.WireframeMesh(1.0f, Vec3.UnitZ, Vec3.Zero, 30), Material.GetUnlitColorMaterial(Color.Blue), "ZRotation"));
            mesh.RigidChildren.Add(new SkeletalRigidSubMesh(Circle3D.WireframeMesh(1.0f, Vec3.UnitZ, Vec3.Zero, 30), Material.GetUnlitColorMaterial(Color.Gray), "ScreenRotation"));
            mesh.RigidChildren.Add(new SkeletalRigidSubMesh(Circle3D.WireframeMesh(1.0f, Vec3.UnitZ, Vec3.Zero, 30), Material.GetUnlitColorMaterial(Color.Gray), "ScreenTranslation"));
            SkeletalMeshComponent meshComp = new SkeletalMeshComponent(mesh, null);
            meshComp.WorldTransformChanged += _transform_WorldTransformChanged;
            return meshComp;
        }
        
        private void _transform_WorldTransformChanged()
        {
            _transformChanged = true;
        }

        private bool _transformChanged = false;
        private TransformType _mode = TransformType.Translate;
        private ISocket _modified = null;
        private TRSComponent _transform;

        public TransformType Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }
        public ISocket ModifiedComponent
        {
            get { return _modified; }
            set
            {
                _modified = value;

            }
        }

        private Shape _cullingVolume;
        private IOctreeNode _renderNode;
        private bool _isRendering;

        public Shape CullingVolume => _cullingVolume;

        public IOctreeNode RenderNode
        {
            get => _renderNode;
            set => _renderNode = value;
        }
        public bool IsRendering
        {
            get => _isRendering;
            set => _isRendering = value;
        }

        public bool HasTransparency => false;

        public override void OnSpawned(World world)
        {
            base.OnSpawned(world);
            CurrentInstance = this;
        }

        public override void OnDespawned()
        {
            base.OnDespawned();
            CurrentInstance = null;
        }

        public static EditorTransformTool3D CurrentInstance;

        private bool _hiX, _hiY, _hiZ, _hiCirc, _hiSphere;
        private const float _orbRadius = 1.0f;
        private const float _circRadius = 1.2f;
        private const float _axisSnapRange = 7.0f;
        private const float _selectRange = 0.03f; //Selection error range for orb and circ
        private const float _axisSelectRange = 0.15f; //Selection error range for axes
        private const float _selectOrbScale = _selectRange / _orbRadius;
        private const float _circOrbScale = _circRadius / _orbRadius;
        private const float _axisLDist = 2.0f;
        private const float _axisHalfLDist = 0.75f;
        private const float _apthm = 0.075f;
        private const float _dst = 1.5f;
        private const float _scaleHalf1LDist = 0.8f;
        private const float _scaleHalf2LDist = 1.2f;

        public bool UpdateCursorRay(Ray cursor, Camera camera, bool pressed)
        {
            bool clamp = true, snapFound = false;
            Ray localRay = cursor.TransformedBy(_transform.InverseWorldMatrix);
            float radius = camera.DistanceScale(_transform.Translation, 1.0f);
            if (_mode == TransformType.Rotate)
            {
                if (!localRay.LineSphereIntersect(Vec3.Zero, radius, out Vec3 point))
                {
                    //If no intersect is found, project the ray through the plane perpendicular to the camera.
                    localRay.LinePlaneIntersect(Vec3.Zero, camera.WorldPoint.Normalized(_transform.Translation), out point);

                    //Clamp the point to edge of the sphere
                    if (clamp)
                        point = Ray.PointAtLineDistance(_transform.Translation, point, radius);
                }

                float distance = point.LengthFast;

                //Point lies within orb radius?
                if (Math.Abs(distance - radius) < (radius * _selectOrbScale))
                {
                    _hiSphere = true;

                    //Determine axis snapping
                    Vec3 angles = CustomMath.RadToDeg(point.GetAngles());
                    angles.X = Math.Abs(angles.X);
                    angles.Y = Math.Abs(angles.Y);
                    angles.Z = Math.Abs(angles.Z);

                    if (Math.Abs(angles.Y - 90.0f) <= _axisSnapRange)
                        _hiX = true;
                    else if (angles.X >= (180.0f - _axisSnapRange) || angles.X <= _axisSnapRange)
                        _hiY = true;
                    else if (angles.Y >= (180.0f - _axisSnapRange) || angles.Y <= _axisSnapRange)
                        _hiZ = true;
                }
                //Point lies on circ line?
                else if (Math.Abs(distance - (radius * _circOrbScale)) < (radius * _selectOrbScale))
                    _hiCirc = true;

                if (_hiX || _hiY || _hiZ || _hiCirc)
                    snapFound = true;
            }
            else
            {
                Plane yz = new Plane(Vec3.Zero, localRay.StartPoint.X < 0.0f ? -Vec3.Right  : Vec3.Right);
                Plane xz = new Plane(Vec3.Zero, localRay.StartPoint.Y < 0.0f ? -Vec3.Up     : Vec3.Up);
                Plane xy = new Plane(Vec3.Zero, localRay.StartPoint.Z < 0.0f ? Vec3.Forward : -Vec3.Forward);

                //if (Collision.PlaneIntersectsPoint(_xz, cursor.StartPoint) == EPlaneIntersection.Back)

                Vec3[] intersectionPoints = new Vec3[3];
                bool[] intersects = new bool[3]
                {
                    localRay.LinePlaneIntersect(yz, out intersectionPoints[0]),
                    localRay.LinePlaneIntersect(xz, out intersectionPoints[1]),
                    localRay.LinePlaneIntersect(xy, out intersectionPoints[2]),
                };
                List<Vec3> testDiffs = new List<Vec3>();
                for (int i = 0; i < 3; ++i)
                {
                    if (!intersects[i])
                        continue;
                    Vec3 diff = intersectionPoints[i] / radius;
                    if (diff.X > -_axisSelectRange && diff.X < (_axisLDist + 0.01f) &&
                        diff.Y > -_axisSelectRange && diff.Y < (_axisLDist + 0.01f) &&
                        diff.Z > -_axisSelectRange && diff.Z < (_axisLDist + 0.01f))
                        testDiffs.Add(diff);
                }

                //Check if point lies on a specific axis
                foreach (Vec3 diff in testDiffs)
                {
                    float errorRange = _axisSelectRange;

                    if (diff.X > _axisHalfLDist &&
                        Math.Abs(diff.Y) < errorRange &&
                        Math.Abs(diff.Z) < errorRange)
                        _hiX = true;
                    if (diff.Y > _axisHalfLDist &&
                        Math.Abs(diff.X) < errorRange &&
                        Math.Abs(diff.Z) < errorRange)
                        _hiY = true;
                    if (diff.Z > _axisHalfLDist &&
                        Math.Abs(diff.X) < errorRange &&
                        Math.Abs(diff.Y) < errorRange)
                        _hiZ = true;

                    if (snapFound = _hiX || _hiY || _hiZ)
                        break;
                }

                if (!snapFound)
                {
                    foreach (Vec3 diff in testDiffs)
                    {
                        if (_mode == TransformType.Translate)
                        {
                            if (diff.X < _axisHalfLDist &&
                                diff.Y < _axisHalfLDist &&
                                diff.Z < _axisHalfLDist)
                            {
                                //Point lies inside the double drag areas
                                if (diff.X > _axisSelectRange)
                                    _hiX = true;
                                if (diff.Y > _axisSelectRange)
                                    _hiY = true;
                                if (diff.Z > _axisSelectRange)
                                    _hiZ = true;

                                _hiCirc = !_hiX && !_hiY && !_hiZ;

                                snapFound = true;
                            }
                        }
                        else if (_mode == TransformType.Scale)
                        {
                            //Determine if the point is in the double or triple drag triangles
                            float halfDist = _scaleHalf2LDist;
                            float centerDist = _scaleHalf1LDist;
                            if (diff.IsInTriangle(new Vec3(), new Vec3(halfDist, 0, 0), new Vec3(0, halfDist, 0)))
                                if (diff.IsInTriangle(new Vec3(), new Vec3(centerDist, 0, 0), new Vec3(0, centerDist, 0)))
                                    _hiX = _hiY = _hiZ = true;
                                else
                                    _hiX = _hiY = true;
                            else if (diff.IsInTriangle(new Vec3(), new Vec3(halfDist, 0, 0), new Vec3(0, 0, halfDist)))
                                if (diff.IsInTriangle(new Vec3(), new Vec3(centerDist, 0, 0), new Vec3(0, 0, centerDist)))
                                    _hiX = _hiY = _hiZ = true;
                                else
                                    _hiX = _hiZ = true;
                            else if (diff.IsInTriangle(new Vec3(), new Vec3(0, halfDist, 0), new Vec3(0, 0, halfDist)))
                                if (diff.IsInTriangle(new Vec3(), new Vec3(0, centerDist, 0), new Vec3(0, 0, centerDist)))
                                    _hiX = _hiY = _hiZ = true;
                                else
                                    _hiY = _hiZ = true;

                            snapFound = _hiX || _hiY || _hiZ;
                        }

                        if (snapFound)
                            break;
                    }
                }
            }

            return false;
        }

        public void Render()
        {
            switch (_mode)
            {
                case TransformType.Translate:

                    break;
                case TransformType.Rotate:

                    break;
                case TransformType.Scale:

                    break;
            }
        }
    }
}
