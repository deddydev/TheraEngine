using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Worlds.Actors.Types
{
    public enum ESpace
    {
        World,
        Parent,
        Local,
        Screen,
    }
    public enum TransformType
    {
        Scale,
        Rotate,
        Translate,
        DragDrop,
    }
    public class EditorTransformTool3D : Actor<SkeletalMeshComponent>, I3DRenderable
    {
        public static EditorTransformTool3D Instance => _currentInstance;
        private static EditorTransformTool3D _currentInstance;

        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(Rendering.ERenderPassType3D.OnTopForward, null);
        public Shape CullingVolume => null;
        public IOctreeNode OctreeNode { get; set; }

        public EditorTransformTool3D() : base() { }

        private Material[] _axisMat = new Material[3];
        private Material[] _transPlaneMat = new Material[6];
        private Material[] _scalePlaneMat = new Material[3];
        private Material _screenMat;
        
        private ESpace _transformSpace = ESpace.World;

        protected override SkeletalMeshComponent OnConstruct()
        {
            #region Mesh Generation

            SkeletalMesh mesh = new SkeletalMesh("TransformTool");

            //Skeleton
            string rootBoneName = "Root"; //Screen-scaled bone
            string screenBoneName = "Screen"; //Screen-aligned child bone
            Bone root = new Bone(rootBoneName)
            {
                ScaleByDistance = true,
                DistanceScaleScreenSize = _orbRadius,
            };
            Bone screen = new Bone(screenBoneName)
            {
                BillboardType = BillboardType.RotationXYZ
            };
            root.ChildBones.Add(screen);
            Skeleton skel = new Skeleton(root);
            
            _screenMat = Material.GetUnlitColorMaterialForward(Color.LightGray);
            _screenMat.RenderParams.DepthTest.Enabled = false;
            _screenMat.RenderParams.LineWidth = 2.0f;

            for (int normalAxis = 0; normalAxis < 3; ++normalAxis)
            {
                int planeAxis1 = normalAxis + 1 - (normalAxis >> 1) * 3; //0 = 1, 1 = 2, 2 = 0
                int planeAxis2 = planeAxis1 + 1 - (normalAxis  & 1) * 3; //0 = 2, 1 = 0, 2 = 1

                Vec3 unit = Vec3.Zero;
                unit[normalAxis] = 1.0f;

                Vec3 unit1 = Vec3.Zero;
                unit1[planeAxis1] = 1.0f;

                Vec3 unit2 = Vec3.Zero;
                unit2[planeAxis2] = 1.0f;

                Material axisMat = Material.GetUnlitColorMaterialForward(unit);
                axisMat.RenderParams.DepthTest.Enabled = false;
                axisMat.RenderParams.LineWidth = 2.0f;
                _axisMat[normalAxis] = axisMat;

                Material planeMat1 = Material.GetUnlitColorMaterialForward(unit1);
                planeMat1.RenderParams.DepthTest.Enabled = false;
                planeMat1.RenderParams.LineWidth = 2.0f;
                _transPlaneMat[(normalAxis << 1) + 0] = planeMat1;

                Material planeMat2 = Material.GetUnlitColorMaterialForward(unit2);
                planeMat2.RenderParams.DepthTest.Enabled = false;
                planeMat2.RenderParams.LineWidth = 2.0f;
                _transPlaneMat[(normalAxis << 1) + 1] = planeMat2;
                
                Material scalePlaneMat = Material.GetUnlitColorMaterialForward(unit);
                scalePlaneMat.RenderParams.DepthTest.Enabled = false;
                scalePlaneMat.RenderParams.LineWidth = 2.0f;
                _scalePlaneMat[normalAxis] = scalePlaneMat;

                VertexLine axisLine = new VertexLine(Vec3.Zero, unit * _axisLength);
                Vec3 halfUnit = unit * _axisHalfLength;

                VertexLine transLine1 = new VertexLine(halfUnit, halfUnit + unit1 * _axisHalfLength);
                transLine1.Vertex0._color = unit1;
                transLine1.Vertex1._color = unit1;
                VertexLine transLine2 = new VertexLine(halfUnit, halfUnit + unit2 * _axisHalfLength);
                transLine2.Vertex0._color = unit2;
                transLine2.Vertex1._color = unit2;

                VertexLine scaleLine1 = new VertexLine(unit1 * _scaleHalf1LDist, unit2 * _scaleHalf1LDist);
                scaleLine1.Vertex0._color = unit;
                scaleLine1.Vertex1._color = unit;
                VertexLine scaleLine2 = new VertexLine(unit1 * _scaleHalf2LDist, unit2 * _scaleHalf2LDist);
                scaleLine2.Vertex0._color = unit;
                scaleLine2.Vertex1._color = unit;

                string axis = ((char)('X' + normalAxis)).ToString();

                PrimitiveData axisPrim = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), axisLine);
                axisPrim.SingleBindBone = rootBoneName;
                mesh.RigidChildren.Add(new SkeletalRigidSubMesh(axis + "Axis", axisPrim, axisMat, true)
                {
                    RenderInfo = new RenderInfo3D(Rendering.ERenderPassType3D.OnTopForward, null, false, false),
                    VisibleByDefault = TransformMode != TransformType.Rotate
                });

                float coneHeight = _axisLength - _coneDistance;
                PrimitiveData arrowPrim = BaseCone.SolidMesh(unit * (_coneDistance + coneHeight / 2.0f), unit, coneHeight, _coneRadius, 6, false);
                arrowPrim.SingleBindBone = rootBoneName;
                mesh.RigidChildren.Add(new SkeletalRigidSubMesh(axis + "Arrow", arrowPrim, axisMat, true)
                {
                    RenderInfo = new RenderInfo3D(Rendering.ERenderPassType3D.OnTopForward, null, false, false),
                    VisibleByDefault = TransformMode != TransformType.Rotate
                });
                
                PrimitiveData transPrim1 = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), transLine1);
                transPrim1.SingleBindBone = rootBoneName;
                mesh.RigidChildren.Add(new SkeletalRigidSubMesh(axis + "TransPlane1", transPrim1, planeMat1, true)
                {
                    RenderInfo = new RenderInfo3D(Rendering.ERenderPassType3D.OnTopForward, null, false, false),
                    VisibleByDefault = TransformMode == TransformType.Translate
                });

                PrimitiveData transPrim2 = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), transLine2);
                transPrim2.SingleBindBone = rootBoneName;
                mesh.RigidChildren.Add(new SkeletalRigidSubMesh(axis + "TransPlane2", transPrim2, planeMat2, true)
                {
                    RenderInfo = new RenderInfo3D(Rendering.ERenderPassType3D.OnTopForward, null, false, false),
                    VisibleByDefault = TransformMode == TransformType.Translate
                });

                PrimitiveData scalePrim = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), scaleLine1, scaleLine2);
                scalePrim.SingleBindBone = rootBoneName;
                mesh.RigidChildren.Add(new SkeletalRigidSubMesh(axis + "ScalePlane", scalePrim, scalePlaneMat, true)
                {
                    RenderInfo = new RenderInfo3D(Rendering.ERenderPassType3D.OnTopForward, null, false, false),
                    VisibleByDefault = TransformMode == TransformType.Scale
                });

                PrimitiveData rotPrim = Circle3D.WireframeMesh(_orbRadius, unit, Vec3.Zero, _circlePrecision);
                rotPrim.SingleBindBone = rootBoneName;
                mesh.RigidChildren.Add(new SkeletalRigidSubMesh(axis + "Rotation", rotPrim, axisMat, true)
                {
                    RenderInfo = new RenderInfo3D(Rendering.ERenderPassType3D.OnTopForward, null, false, false),
                    VisibleByDefault = TransformMode == TransformType.Rotate
                });
            }

            //Screen-aligned rotation
            PrimitiveData screenRotPrim = Circle3D.WireframeMesh(_circRadius, Vec3.UnitZ, Vec3.Zero, _circlePrecision);
            screenRotPrim.SingleBindBone = screenBoneName;

            mesh.RigidChildren.Add(new SkeletalRigidSubMesh("ScreenRotation", screenRotPrim, _screenMat, true)
            {
                RenderInfo = new RenderInfo3D(Rendering.ERenderPassType3D.OnTopForward, null, false, false),
                VisibleByDefault = TransformMode == TransformType.Rotate
            });

            //Screen-aligned translation
            Vertex v1 = new Vec3(-_screenTransExtent, -_screenTransExtent, 0.0f);
            Vertex v2 = new Vec3(_screenTransExtent, -_screenTransExtent, 0.0f);
            Vertex v3 = new Vec3(_screenTransExtent, _screenTransExtent, 0.0f);
            Vertex v4 = new Vec3(-_screenTransExtent, _screenTransExtent, 0.0f);
            VertexLineStrip strip = new VertexLineStrip(true, v1, v2, v3, v4);
            PrimitiveData screenTransPrim = PrimitiveData.FromLineStrips(VertexShaderDesc.JustPositions(), strip);
            screenTransPrim.SingleBindBone = screenBoneName;

            mesh.RigidChildren.Add(new SkeletalRigidSubMesh("ScreenTranslation", screenTransPrim, _screenMat, true)
            {
                RenderInfo = new RenderInfo3D(Rendering.ERenderPassType3D.OnTopForward, null, false, false),
                VisibleByDefault = TransformMode == TransformType.Translate
            });
            
            return new SkeletalMeshComponent(mesh, skel);

            #endregion
        }
        
        private TransformType _mode = TransformType.Translate;
        private ISocket _targetSocket = null;

        public TransformType TransformMode
        {
            get => _mode;
            set
            {
                _mode = value;
                switch (_mode)
                {
                    case TransformType.Rotate:
                        _highlight = HighlightRotation;
                        _drag = DragRotation;
                        break;
                    case TransformType.Translate:
                        _highlight = HighlightTranslation;
                        _drag = DragTranslation;
                        break;
                    case TransformType.Scale:
                        _highlight = HighlightScale;
                        _drag = DragScale;
                        break;
                }
                int x = 0;
                for (int i = 0; i < 3; ++i)
                {
                    RootComponent.Meshes[x++].Visible = _mode != TransformType.Rotate;
                    RootComponent.Meshes[x++].Visible = _mode != TransformType.Rotate;
                    RootComponent.Meshes[x++].Visible = _mode == TransformType.Translate;
                    RootComponent.Meshes[x++].Visible = _mode == TransformType.Translate;
                    RootComponent.Meshes[x++].Visible = _mode == TransformType.Scale;
                    RootComponent.Meshes[x++].Visible = _mode == TransformType.Rotate;
                }
                RootComponent.Meshes[x++].Visible = _mode == TransformType.Rotate;
                RootComponent.Meshes[x++].Visible = _mode == TransformType.Translate;

                GetDependentColors();
            }
        }
        /// <summary>
        /// The socket transform that is being manipulated by this transform tool.
        /// </summary>
        public ISocket TargetSocket
        {
            get => _targetSocket;
            set
            {
#if EDITOR
                if (_targetSocket != null)
                    _targetSocket.Selected = false;
#endif
                _targetSocket = value;
                if (_targetSocket != null)
                {
#if EDITOR
                    _targetSocket.Selected = true;
#endif
                    RootComponent.WorldMatrix = GetWorldMatrix();
                }
                else
                    RootComponent.WorldMatrix = Matrix4.Identity;
                _dragMatrix = RootComponent.WorldMatrix;
                _invDragMatrix = RootComponent.InverseWorldMatrix;
            }
        }

        private Matrix4 GetWorldMatrix()
        {
            return _targetSocket.WorldMatrix.GetPoint().AsTranslationMatrix();
        }
        private Matrix4 GetInvWorldMatrix()
        {
            return _targetSocket.InverseWorldMatrix.GetPoint().AsTranslationMatrix();
        }
        private Matrix4 GetLocalMatrix()
        {
            return _targetSocket.WorldMatrix;
        }
        public static EditorTransformTool3D GetInstance(ISocket comp, TransformType transformType)
        {
            if (_currentInstance == null)
                _currentInstance = new EditorTransformTool3D();

            if (!_currentInstance.IsSpawned)
                Engine.World.SpawnActor(_currentInstance);

            _currentInstance.TargetSocket = comp;
            _currentInstance.TransformMode = transformType;

            return _currentInstance;
        }
        public static void DestroyInstance()
        {
            _currentInstance?.Despawn();
        }
        public override void OnSpawnedPreComponentSetup(World world)
        {
            //Engine.Scene.Add(this);
        }
        public override void OnDespawned()
        {
            //Engine.Scene.Remove(this);
            base.OnDespawned();
        }

        private BoolVec3 _hiAxis;
        private bool _hiCam, _hiSphere;
        private const int _circlePrecision = 20;
        private const float _orbRadius = 1.0f;
        private const float _circRadius = _orbRadius * _circOrbScale;
        private const float _screenTransExtent = _orbRadius * 0.1f;
        private const float _axisSnapRange = 7.0f;
        private const float _selectRange = 0.03f; //Selection error range for orb and circ
        private const float _axisSelectRange = 0.1f; //Selection error range for axes
        private const float _selectOrbScale = _selectRange / _orbRadius;
        private const float _circOrbScale = 1.2f;
        private const float _axisLength = _orbRadius * 2.0f;
        private const float _axisHalfLength = _orbRadius * 0.75f;
        private const float _coneRadius = _orbRadius * 0.1f;
        private const float _coneDistance = _orbRadius * 1.5f;
        private const float _scaleHalf1LDist = _orbRadius * 0.8f;
        private const float _scaleHalf2LDist = _orbRadius * 1.2f;
        private List<Vec3> _intersectionPoints = new List<Vec3>(3);
        
        Vec3 _lastPoint;
        Vec3 _dragPlaneNormal;

        private DelDrag _drag;
        private DelHighlight _highlight;
        private delegate bool DelHighlight(Camera camera, Ray localRay);
        private delegate void DelDrag(Vec3 dragPoint);
        private delegate void DelDragRot(Quat dragPoint);

#region Drag
        private void DragRotation(Vec3 dragPoint)
        {
            Quat delta = Quat.BetweenVectors(_lastPoint, dragPoint);
            _targetSocket.HandleRotation(delta);
            RootComponent.WorldMatrix = GetWorldMatrix();
        }
        private void DragTranslation(Vec3 dragPoint)
        {
            Vec3 delta = dragPoint - _lastPoint;
            _targetSocket.HandleTranslation(delta);
            RootComponent.WorldMatrix = GetWorldMatrix();
        }
        private void DragScale(Vec3 dragPoint)
        {
            Vec3 delta = dragPoint - _lastPoint;
            _targetSocket.HandleScale(delta);
            RootComponent.WorldMatrix = GetWorldMatrix();
        }
        /// <summary>
        /// Returns a point relative to the local space of the target socket (origin at 0,0,0), clamped to the highlighted drag plane.
        /// </summary>
        /// <param name="camera">The camera viewing this tool, used for camera space drag clamping.</param>
        /// <param name="localRay">The mouse ray, transformed into the socket's local space.</param>
        /// <returns></returns>
        private Vec3 GetDragPoint(Camera camera, Ray localRay)
        {
            //Convert all coordinates to local space

            Vec3 localCamPoint = camera.WorldPoint * _invDragMatrix;
            Vec3 dragPoint, unit;
            if (_hiCam)
            {
                _dragPlaneNormal = localCamPoint;
                _dragPlaneNormal.NormalizeFast();
            }
            else if (_hiAxis.X)
            {
                if (_hiAxis.Y)
                {
                    _dragPlaneNormal = Vec3.UnitZ;
                }
                else if (_hiAxis.Z)
                {
                    _dragPlaneNormal = Vec3.UnitY;
                }
                else
                {
                    unit = Vec3.UnitX;
                    Vec3 perpPoint = Ray.GetClosestColinearPoint(Vec3.Zero, unit, localCamPoint);
                    _dragPlaneNormal = localCamPoint - perpPoint;
                    _dragPlaneNormal.NormalizeFast();

                    if (!Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, _dragPlaneNormal, out dragPoint))
                        return _lastPoint;

                    return Ray.GetClosestColinearPoint(Vec3.Zero, unit, dragPoint);
                }
            }
            else if (_hiAxis.Y)
            {
                if (_hiAxis.X)
                {
                    _dragPlaneNormal = Vec3.UnitZ;
                }
                else if (_hiAxis.Z)
                {
                    _dragPlaneNormal = Vec3.UnitX;
                }
                else
                {
                    unit = Vec3.UnitY;
                    Vec3 perpPoint = Ray.GetClosestColinearPoint(Vec3.Zero, unit, localCamPoint);
                    _dragPlaneNormal = localCamPoint - perpPoint;
                    _dragPlaneNormal.NormalizeFast();

                    if (!Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, _dragPlaneNormal, out dragPoint))
                        return _lastPoint;

                    return Ray.GetClosestColinearPoint(Vec3.Zero, unit, dragPoint);
                }
            }
            else if (_hiAxis.Z)
            {
                if (_hiAxis.X)
                {
                    _dragPlaneNormal = Vec3.UnitY;
                }
                else if (_hiAxis.Y)
                {
                    _dragPlaneNormal = Vec3.UnitX;
                }
                else
                {
                    unit = Vec3.UnitZ;
                    Vec3 perpPoint = Ray.GetClosestColinearPoint(Vec3.Zero, unit, localCamPoint);
                    _dragPlaneNormal = localCamPoint - perpPoint;
                    _dragPlaneNormal.NormalizeFast();

                    if (!Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, _dragPlaneNormal, out dragPoint))
                        return _lastPoint;

                    return Ray.GetClosestColinearPoint(Vec3.Zero, unit, dragPoint);
                }
            }

            if (Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, _dragPlaneNormal, out dragPoint))
                return dragPoint;

            return _lastPoint;
        }
#endregion

#region Highlighting
        private bool HighlightRotation(Camera camera, Ray localRay)
        {
            Vec3 worldPoint = _dragMatrix.GetPoint();
            float radius = camera.DistanceScale(worldPoint, _orbRadius);

            if (!Collision.RayIntersectsSphere(localRay.StartPoint, localRay.Direction, Vec3.Zero, radius, out Vec3 point))
            {
                //If no intersect is found, project the ray through the plane perpendicular to the camera.
                //localRay.LinePlaneIntersect(Vec3.Zero, (camera.WorldPoint - worldPoint).Normalized(), out point);
                Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, (camera.WorldPoint - worldPoint) * _invDragMatrix, out point);
                
                //Clamp the point to edge of the sphere
                //if (clamp)
                //    point = Ray.PointAtLineDistance(Vec3.Zero, point, radius);
            }

            float distance = point.LengthFast;

            //Point lies within orb radius?
            if (Math.Abs(distance - radius) < radius * _selectOrbScale)
            {
                _hiSphere = true;

                //Determine axis snapping
                Vec3 angles = point.GetAngles();
                angles.X = Math.Abs(angles.X);
                angles.Y = Math.Abs(angles.Y);
                angles.Z = Math.Abs(angles.Z);

                if (Math.Abs(angles.Y - 90.0f) <= _axisSnapRange)
                    _hiAxis.X = true;
                else if (angles.X >= (180.0f - _axisSnapRange) || angles.X <= _axisSnapRange)
                    _hiAxis.Y = true;
                else if (angles.Y >= (180.0f - _axisSnapRange) || angles.Y <= _axisSnapRange)
                    _hiAxis.Z = true;
            }
            //Point lies on circ line?
            else if (Math.Abs(distance - radius * _circOrbScale) < radius * _selectOrbScale)
                _hiCam = true;

            return _hiAxis.Any || _hiCam || _hiSphere;
        }
        private bool HighlightTranslation(Camera camera, Ray localRay)
        {
            Vec3 worldPoint = _dragMatrix.GetPoint();
            float radius = camera.DistanceScale(worldPoint, _orbRadius);

            _intersectionPoints.Clear();

            bool snapFound = false;
            for (int normalAxis = 0; normalAxis < 3; ++normalAxis)
            {
                Vec3 unit = Vec3.Zero;
                unit[normalAxis] = localRay.StartPoint[normalAxis] < 0.0f ? -1.0f : 1.0f;

                //Get plane intersection point for cursor ray and each drag plane
                if (Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, unit, out Vec3 point))
                    _intersectionPoints.Add(point);
            }

            //_intersectionPoints.Sort((l, r) => l.DistanceToSquared(camera.WorldPoint).CompareTo(r.DistanceToSquared(camera.WorldPoint)));

            foreach (Vec3 v in _intersectionPoints)
            {
                Vec3 diff = v / radius;
                //int planeAxis1 = normalAxis + 1 - (normalAxis >> 1) * 3;    //0 = 1, 1 = 2, 2 = 0
                //int planeAxis2 = planeAxis1 + 1 - (normalAxis  & 1) * 3;    //0 = 2, 1 = 0, 2 = 1

                if (diff.X > -_axisSelectRange && diff.X <= _axisLength &&
                    diff.Y > -_axisSelectRange && diff.Y <= _axisLength &&
                    diff.Z > -_axisSelectRange && diff.Z <= _axisLength)
                {
                    float errorRange = _axisSelectRange;

                    _hiAxis.X = diff.X > _axisHalfLength && Math.Abs(diff.Y) < errorRange && Math.Abs(diff.Z) < errorRange;
                    _hiAxis.Y = diff.Y > _axisHalfLength && Math.Abs(diff.X) < errorRange && Math.Abs(diff.Z) < errorRange;
                    _hiAxis.Z = diff.Z > _axisHalfLength && Math.Abs(diff.X) < errorRange && Math.Abs(diff.Y) < errorRange;

                    if (snapFound = _hiAxis.Any)
                        break;

                    if (diff.X < _axisHalfLength &&
                        diff.Y < _axisHalfLength &&
                        diff.Z < _axisHalfLength)
                    {
                        //Point lies inside the double drag areas
                        _hiAxis.X = diff.X > _axisSelectRange;
                        _hiAxis.Y = diff.Y > _axisSelectRange;
                        _hiAxis.Z = diff.Z > _axisSelectRange;
                        _hiCam = _hiAxis.None;

                        snapFound = true;
                        break;
                    }
                }
            }

            return snapFound;
        }
        private bool HighlightScale(Camera camera, Ray localRay)
        {
            Vec3 worldPoint = _dragMatrix.GetPoint();
            float radius = camera.DistanceScale(worldPoint, _orbRadius);

            _intersectionPoints.Clear();

            bool snapFound = false;
            for (int normalAxis = 0; normalAxis < 3; ++normalAxis)
            {
                Vec3 unit = Vec3.Zero;
                unit[normalAxis] = localRay.StartPoint[normalAxis] < 0.0f ? -1.0f : 1.0f;
                
                //Get plane intersection point for cursor ray and each drag plane
                if (Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, unit, out Vec3 point))
                    _intersectionPoints.Add(point);
            }

            //_intersectionPoints.Sort((l, r) => l.DistanceToSquared(camera.WorldPoint).CompareTo(r.DistanceToSquared(camera.WorldPoint)));
            
            foreach (Vec3 v in _intersectionPoints)
            {
                Vec3 diff = v / radius;
                //int planeAxis1 = normalAxis + 1 - (normalAxis >> 1) * 3;    //0 = 1, 1 = 2, 2 = 0
                //int planeAxis2 = planeAxis1 + 1 - (normalAxis  & 1) * 3;    //0 = 2, 1 = 0, 2 = 1

                if (diff.X > -_axisSelectRange && diff.X <= _axisLength &&
                    diff.Y > -_axisSelectRange && diff.Y <= _axisLength &&
                    diff.Z > -_axisSelectRange && diff.Z <= _axisLength)
                {
                    float errorRange = _axisSelectRange;
                    
                    _hiAxis.X = diff.X > _axisHalfLength && Math.Abs(diff.Y) < errorRange && Math.Abs(diff.Z) < errorRange;
                    _hiAxis.Y = diff.Y > _axisHalfLength && Math.Abs(diff.X) < errorRange && Math.Abs(diff.Z) < errorRange;
                    _hiAxis.Z = diff.Z > _axisHalfLength && Math.Abs(diff.X) < errorRange && Math.Abs(diff.Y) < errorRange;

                    if (snapFound = _hiAxis.Any)
                        break;

                    //Determine if the point is in the double or triple drag triangles
                    float halfDist = _scaleHalf2LDist;
                    float centerDist = _scaleHalf1LDist;
                    if (diff.IsInTriangle(new Vec3(), new Vec3(halfDist, 0, 0), new Vec3(0, halfDist, 0)))
                        if (diff.IsInTriangle(new Vec3(), new Vec3(centerDist, 0, 0), new Vec3(0, centerDist, 0)))
                            _hiAxis.X = _hiAxis.Y = _hiAxis.Z = true;
                        else
                            _hiAxis.X = _hiAxis.Y = true;
                    else if (diff.IsInTriangle(new Vec3(), new Vec3(halfDist, 0, 0), new Vec3(0, 0, halfDist)))
                        if (diff.IsInTriangle(new Vec3(), new Vec3(centerDist, 0, 0), new Vec3(0, 0, centerDist)))
                            _hiAxis.X = _hiAxis.Y = _hiAxis.Z = true;
                        else
                            _hiAxis.X = _hiAxis.Y = true;
                    else if (diff.IsInTriangle(new Vec3(), new Vec3(0, halfDist, 0), new Vec3(0, 0, halfDist)))
                        if (diff.IsInTriangle(new Vec3(), new Vec3(0, centerDist, 0), new Vec3(0, 0, centerDist)))
                            _hiAxis.X = _hiAxis.Y = _hiAxis.Z = true;
                        else
                            _hiAxis.Y = _hiAxis.Z = true;

                    snapFound = _hiAxis.Any;
                    
                    if (snapFound)
                        break;
                }
            }

            return snapFound;
        }
#endregion

        private bool _pressed = false;
        private Matrix4 _dragMatrix, _invDragMatrix;

        /// <summary>
        /// Returns true if intersecting one of the transform tool's various parts.
        /// </summary>
        public bool MouseMove(Ray cursor, Camera camera, bool pressed)
        {
            bool snapFound = true;
            if (pressed)
            {
                if (_hiAxis.None && !_hiCam && !_hiSphere)
                    return false;

                if (!_pressed)
                    OnPressed();

                Ray localRay = cursor.TransformedBy(_invDragMatrix);
                Vec3 dragPoint = GetDragPoint(camera, localRay);
                _drag(dragPoint);

                _lastPoint = dragPoint;
            }
            else
            {
                if (_pressed)
                    OnReleased();

                Ray localRay = cursor.TransformedBy(_invDragMatrix);

                _hiAxis.X = _hiAxis.Y = _hiAxis.Z = false;
                _hiCam = _hiSphere = false;

                snapFound = _highlight(camera, localRay);

                _axisMat[0].Parameter<ShaderVec4>(0).Value = _hiAxis.X ? (ColorF4)Color.Yellow : Color.Red;
                _axisMat[1].Parameter<ShaderVec4>(0).Value = _hiAxis.Y ? (ColorF4)Color.Yellow : Color.Green;
                _axisMat[2].Parameter<ShaderVec4>(0).Value = _hiAxis.Z ? (ColorF4)Color.Yellow : Color.Blue;
                _screenMat.Parameter<ShaderVec4>(0).Value = _hiCam ? (ColorF4)Color.Yellow : Color.LightGray;

                GetDependentColors();

                _lastPoint = GetDragPoint(camera, localRay);
            }
            return snapFound;
        }
        private void GetDependentColors()
        {
            if (TransformMode != TransformType.Rotate)
            {
                if (TransformMode == TransformType.Translate)
                {
                    _transPlaneMat[0].Parameter<ShaderVec4>(0).Value = _hiAxis.X && _hiAxis.Y ? (ColorF4)Color.Yellow : Color.Red;
                    _transPlaneMat[1].Parameter<ShaderVec4>(0).Value = _hiAxis.X && _hiAxis.Z ? (ColorF4)Color.Yellow : Color.Red;
                    _transPlaneMat[2].Parameter<ShaderVec4>(0).Value = _hiAxis.Y && _hiAxis.Z ? (ColorF4)Color.Yellow : Color.Green;
                    _transPlaneMat[3].Parameter<ShaderVec4>(0).Value = _hiAxis.Y && _hiAxis.X ? (ColorF4)Color.Yellow : Color.Green;
                    _transPlaneMat[4].Parameter<ShaderVec4>(0).Value = _hiAxis.Z && _hiAxis.X ? (ColorF4)Color.Yellow : Color.Blue;
                    _transPlaneMat[5].Parameter<ShaderVec4>(0).Value = _hiAxis.Z && _hiAxis.Y ? (ColorF4)Color.Yellow : Color.Blue;
                }
                else
                {
                    _scalePlaneMat[0].Parameter<ShaderVec4>(0).Value = _hiAxis.Y && _hiAxis.Z ? (ColorF4)Color.Yellow : Color.Red;
                    _scalePlaneMat[1].Parameter<ShaderVec4>(0).Value = _hiAxis.X && _hiAxis.Z ? (ColorF4)Color.Yellow : Color.Green;
                    _scalePlaneMat[2].Parameter<ShaderVec4>(0).Value = _hiAxis.X && _hiAxis.Y ? (ColorF4)Color.Yellow : Color.Blue;
                }
            }
        }
        private void OnPressed()
        {
            _pressed = true;
            _dragMatrix = RootComponent.WorldMatrix;
            _invDragMatrix = RootComponent.InverseWorldMatrix;
        }
        private void OnReleased()
        {
            _pressed = false;
            _dragMatrix = RootComponent.WorldMatrix;
            _invDragMatrix = RootComponent.InverseWorldMatrix;
        }
        public void Render()
        {
            //foreach (Vec3 v in _intersectionPoints)
            //    Engine.Renderer.RenderPoint(v * _dragMatrix, Color.Orange);
            //Vec3 dragPoint = _dragMatrix.GetPoint();
            //Engine.Renderer.RenderLine(dragPoint, dragPoint + _dragPlaneNormal * 10.0f, Color.White);
            //Engine.Renderer.RenderPoint(_lastPoint * _dragMatrix, Color.Magenta);
        }
    }
}
