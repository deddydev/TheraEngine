using System;
using TheraEngine.Components;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering.Models
{
    public partial class Bone
    {
        public void CalcFrameMatrix(ICamera camera, Matrix4 parentMatrix, Matrix4 inverseParentMatrix, bool force = false)
        {
            //WorldMatrix = _rigidBodyCollision.WorldTransform;

            bool usesCamera = UsesCamera;
            bool needsUpdate = FrameMatrixChanged || force || usesCamera;
            if (needsUpdate)
            {
                if (_rigidBodyCollision != null)
                {
                    Matrix4 bodyMtx = _rigidBodyCollision.WorldTransform;
                    bodyMtx = bodyMtx.ClearScale();
                    Matrix4 worldMatrix = bodyMtx * _rigidBodyLocalTransform.InverseMatrix;
                    Matrix4 frameMatrix = worldMatrix * OwningComponent.InverseWorldMatrix;
                    Matrix4 localMatrix = inverseParentMatrix * frameMatrix;
                    FrameState.Matrix = localMatrix;
                }

                //Regular parent-child transformation
                _frameMatrix = parentMatrix * FrameState.Matrix;
                _inverseFrameMatrix = FrameState.InverseMatrix * inverseParentMatrix;

                if (usesCamera)
                {
                    //Align rotation using camera
                    if (BillboardType != EBillboardType.None)
                        HandleBillboarding(parentMatrix, inverseParentMatrix, camera);

                    if (ScaleByDistance && camera != null)
                    {
                        float scale = camera.DistanceScale(WorldMatrix.Translation, DistanceScaleScreenSize);
                        //Engine.PrintLine(scale.ToString());
                        _frameMatrix *= Matrix4.CreateScale(scale);
                        _inverseFrameMatrix = Matrix4.CreateScale(1.0f / scale) * _inverseFrameMatrix;
                    }
                }

                //Precalculate vertex/normal weighting matrices
                _vtxPosMtx = FrameMatrix * InverseBindMatrix;
                //_vtxNrmMtx = (BindMatrix * InverseFrameMatrix).Transposed().GetRotationMatrix4();

                //Process skinning information dealing with this bone
                if (Engine.Settings.SkinOnGPU)
                {
                    foreach (var m in _influencedVertices.Values)
                        m.Item1.ModifiedBoneIndicesUpdating.Add(_index);
                }
                else
                {
                    foreach (var m in _influencedVertices.Values)
                        m.Item1.ModifiedVertexIndicesUpdating.UnionWith(m.Item2);
                    _influencedInfluences.ForEach(x => x._hasChanged = true);
                }

                //Recalculate child component transforms
                foreach (SceneComponent comp in ChildComponents)
                    comp.RecalcWorldTransform();

                //Inform subscribers that the bone's transform has changed
                SocketTransformChanged?.Invoke(this);
            }

            //Recalculate child bone transforms
            foreach (Bone b in _childBones)
                b.CalcFrameMatrix(camera, _frameMatrix, _inverseFrameMatrix, needsUpdate);

            FrameMatrixChanged = false;
        }
        private void HandleBillboarding(Matrix4 parentMatrix, Matrix4 inverseParentMatrix, ICamera camera)
        {
            if (camera is null)
                return;

            //Apply local translation component to parent matrix
            Matrix4 frameTrans = parentMatrix * FrameState.Translation.Raw.AsTranslationMatrix();
            Matrix4 invFramTrans = (-FrameState.Translation.Raw).AsTranslationMatrix() * inverseParentMatrix;

            //Reset rotation for billboard
            frameTrans = parentMatrix.ClearRotation();
            invFramTrans = inverseParentMatrix.ClearRotation();

            //Calculate angles from current position to camera
            Matrix4 angles = Matrix4.Identity, invAngles = Matrix4.Identity;
            switch (BillboardType)
            {
                case EBillboardType.PerspectiveXYZ:

                    Vec3 componentPoint = camera.WorldPoint * OwningComponent.InverseWorldMatrix;
                    Vec3 diff = frameTrans.Translation - componentPoint;
                    Rotator r = diff.LookatAngles();

                    angles = r.GetMatrix();
                    invAngles = r.GetInverseMatrix();

                    break;

                case EBillboardType.PerspectiveXY:

                    break;

                case EBillboardType.PerspectiveY:

                    break;

                case EBillboardType.OrthographicXYZ:

                    Vec3 up1 = camera.UpVector;
                    Vec3 forward1 = camera.ForwardVector;

                    angles = new Matrix4(
                        new Vec4(forward1 ^ up1, 0.0f),
                        new Vec4(up1, 0.0f),
                        new Vec4(forward1, 0.0f),
                        Vec4.UnitW);

                    invAngles = new Matrix4(
                        new Vec4(up1 ^ forward1, 0.0f),
                        new Vec4(-up1, 0.0f),
                        new Vec4(-forward1, 0.0f),
                        Vec4.UnitW);

                    break;

                case EBillboardType.OrthographicXY:

                    Vec3 forward2 = camera.ForwardVector;
                    Vec3 right2 = camera.RightVector;
                    right2.Y = 0.0f;

                    angles = new Matrix4(
                        new Vec4(right2, 0.0f),
                        new Vec4(right2 ^ forward2, 0.0f),
                        new Vec4(forward2, 0.0f),
                        Vec4.UnitW);

                    invAngles = new Matrix4(
                        new Vec4(-right2, 0.0f),
                        new Vec4(forward2 ^ right2, 0.0f),
                        new Vec4(-forward2, 0.0f),
                        Vec4.UnitW);

                    break;

                case EBillboardType.OrthographicY:

                    Vec3 up3 = Vec3.TransformNormalInverse(Vec3.UnitY, inverseParentMatrix); //Up is related to parent
                    Vec3 forward3 = camera.ForwardVector;
                    forward3.Y = 0.0f;

                    angles = new Matrix4(
                        new Vec4(forward3 ^ up3, 0.0f),
                        new Vec4(up3, 0.0f),
                        new Vec4(forward3, 0.0f),
                        Vec4.UnitW);

                    invAngles = new Matrix4(
                        new Vec4(up3 ^ forward3, 0.0f),
                        new Vec4(-up3, 0.0f),
                        new Vec4(-forward3, 0.0f),
                        Vec4.UnitW);

                    break;
            }

            //Fix rotation in relation to parent component
            if (OwningComponent != null)
            {
                angles = OwningComponent.InverseWorldMatrix.GetRotationMatrix4() * angles;
                invAngles *= OwningComponent.WorldMatrix.GetRotationMatrix4();
            }

            //Multiply translation, rotation and scale parts together
            _frameMatrix = frameTrans * angles * FrameState.Scale.Raw.AsScaleMatrix();
            _inverseFrameMatrix = (1.0f / FrameState.Scale).AsScaleMatrix() * invAngles * invFramTrans;
        }
    }
}