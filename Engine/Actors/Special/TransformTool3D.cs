using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds;

namespace TheraEngine.Actors.Types
{
    public enum ESpace
    {
        /// <summary>
        /// Relative to the world.
        /// </summary>
        World,
        /// <summary>
        /// Relative to the parent transform (or world if no parent).
        /// </summary>
        Parent,
        /// <summary>
        /// Relative to the current transform.
        /// </summary>
        Local,
        /// <summary>
        /// Relative to the camera transform.
        /// </summary>
        Screen,
    }
    public enum TransformType
    {
        Scale,
        Rotate,
        Translate,
        DragDrop,
    }
    public class TransformTool3D : Actor<SkeletalMeshComponent>, I3DRenderable
    {
        public static TransformTool3D Instance => _currentInstance.Value;
        private static readonly Lazy<TransformTool3D> _currentInstance = new Lazy<TransformTool3D>(() =>
        {
            var node = new TransformTool3D();
            node.EditorState.DisplayInActorTree = false;
            return node;
        });

        public IRenderInfo3D RenderInfo { get; } = new RenderInfo3D(true, true);

        public TransformTool3D() : base()
        {
            TransformSpace = ESpace.Local;
            _rc = new RenderCommandMethod3D(ERenderPass.OnTopForward, Render);
        }

        private readonly TMaterial[] _axisMat = new TMaterial[3];
        private readonly TMaterial[] _transPlaneMat = new TMaterial[6];
        private readonly TMaterial[] _scalePlaneMat = new TMaterial[3];
        private TMaterial _screenMat;
        
        private ESpace _transformSpace;

        protected override SkeletalMeshComponent OnConstructRoot()
        {
            #region Mesh Generation

            SkeletalModel mesh = new SkeletalModel("TransformTool");

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
                BillboardType = EBillboardType.OrthographicXYZ
            };
            root.ChildBones.Add(screen);
            Skeleton skel = new Skeleton(root);
            
            _screenMat = TMaterial.CreateUnlitColorMaterialForward(Color.LightGray);
            _screenMat.RenderParamsRef.File.DepthTest.Enabled = ERenderParamUsage.Disabled;
            _screenMat.RenderParamsRef.File.LineWidth = 1.0f;

            bool isTranslate = TransformMode == TransformType.Translate;
            bool isRotate = TransformMode == TransformType.Rotate;
            bool isScale = TransformMode == TransformType.Scale;

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

                TMaterial axisMat = TMaterial.CreateUnlitColorMaterialForward(unit);
                axisMat.RenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
                axisMat.RenderParams.LineWidth = 1.0f;
                _axisMat[normalAxis] = axisMat;

                TMaterial planeMat1 = TMaterial.CreateUnlitColorMaterialForward(unit1);
                planeMat1.RenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
                planeMat1.RenderParams.LineWidth = 1.0f;
                _transPlaneMat[(normalAxis << 1) + 0] = planeMat1;

                TMaterial planeMat2 = TMaterial.CreateUnlitColorMaterialForward(unit2);
                planeMat2.RenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
                planeMat2.RenderParams.LineWidth = 1.0f;
                _transPlaneMat[(normalAxis << 1) + 1] = planeMat2;
                
                TMaterial scalePlaneMat = TMaterial.CreateUnlitColorMaterialForward(unit);
                scalePlaneMat.RenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
                scalePlaneMat.RenderParams.LineWidth = 1.0f;
                _scalePlaneMat[normalAxis] = scalePlaneMat;

                VertexLine axisLine = new VertexLine(Vec3.Zero, unit * _axisLength);
                Vec3 halfUnit = unit * _axisHalfLength;

                VertexLine transLine1 = new VertexLine(halfUnit, halfUnit + unit1 * _axisHalfLength);
                transLine1.Vertex0.Color = unit1;
                transLine1.Vertex1.Color = unit1;
                VertexLine transLine2 = new VertexLine(halfUnit, halfUnit + unit2 * _axisHalfLength);
                transLine2.Vertex0.Color = unit2;
                transLine2.Vertex1.Color = unit2;

                VertexLine scaleLine1 = new VertexLine(unit1 * _scaleHalf1LDist, unit2 * _scaleHalf1LDist);
                scaleLine1.Vertex0.Color = unit;
                scaleLine1.Vertex1.Color = unit;
                VertexLine scaleLine2 = new VertexLine(unit1 * _scaleHalf2LDist, unit2 * _scaleHalf2LDist);
                scaleLine2.Vertex0.Color = unit;
                scaleLine2.Vertex1.Color = unit;

                string axis = ((char)('X' + normalAxis)).ToString();

                PrimitiveData axisPrim = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), axisLine);
                axisPrim.SingleBindBone = rootBoneName;
                mesh.RigidChildren.Add(new SkeletalRigidSubMesh(axis + "Axis", new RenderInfo3D(!isRotate, true), ERenderPass.OnTopForward, axisPrim, axisMat));

                float coneHeight = _axisLength - _coneDistance;
                PrimitiveData arrowPrim = Cone.SolidMesh(unit * (_coneDistance + coneHeight * 0.5f), unit, coneHeight, _coneRadius, 6, false);
                arrowPrim.SingleBindBone = rootBoneName;
                mesh.RigidChildren.Add(new SkeletalRigidSubMesh(axis + "Arrow", new RenderInfo3D(!isRotate, true), ERenderPass.OnTopForward, arrowPrim, axisMat));
                
                PrimitiveData transPrim1 = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), transLine1);
                transPrim1.SingleBindBone = rootBoneName;
                mesh.RigidChildren.Add(new SkeletalRigidSubMesh(axis + "TransPlane1", new RenderInfo3D(isTranslate, true), ERenderPass.OnTopForward, transPrim1, planeMat1));

                PrimitiveData transPrim2 = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), transLine2);
                transPrim2.SingleBindBone = rootBoneName;
                mesh.RigidChildren.Add(new SkeletalRigidSubMesh(axis + "TransPlane2", new RenderInfo3D(isTranslate, true), ERenderPass.OnTopForward, transPrim2, planeMat2));

                PrimitiveData scalePrim = PrimitiveData.FromLines(VertexShaderDesc.JustPositions(), scaleLine1, scaleLine2);
                scalePrim.SingleBindBone = rootBoneName;
                mesh.RigidChildren.Add(new SkeletalRigidSubMesh(axis + "ScalePlane", new RenderInfo3D(isScale, true), ERenderPass.OnTopForward, scalePrim, scalePlaneMat));

                PrimitiveData rotPrim = Circle3D.WireframeMesh(_orbRadius, unit, Vec3.Zero, _circlePrecision);
                rotPrim.SingleBindBone = rootBoneName;
                mesh.RigidChildren.Add(new SkeletalRigidSubMesh(axis + "Rotation", new RenderInfo3D(isRotate, true), ERenderPass.OnTopForward, rotPrim, axisMat));
            }

            //Screen-aligned rotation
            PrimitiveData screenRotPrim = Circle3D.WireframeMesh(_circRadius, Vec3.UnitZ, Vec3.Zero, _circlePrecision);
            screenRotPrim.SingleBindBone = screenBoneName;

            mesh.RigidChildren.Add(new SkeletalRigidSubMesh("ScreenRotation", new RenderInfo3D(isRotate, true), ERenderPass.OnTopForward, screenRotPrim, _screenMat));

            //Screen-aligned translation
            Vertex v1 = new Vec3(-_screenTransExtent, -_screenTransExtent, 0.0f);
            Vertex v2 = new Vec3(_screenTransExtent, -_screenTransExtent, 0.0f);
            Vertex v3 = new Vec3(_screenTransExtent, _screenTransExtent, 0.0f);
            Vertex v4 = new Vec3(-_screenTransExtent, _screenTransExtent, 0.0f);
            VertexLineStrip strip = new VertexLineStrip(true, v1, v2, v3, v4);
            PrimitiveData screenTransPrim = PrimitiveData.FromLineStrips(VertexShaderDesc.JustPositions(), strip);
            screenTransPrim.SingleBindBone = screenBoneName;

            mesh.RigidChildren.Add(new SkeletalRigidSubMesh("ScreenTranslation", new RenderInfo3D(isTranslate, true), ERenderPass.OnTopForward, screenTransPrim, _screenMat));

            TMaterial sphereMat = TMaterial.CreateUnlitColorMaterialForward(Color.Orange);
            sphereMat.RenderParams.DepthTest.Enabled = ERenderParamUsage.Enabled;
            sphereMat.RenderParams.DepthTest.UpdateDepth = true;
            sphereMat.RenderParams.DepthTest.Function = EComparison.Lequal;
            sphereMat.RenderParams.LineWidth = 1.0f;
            sphereMat.RenderParams.WriteRed = false;
            sphereMat.RenderParams.WriteGreen = false;
            sphereMat.RenderParams.WriteBlue = false;
            sphereMat.RenderParams.WriteAlpha = false;

            PrimitiveData spherePrim = Sphere.SolidMesh(Vec3.Zero, _orbRadius, 10, 10);
            spherePrim.SingleBindBone = rootBoneName;
            mesh.RigidChildren.Add(new SkeletalRigidSubMesh("RotationSphere", new RenderInfo3D(isRotate, true), ERenderPass.OnTopForward, spherePrim, sphereMat));

            return new SkeletalMeshComponent(mesh, skel);

            #endregion
        }
        
        private TransformType _mode = TransformType.Translate;
        private ISocket _targetSocket = null;

        [Category("Transform Tool 3D")]
        public ESpace TransformSpace
        {
            get => _transformSpace;
            set
            {
                if (_transformSpace == value)
                    return;

                _transformSpace = value;

                RootComponent.SetWorldMatrices(GetWorldMatrix(), GetInvWorldMatrix());
                //_dragMatrix = RootComponent.WorldMatrix;
                //_invDragMatrix = RootComponent.InverseWorldMatrix;

                if (_transformSpace == ESpace.Screen)
                    RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, UpdateScreenSpace);
                else
                    UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, UpdateScreenSpace);
            }
        }

        private void UpdateScreenSpace(float delta)
        {
            if (_targetSocket != null)
                TransformChanged(null);
        }

        [Category("Transform Tool 3D")]
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
                        _mouseDown = MouseDownRotation;
                        _mouseUp = MouseUpRotation;
                        break;
                    case TransformType.Translate:
                        _highlight = HighlightTranslation;
                        _drag = DragTranslation;
                        _mouseDown = MouseDownTranslation;
                        _mouseUp = MouseUpTranslation;
                        break;
                    case TransformType.Scale:
                        _highlight = HighlightScale;
                        _drag = DragScale;
                        _mouseDown = MouseDownScale;
                        _mouseUp = MouseUpScale;
                        break;
                }
                int x = 0;

                if (RootComponent.Meshes is null)
                    return;

                for (int i = 0; i < 3; ++i)
                {
                    RootComponent.Meshes[x++].RenderInfo.Visible = _mode != TransformType.Rotate;
                    RootComponent.Meshes[x++].RenderInfo.Visible = _mode != TransformType.Rotate;
                    RootComponent.Meshes[x++].RenderInfo.Visible = _mode == TransformType.Translate;
                    RootComponent.Meshes[x++].RenderInfo.Visible = _mode == TransformType.Translate;
                    RootComponent.Meshes[x++].RenderInfo.Visible = _mode == TransformType.Scale;
                    RootComponent.Meshes[x++].RenderInfo.Visible = _mode == TransformType.Rotate;
                }
                RootComponent.Meshes[x++].RenderInfo.Visible = _mode == TransformType.Rotate;
                RootComponent.Meshes[x++].RenderInfo.Visible = _mode == TransformType.Translate;
                RootComponent.Meshes[x++].RenderInfo.Visible = _mode == TransformType.Rotate;

                GetDependentColors();
            }
        }

        private void MouseUpScale()
        {

        }

        private void MouseDownScale()
        {

        }

        private void MouseUpTranslation()
        {

        }

        private void MouseDownTranslation()
        {

        }

        private void MouseUpRotation()
        {

        }

        private void MouseDownRotation()
        {

        }

        /// <summary>
        /// The socket transform that is being manipulated by this transform tool.
        /// </summary>
        [Browsable(false)]
        public ISocket TargetSocket
        {
            get => _targetSocket;
            set
            {
                if (_targetSocket != null)
                {
//#if EDITOR
//                    _targetSocket.Selected = false;
//#endif
                    _targetSocket.SocketTransformChanged -= Instance.TransformChanged;
                }
                _targetSocket = value;
                if (_targetSocket != null)
                {
//#if EDITOR
//                    _targetSocket.Selected = true;
//#endif
                    
                    RootComponent.SetWorldMatrices(GetWorldMatrix(), GetInvWorldMatrix());
                    _targetSocket.SocketTransformChanged += Instance.TransformChanged;
                }
                else
                    RootComponent.SetWorldMatrices(Matrix4.Identity, Matrix4.Identity);

                //_dragMatrix = RootComponent.WorldMatrix;
                //_invDragMatrix = RootComponent.InverseWorldMatrix;
            }
        }

        private Matrix4 GetWorldMatrix()
        {
            if (_targetSocket is null)
                return Matrix4.Identity;

            switch (TransformSpace)
            {
                case ESpace.Local:
                    {
                        return _targetSocket.WorldMatrix.ClearScale();
                    }
                case ESpace.Parent:
                    {
                        if (_targetSocket.ParentSocket != null)
                            return _targetSocket.ParentSocket.WorldMatrix.ClearScale();
                        else
                            return _targetSocket.WorldMatrix.Translation.AsTranslationMatrix();
                    }
                case ESpace.Screen:
                    {
                        Vec3 point = _targetSocket.WorldMatrix.Translation;
                        var localPlayers = OwningWorld.CurrentGameMode.LocalPlayers;
                        if (localPlayers.Count > 0)
                        {
                            ICamera c = localPlayers[0].ViewportCamera;
                            if (c != null)
                            {
                                //Rotator angles = (c.WorldPoint - point).LookatAngles();
                                //Matrix4 angleMatrix = angles.GetMatrix();
                                //return point.AsTranslationMatrix() * angleMatrix;
                                Vec3 fwd = (c.WorldPoint - point).NormalizedFast();
                                Vec3 up = c.UpVector;
                                Vec3 right = up ^ fwd;
                                return Matrix4.CreateSpacialTransform(point, right, up, fwd);
                            }
                        }

                        return Matrix4.Identity;
                    }
                case ESpace.World:
                default:
                    {
                        return _targetSocket.WorldMatrix.Translation.AsTranslationMatrix();
                    }
            }
        }
        private Matrix4 GetInvWorldMatrix()
        {
            if (_targetSocket is null)
                return Matrix4.Identity;

            switch (TransformSpace)
            {
                case ESpace.Local:
                    {
                        return _targetSocket.InverseWorldMatrix.ClearScale();
                    }
                case ESpace.Parent:
                    {
                        if (_targetSocket.ParentSocket != null)
                            return _targetSocket.ParentSocket.InverseWorldMatrix.ClearScale();
                        else
                            return _targetSocket.InverseWorldMatrix.Translation.AsTranslationMatrix();
                    }
                case ESpace.Screen:
                    {
                        ICamera c = OwningWorld.CurrentGameMode.LocalPlayers[0].ViewportCamera;
                        Matrix4 mtx = c.CameraToWorldSpaceMatrix;
                        mtx.Translation = _targetSocket.InverseWorldMatrix.Translation;
                        return mtx;
                    }
                case ESpace.World:
                default:
                    {
                        return _targetSocket.InverseWorldMatrix.Translation.AsTranslationMatrix();
                    }
            }
        }

        public static void DestroyInstance()
        {
            Instance.Despawn();
        }
        public static TransformTool3D GetInstance(IWorld world, ISocket comp, TransformType transformType)
        {
            if (world is null)
                return null;

            if (!Instance.IsSpawnedIn(world))
            {
                Instance.Despawn();
                world.SpawnActor(Instance);
            }

            Instance.TargetSocket = comp;
            Instance.TransformMode = transformType;

            return Instance;
        }

        private void TransformChanged(ISocket socket)
        {
            if (!_pressed)
            {
                _pressed = true;
                RootComponent.SetWorldMatrices(GetWorldMatrix(), GetInvWorldMatrix());
                //_dragMatrix = RootComponent.WorldMatrix;
                //_invDragMatrix = RootComponent.InverseWorldMatrix;
                _pressed = false;
            }
        }

        //public override void OnSpawnedPreComponentSetup()
        //{
        //    //OwningWorld.Scene.Add(this);
        //}
        //public override void OnDespawned()
        //{
        //    //OwningWorld.Scene.Remove(this);
        //}

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
        
        Vec3 _lastPointWorld;
        Vec3 _localDragPlaneNormal;

        private Action _mouseUp, _mouseDown;
        private DelDrag _drag;
        private DelHighlight _highlight;
        private delegate bool DelHighlight(ICamera camera, Ray localRay);
        private delegate void DelDrag(Vec3 dragPoint);
        private delegate void DelDragRot(Quat dragPoint);

        #region Drag
        private bool 
            _snapRotations = false,
            _snapTranslations = false,
            _snapScale = false;
        private float _rotationSnapBias = 0.0f;
        private float _rotationSnapInterval = 5.0f;
        private float _translationSnapBias = 0.0f;
        private float _translationSnapInterval = 30.0f;
        private float _scaleSnapBias = 0.0f;
        private float _scaleSnapInterval = 0.25f;
        private void DragRotation(Vec3 dragPointWorld)
        {
            TMath.AxisAngleBetween(_lastPointWorld, dragPointWorld, out Vec3 axis, out float angle);
            //if (angle == 0.0f)
            //    return;

            //if (_snapRotations)
            //    angle = angle.RoundToNearest(_rotationSnapBias, _rotationSnapInterval);

            Quat delta = Quat.FromAxisAngleDeg(axis, angle);
            
            _targetSocket.HandleRotation(delta);

            RootComponent.SetWorldMatrices(GetWorldMatrix(), GetInvWorldMatrix());
        }
        
        private void DragTranslation(Vec3 dragPointWorld)
        {
            Vec3 delta = dragPointWorld - _lastPointWorld;
            
            //Matrix4 m = _targetSocket.InverseWorldMatrix.ClearScale();
            //m = m.ClearTranslation();
            //Vec3 worldTrans = m * delta;

            //if (_snapTranslations)
            //{
            //    //Modify delta to move resulting world point to nearest snap
            //    Vec3 worldPoint = _targetSocket.WorldMatrix.Translation;
            //    Vec3 resultPoint = worldPoint + worldTrans;

            //    resultPoint.X = resultPoint.X.RoundToNearest(_translationSnapBias, _translationSnapInterval);
            //    resultPoint.Y = resultPoint.Y.RoundToNearest(_translationSnapBias, _translationSnapInterval);
            //    resultPoint.Z = resultPoint.Z.RoundToNearest(_translationSnapBias, _translationSnapInterval);

            //    worldTrans = resultPoint - worldPoint;
            //}

            _targetSocket.HandleTranslation(delta);

            RootComponent.SetWorldMatrices(GetWorldMatrix(), GetInvWorldMatrix());
        }
        private void DragScale(Vec3 dragPointWorld)
        {
            Vec3 delta = dragPointWorld - _lastPointWorld;
            _targetSocket.HandleScale(delta);

            RootComponent.SetWorldMatrices(GetWorldMatrix(), GetInvWorldMatrix());
        }
        /// <summary>
        /// Returns a point relative to the local space of the target socket (origin at 0,0,0), clamped to the highlighted drag plane.
        /// </summary>
        /// <param name="camera">The camera viewing this tool, used for camera space drag clamping.</param>
        /// <param name="localRay">The mouse ray, transformed into the socket's local space.</param>
        /// <returns></returns>
        private Vec3 GetLocalDragPoint(ICamera camera, Ray localRay)
        {
            //Convert all coordinates to local space

            Vec3 localCamPoint = camera.WorldPoint * RootComponent.InverseWorldMatrix;
            Vec3 localDragPoint, unit;

            switch (_mode)
            {
                case TransformType.Scale:
                case TransformType.Translate:
                    {
                        if (_hiCam)
                        {
                            _localDragPlaneNormal = localCamPoint;
                            _localDragPlaneNormal.Normalize();
                        }
                        else if (_hiAxis.X)
                        {
                            if (_hiAxis.Y)
                            {
                                _localDragPlaneNormal = Vec3.UnitZ;
                            }
                            else if (_hiAxis.Z)
                            {
                                _localDragPlaneNormal = Vec3.UnitY;
                            }
                            else
                            {
                                unit = Vec3.UnitX;
                                Vec3 perpPoint = Ray.GetClosestColinearPoint(Vec3.Zero, unit, localCamPoint);
                                _localDragPlaneNormal = localCamPoint - perpPoint;
                                _localDragPlaneNormal.Normalize();

                                if (!Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, _localDragPlaneNormal, out localDragPoint))
                                    return _lastPointWorld;

                                return Ray.GetClosestColinearPoint(Vec3.Zero, unit, localDragPoint);
                            }
                        }
                        else if (_hiAxis.Y)
                        {
                            if (_hiAxis.X)
                            {
                                _localDragPlaneNormal = Vec3.UnitZ;
                            }
                            else if (_hiAxis.Z)
                            {
                                _localDragPlaneNormal = Vec3.UnitX;
                            }
                            else
                            {
                                unit = Vec3.UnitY;
                                Vec3 perpPoint = Ray.GetClosestColinearPoint(Vec3.Zero, unit, localCamPoint);
                                _localDragPlaneNormal = localCamPoint - perpPoint;
                                _localDragPlaneNormal.Normalize();

                                if (!Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, _localDragPlaneNormal, out localDragPoint))
                                    return _lastPointWorld;

                                return Ray.GetClosestColinearPoint(Vec3.Zero, unit, localDragPoint);
                            }
                        }
                        else if (_hiAxis.Z)
                        {
                            if (_hiAxis.X)
                            {
                                _localDragPlaneNormal = Vec3.UnitY;
                            }
                            else if (_hiAxis.Y)
                            {
                                _localDragPlaneNormal = Vec3.UnitX;
                            }
                            else
                            {
                                unit = Vec3.UnitZ;
                                Vec3 perpPoint = Ray.GetClosestColinearPoint(Vec3.Zero, unit, localCamPoint);
                                _localDragPlaneNormal = localCamPoint - perpPoint;
                                _localDragPlaneNormal.Normalize();

                                if (!Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, _localDragPlaneNormal, out localDragPoint))
                                    return _lastPointWorld;

                                return Ray.GetClosestColinearPoint(Vec3.Zero, unit, localDragPoint);
                            }
                        }

                        if (Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, _localDragPlaneNormal, out localDragPoint))
                            return localDragPoint;
                    }
                    break;
                case TransformType.Rotate:
                    {
                        if (_hiCam)
                        {
                            _localDragPlaneNormal = localCamPoint;
                            _localDragPlaneNormal.Normalize();

                            if (Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, _localDragPlaneNormal, out localDragPoint))
                                return localDragPoint;
                        }
                        else if (_hiAxis.Any)
                        {
                            if (_hiAxis.X)
                                unit = Vec3.UnitX;
                            else if (_hiAxis.Y)
                                unit = Vec3.UnitY;
                            else// if (_hiAxis.Z)
                                unit = Vec3.UnitZ;

                            _localDragPlaneNormal = unit;
                            _localDragPlaneNormal.Normalize();

                            if (Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, _localDragPlaneNormal, out localDragPoint))
                                return localDragPoint;
                        }
                        else if (_hiSphere)
                        {
                            Vec3 worldPoint = RootComponent.WorldPoint;
                            float radius = camera.DistanceScale(worldPoint, _orbRadius);

                            if (Collision.RayIntersectsSphere(localRay.StartPoint, localRay.Direction, Vec3.Zero, radius * _circOrbScale, out localDragPoint))
                            {
                                _localDragPlaneNormal = localDragPoint.Normalized();
                                return localDragPoint;
                            }
                        }
                    }
                    break;
            }
            
            return _lastPointWorld;
        }
#endregion

        #region Highlighting
        private bool HighlightRotation(ICamera camera, Ray localRay)
        {
            Vec3 worldPoint = RootComponent.WorldMatrix.Translation;
            float radius = camera.DistanceScale(worldPoint, _orbRadius);

            if (!Collision.RayIntersectsSphere(localRay.StartPoint, localRay.Direction, Vec3.Zero, radius * _circOrbScale, out Vec3 point))
            {
                //If no intersect is found, project the ray through the plane perpendicular to the camera.
                //localRay.LinePlaneIntersect(Vec3.Zero, (camera.WorldPoint - worldPoint).Normalized(), out point);
                Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, (camera.WorldPoint - worldPoint) * RootComponent.InverseWorldMatrix, out point);

                //Clamp the point to edge of the sphere
                point = Ray.PointAtLineDistance(Vec3.Zero, point, radius);

                //Point lies on circ line?
                float distance = point.LengthFast;
                if (Math.Abs(distance - radius * _circOrbScale) < radius * _selectOrbScale)
                    _hiCam = true;
            }
            else
            {
                point.NormalizeFast();
                
                _hiSphere = true;
                
                float x = point.Dot(Vec3.UnitX);
                float y = point.Dot(Vec3.UnitY);
                float z = point.Dot(Vec3.UnitZ);

                if (Math.Abs(x) < 0.3f)
                {
                    _hiAxis.X = true;
                }
                else if (Math.Abs(y) < 0.3f)
                {
                    _hiAxis.Y = true;
                }
                else if (Math.Abs(z) < 0.3f)
                {
                    _hiAxis.Z = true;
                }
            }

            return _hiAxis.Any || _hiCam || _hiSphere;
        }
        private bool HighlightTranslation(ICamera camera, Ray localRay)
        {
            Vec3 worldPoint = RootComponent.WorldMatrix.Translation;
            float radius = camera.DistanceScale(worldPoint, _orbRadius);

            List<Vec3> intersectionPoints = new List<Vec3>(3);

            bool snapFound = false;
            for (int normalAxis = 0; normalAxis < 3; ++normalAxis)
            {
                Vec3 unit = Vec3.Zero;
                unit[normalAxis] = localRay.StartPoint[normalAxis] < 0.0f ? -1.0f : 1.0f;

                //Get plane intersection point for cursor ray and each drag plane
                if (Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, unit, out Vec3 point))
                    intersectionPoints.Add(point);
            }

            //_intersectionPoints.Sort((l, r) => l.DistanceToSquared(camera.WorldPoint).CompareTo(r.DistanceToSquared(camera.WorldPoint)));

            foreach (Vec3 v in intersectionPoints)
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
        private bool HighlightScale(ICamera camera, Ray localRay)
        {
            Vec3 worldPoint = RootComponent.WorldMatrix.Translation;
            float radius = camera.DistanceScale(worldPoint, _orbRadius);

            List<Vec3> intersectionPoints = new List<Vec3>(3);

            bool snapFound = false;
            for (int normalAxis = 0; normalAxis < 3; ++normalAxis)
            {
                Vec3 unit = Vec3.Zero;
                unit[normalAxis] = localRay.StartPoint[normalAxis] < 0.0f ? -1.0f : 1.0f;
                
                //Get plane intersection point for cursor ray and each drag plane
                if (Collision.RayIntersectsPlane(localRay.StartPoint, localRay.Direction, Vec3.Zero, unit, out Vec3 point))
                    intersectionPoints.Add(point);
            }

            //_intersectionPoints.Sort((l, r) => l.DistanceToSquared(camera.WorldPoint).CompareTo(r.DistanceToSquared(camera.WorldPoint)));
            
            foreach (Vec3 v in intersectionPoints)
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
        //private Matrix4 _dragMatrix, _invDragMatrix;

        /// <summary>
        /// Returns true if intersecting one of the transform tool's various parts.
        /// </summary>
        public bool MouseMove(Ray cursor, ICamera camera, bool pressed)
        {
            bool snapFound = true;
            if (pressed)
            {
                if (_hiAxis.None && !_hiCam && !_hiSphere)
                    return false;

                if (!_pressed)
                    OnPressed();

                Ray localRay = cursor.TransformedBy(RootComponent.InverseWorldMatrix);
                Vec3 localDragPoint = GetLocalDragPoint(camera, localRay);
                Vec3 worldDragPoint = Vec3.TransformPosition(localDragPoint, RootComponent.WorldMatrix);
                _drag(worldDragPoint);

                _lastPointWorld = worldDragPoint;
            }
            else
            {
                if (_pressed)
                    OnReleased();

                Ray localRay = cursor.TransformedBy(RootComponent.InverseWorldMatrix);

                _hiAxis.X = _hiAxis.Y = _hiAxis.Z = false;
                _hiCam = _hiSphere = false;

                snapFound = _highlight?.Invoke(camera, localRay) ?? false;

                _axisMat[0].Parameter<ShaderVec4>(0).Value = _hiAxis.X ? (ColorF4)Color.Yellow : Color.Red;
                _axisMat[1].Parameter<ShaderVec4>(0).Value = _hiAxis.Y ? (ColorF4)Color.Yellow : Color.Green;
                _axisMat[2].Parameter<ShaderVec4>(0).Value = _hiAxis.Z ? (ColorF4)Color.Yellow : Color.Blue;
                _screenMat.Parameter<ShaderVec4>(0).Value = _hiCam ? (ColorF4)Color.Yellow : Color.LightGray;

                GetDependentColors();

                Vec3 localDragPoint = GetLocalDragPoint(camera, localRay);
                _lastPointWorld = Vec3.TransformPosition(localDragPoint, RootComponent.WorldMatrix);
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

        [Browsable(false)]
        public Matrix4 PrevRootWorldMatrix { get; private set; } = Matrix4.Identity;
        private void OnPressed()
        {
            if (_targetSocket != null)
                RootComponent.SetWorldMatrices(GetWorldMatrix(), GetInvWorldMatrix());
            else
                RootComponent.SetWorldMatrices(Matrix4.Identity, Matrix4.Identity);

            _pressed = true;

            PrevRootWorldMatrix = _targetSocket.WorldMatrix;
            MouseDown?.Invoke();
        }
        private void OnReleased()
        {
            _pressed = false;
            MouseUp?.Invoke();
        }

        public event Action MouseDown, MouseUp;

        //UIString2D _xText, _yText, _zText;

        private void Render(bool shadowPass)
        {
            //if (_hiCam || _hiSphere || _hiAxis.Any)
            //{
            //    Engine.Renderer.RenderPoint(_lastPointWorld, Color.Black, false);
            //    Vec3 worldNormal = Vec3.TransformVector(_localDragPlaneNormal, RootComponent.WorldMatrix);
            //    Engine.Renderer.RenderLine(_lastPointWorld, _lastPointWorld + worldNormal * Engine.Renderer.CurrentCamera.DistanceScale(RootComponent.WorldPoint, 2.0f), Color.Black, false);
            //}
        }

        private readonly RenderCommandMethod3D _rc;
        public void AddRenderables(RenderPasses passes, ICamera camera) => passes.Add(_rc);
    }
}
