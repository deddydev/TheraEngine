using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using TheraEngine;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Animation;
using TheraEngine.Components;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Text;
using TheraEngine.Rendering.UI;

namespace TheraEditor.Windows.Forms
{
    public partial class EditorUIPropAnimFloat : EditorUI2DBase, I2DRenderable, IPreRendered
    {

        private float GetAcceleration(FloatKeyframe kf, bool inAcc)
        {
            float acc = kf.Interpolate(kf.Second + (inAcc ? -0.0001f : 0.0001f), EVectorInterpValueType.Acceleration);
            if (inAcc)
                acc = -acc;
            return acc;
        }
        private float GetCurrentPosition()
        {
            switch (ValueDisplayMode)
            {
                case EVectorInterpValueType.Position:
                    return _targetAnimation.CurrentPosition;
                case EVectorInterpValueType.Velocity:
                    return _targetAnimation.CurrentVelocity;
                case EVectorInterpValueType.Acceleration:
                    return _targetAnimation.CurrentAcceleration;
                default:
                    return 0.0f;
            }
        }
        private float GetCurrentVelocity()
        {
            switch (ValueDisplayMode)
            {
                case EVectorInterpValueType.Position:
                    return _targetAnimation.CurrentVelocity;
                case EVectorInterpValueType.Velocity:
                    return _targetAnimation.CurrentAcceleration;
                case EVectorInterpValueType.Acceleration:
                default:
                    return 0.0f;
            }
        }
        private float GetCurrentAcceleration()
        {
            switch (ValueDisplayMode)
            {
                case EVectorInterpValueType.Position:
                    return _targetAnimation.CurrentAcceleration;
                case EVectorInterpValueType.Velocity:
                case EVectorInterpValueType.Acceleration:
                default:
                    return 0.0f;
            }
        }
        private void GetDisplayValue(EVectorInterpValueType type, FloatKeyframe kf, bool inPos, out float pos, out float tan)
        {
            switch (type)
            {
                default:
                case EVectorInterpValueType.Position:
                    pos = inPos ? kf.InValue : kf.OutValue;
                    tan = inPos ? kf.InTangent : kf.OutTangent;
                    break;
                case EVectorInterpValueType.Velocity:
                    pos = inPos ? kf.InTangent : kf.OutTangent;
                    tan = GetAcceleration(kf, inPos);
                    break;
                case EVectorInterpValueType.Acceleration:
                    pos = GetAcceleration(kf, inPos);
                    tan = 0.0f;
                    break;
            }
        }
        private float GetDisplayValue(EVectorInterpValueType type, float sec)
        {
            switch (type)
            {
                case EVectorInterpValueType.Position:
                    return _targetAnimation.GetValue(sec);
                case EVectorInterpValueType.Velocity:
                    return _targetAnimation.GetVelocityKeyframed(sec);
                case EVectorInterpValueType.Acceleration:
                    return _targetAnimation.GetAccelerationKeyframed(sec);
                default:
                    return 0.0f;
            }
        }
        private void GetKeyframePosInfo(FloatKeyframe kf, out Vec3 inPos, out Vec3 outPos, out Vec3 inTanPos, out Vec3 outTanPos)
        {
            GetDisplayValue(ValueDisplayMode, kf, true, out float inVal, out float inVel);
            GetDisplayValue(ValueDisplayMode, kf, false, out float outVal, out float outVel);

            Vec2 tangentInVector = new Vec2(-1.0f, inVel);
            Vec2 tangentOutVector = new Vec2(1.0f, outVel);
            tangentInVector.Normalize();
            tangentOutVector.Normalize();
            tangentInVector *= TangentScale / BaseTransformComponent.Scale.XProperty;
            tangentOutVector *= TangentScale / BaseTransformComponent.Scale.XProperty;

            inPos = new Vec3(kf.Second, inVal, 0.0f);
            inTanPos = inPos + tangentInVector;

            outPos = new Vec3(kf.Second, outVal, 0.0f);
            outTanPos = outPos + tangentOutVector;
        }
        private void GetSplineVertex(float sec, float maxVelocity, out Vec3 pos, out ColorF4 color)
        {
            float val = GetDisplayValue(ValueDisplayMode, sec);
            float vel = GetDisplayValue(ValueDisplayMode + 1, sec);

            //float time = 1.0f - 1.0f / (1.0f + VelocitySigmoidScale * (vel * vel));
            float time = Math.Abs(vel) / maxVelocity;

            color = Vec3.Lerp(Vec3.UnitZ, Vec3.UnitX, time);
            pos = new Vec3(sec, val, 0.0f);
        }
        public float GetMaxSpeed()
        {
            _targetAnimation.GetMinMax(true,
              out (float Time, float Value)[] min,
              out (float Time, float Value)[] max);
            return TMath.Max(Math.Abs(min[0].Value), Math.Abs(max[0].Value));
        }
        protected override bool GetWorkArea(out Vec2 min, out Vec2 max)
        {
            if (_targetAnimation is null)
            {
                min = Vec2.Zero;
                max = Vec2.Zero;
                return false;
            }

            _targetAnimation.GetMinMax(false,
                out (float Time, float Value)[] minResult,
                out (float Time, float Value)[] maxResult);

            float minY = minResult[0].Value;
            float maxY = maxResult[0].Value;
            if (minY > maxY)
            {
                min = Vec2.Zero;
                max = Vec2.Zero;
                return false;
            }

            float minX = 0.0f;
            float maxX = _targetAnimation.LengthInSeconds;

            min = new Vec2(minX, minY);
            max = new Vec2(maxX, maxY);
            return true;
        }
    }
}
