using CustomEngine.GameModes;
using CustomEngine.Worlds.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Thera.GameModes
{
    public struct ZoomLevel
    {
        public ZoomLevel(float fovX, float aimAssistDistance)
        {
            _fovX = fovX;
            _aimAssistDistance = aimAssistDistance;
        }

        private float _fovX;
        private float _aimAssistDistance;

        public static readonly ZoomLevel DefaultNonZoomed = new ZoomLevel(90.0f, 1000.0f);
        public static readonly ZoomLevel DefaultZoomed = new ZoomLevel(60.0f, 2000.0f);

        public float FovX { get => _fovX; set => _fovX = value; }
        public float AimAssistDistance { get => _aimAssistDistance; set => _aimAssistDistance = value; }
    }
    public struct StaticPlayerTraits
    {
        ZoomLevel _defaultNonZoomed;
        ZoomLevel _defaultZoomed;

        public void InitDefaults()
        {
            _defaultNonZoomed = ZoomLevel.DefaultNonZoomed;
            _defaultZoomed = ZoomLevel.DefaultZoomed;
        }
    }
    public struct InheritablePlayerTraits
    {
        InheritableBool _allowCrouching;
        InheritableBool _allowFlying;
        InheritableBool _allowSwimming;
        InheritableBool _allowSprinting;
        InheritableBool _allowSprintingWhileCrouched;
        InheritableBool _allowSprintingWhileFlying;
        InheritableBool _allowSprintingWhileSwimming;

        InheritableFloat _walkingMultiplier;
        InheritableFloat _crouchedMovementMultiplier;
        InheritableFloat _flyingMultiplier;
        InheritableFloat _swimmingMultiplier;
        InheritableFloat _sprintingMultiplier;
        InheritableFloat _crouchedSprintMultiplier;
        InheritableFloat _flyingSprintMultiplier;
        InheritableFloat _swimmingSprintMultiplier;

        InheritableBool _allowVehicleUse;
        InheritableBool _allowGunPickup;
        InheritableBool _allowMeleeWeaponPickup;
        InheritableBool _allowUpgradePickup;
        InheritableBool _allowHealthPickup;
        InheritableBool _allowToolPickup;
        InheritableBool _allowMagicPickup;
        InheritableBool _allowStageInteraction;

        InheritableBool _shieldsEnabled;
        InheritableFloat _shieldStrengthMultiplier;
        InheritableBool _shieldRecoveryEnabled;
        InheritableBool _shieldsFixable;
        InheritableFloat _shieldFixWaitSeconds;
        InheritableFloat _shieldRecoveryWaitSeconds;
        InheritableFloat _shieldRecoverySeconds;
        InheritableFloat _shieldBarDecaySeconds;
        InheritableInt _overshieldCount;
        InheritableBool _overshieldsRegenerate;

        InheritableFloat _deathWaitSeconds;
        InheritableFloat _waitAmountPerBetrayal;

        InheritableBool _radarsEnabled;
        InheritableFloat _radarDistance;
        InheritableBool _enemyWaypoints;
        InheritableBool _allyWaypoints;

        public void InitDefaultInherited()
        {
            _allowCrouching = InheritableBool.Inherited;
            _allowFlying = InheritableBool.Inherited;
            _allowSwimming = InheritableBool.Inherited;
            _allowSprinting = InheritableBool.Inherited;
            _allowSprintingWhileCrouched = InheritableBool.Inherited;
            _allowSprintingWhileFlying = InheritableBool.Inherited;
            _allowSprintingWhileSwimming = InheritableBool.Inherited;

            _walkingMultiplier = true;
            _crouchedMovementMultiplier = true;
            _flyingMultiplier = true;
            _swimmingMultiplier = true;
            _sprintingMultiplier = true;
            _crouchedSprintMultiplier = true;
            _flyingSprintMultiplier = true;
            _swimmingSprintMultiplier = true;

            _allowVehicleUse = InheritableBool.Inherited;
            _allowGunPickup = InheritableBool.Inherited;
            _allowMeleeWeaponPickup = InheritableBool.Inherited;
            _allowUpgradePickup = InheritableBool.Inherited;
            _allowHealthPickup = InheritableBool.Inherited;
            _allowToolPickup = InheritableBool.Inherited;
            _allowMagicPickup = InheritableBool.Inherited;
            _allowStageInteraction = InheritableBool.Inherited;

            _shieldsEnabled = InheritableBool.Inherited;
            _shieldStrengthMultiplier = true;
            _shieldRecoveryEnabled = InheritableBool.Inherited;
            _shieldsFixable = InheritableBool.Inherited;
            _shieldFixWaitSeconds = true;
            _shieldRecoveryWaitSeconds = true;
            _shieldRecoverySeconds = true;
            _shieldBarDecaySeconds = true;
            _overshieldCount = true;
            _overshieldsRegenerate = InheritableBool.Inherited;

            _deathWaitSeconds = true;
            _waitAmountPerBetrayal = true;

            _radarsEnabled = InheritableBool.Inherited;
            _radarDistance = true;
            _enemyWaypoints = InheritableBool.Inherited;
            _allyWaypoints = InheritableBool.Inherited;
        }
        public void InitDefaultNonInherited()
        {
            _allowCrouching = InheritableBool.True;
            _allowFlying = InheritableBool.True;
            _allowSwimming = InheritableBool.True;
            _allowSprinting = InheritableBool.True;
            _allowSprintingWhileCrouched = InheritableBool.True;
            _allowSprintingWhileFlying = InheritableBool.True;
            _allowSprintingWhileSwimming = InheritableBool.True;

            _walkingMultiplier = 1.0f;
            _crouchedMovementMultiplier = 1.0f;
            _flyingMultiplier = 1.0f;
            _swimmingMultiplier = 1.0f;
            _sprintingMultiplier = 1.0f;
            _crouchedSprintMultiplier = 1.0f;
            _flyingSprintMultiplier = 1.0f;
            _swimmingSprintMultiplier = 1.0f;

            _allowVehicleUse = InheritableBool.True;
            _allowGunPickup = InheritableBool.True;
            _allowMeleeWeaponPickup = InheritableBool.True;
            _allowUpgradePickup = InheritableBool.True;
            _allowHealthPickup = InheritableBool.True;
            _allowToolPickup = InheritableBool.True;
            _allowMagicPickup = InheritableBool.True;
            _allowStageInteraction = InheritableBool.True;

            _shieldsEnabled = InheritableBool.True;
            _shieldStrengthMultiplier = 1.0f;
            _shieldRecoveryEnabled = InheritableBool.True;
            _shieldsFixable = InheritableBool.True;
            _shieldFixWaitSeconds = 6.0f;
            _shieldRecoveryWaitSeconds = 6.0f;
            _shieldRecoverySeconds = 3.0f;
            _shieldBarDecaySeconds = 0.0f;
            _overshieldCount = 0;
            _overshieldsRegenerate = InheritableBool.False;

            _deathWaitSeconds = 3.0f;
            _waitAmountPerBetrayal = 3.0f;

            _radarsEnabled = InheritableBool.True;
            _radarDistance = 100.0f;
            _enemyWaypoints = InheritableBool.False;
            _allyWaypoints = InheritableBool.True;
        }
        /// <summary>
        /// child first, parent last order
        /// </summary>
        public static InheritablePlayerTraits InheritHierarchy(params InheritablePlayerTraits[] inheritance)
        {
            InheritablePlayerTraits finalTraits = inheritance[0];
            for (int i = 1; i < inheritance.Length; ++i)
                finalTraits = finalTraits.InheritedFrom(inheritance[i]);
            return finalTraits;
        }
        public InheritablePlayerTraits InheritedFrom(InheritablePlayerTraits parent)
        {
            InheritablePlayerTraits newTraits = new InheritablePlayerTraits();
            FieldInfo[] fields = typeof(InheritablePlayerTraits).GetFields();
            foreach (FieldInfo field in fields)
            {
                object fieldValueParent = field.GetValue(parent);
                object fieldValueChild = field.GetValue(this);
                if (fieldValueChild is InheritableBool b)
                    field.SetValue(newTraits, b == InheritableBool.Inherited ? fieldValueParent : fieldValueChild);
            }
            return newTraits;
        }
    }
}
