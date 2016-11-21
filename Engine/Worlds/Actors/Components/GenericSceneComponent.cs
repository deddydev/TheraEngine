using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors.Components
{
    public class GenericSceneComponent : SceneComponent
    {
        protected FrameState _localState = FrameState.Identity;

        public GenericSceneComponent(
            Matrix4.MultiplyOrder transformationOrder = Matrix4.MultiplyOrder.TRS,
            Vec3.EulerOrder rotationOrder = Vec3.EulerOrder.YPR)
        {
            _localState = FrameState.GetIdentity(transformationOrder);
            _localState.RotationOrder = rotationOrder;
            _localState.MatrixChanged += MatrixChanged;
        }
        private void MatrixChanged(Matrix4 oldMatrix, Matrix4 oldInvMatrix)
        {
            RecalcLocalTransform();
        }
        public override void RecalcLocalTransform()
        {
            _localTransform = _localState.Matrix;
            _invLocalTransform = _localState.InverseMatrix;
            RecalcGlobalTransform();
        }
    }
}
