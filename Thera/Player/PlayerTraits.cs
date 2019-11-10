using TheraEngine.GameModes;
using System.Reflection;

namespace Thera.GameModes
{
    public struct ZoomLevel
    {
        public ZoomLevel(float fovX, float aimAssistDistance)
        {
            FovX = fovX;
            AimAssistDistance = aimAssistDistance;
        }

        public static readonly ZoomLevel DefaultNonZoomed = new ZoomLevel(90.0f, 1000.0f);
        public static readonly ZoomLevel DefaultZoomed = new ZoomLevel(60.0f, 2000.0f);

        public float FovX { get; set; }
        public float AimAssistDistance { get; set; }
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
        EInheritableBool _allowCrouching;
        EInheritableBool _allowFlying;
        EInheritableBool _allowSwimming;
        EInheritableBool _allowSprinting;
        EInheritableBool _allowSprintingWhileCrouched;
        EInheritableBool _allowSprintingWhileFlying;
        EInheritableBool _allowSprintingWhileSwimming;

        InheritableFloat _walkingMultiplier;
        InheritableFloat _crouchedMovementMultiplier;
        InheritableFloat _flyingMultiplier;
        InheritableFloat _swimmingMultiplier;
        InheritableFloat _sprintingMultiplier;
        InheritableFloat _crouchedSprintMultiplier;
        InheritableFloat _flyingSprintMultiplier;
        InheritableFloat _swimmingSprintMultiplier;

        EInheritableBool _allowVehicleUse;
        EInheritableBool _allowGunPickup;
        EInheritableBool _allowMeleeWeaponPickup;
        EInheritableBool _allowUpgradePickup;
        EInheritableBool _allowHealthPickup;
        EInheritableBool _allowToolPickup;
        EInheritableBool _allowMagicPickup;
        EInheritableBool _allowStageInteraction;

        EInheritableBool _shieldsEnabled;
        InheritableFloat _shieldStrengthMultiplier;
        EInheritableBool _shieldRecoveryEnabled;
        EInheritableBool _shieldsFixable;
        InheritableFloat _shieldFixWaitSeconds;
        InheritableFloat _shieldRecoveryWaitSeconds;
        InheritableFloat _shieldRecoverySeconds;
        InheritableFloat _shieldBarDecaySeconds;
        InheritableInt _overshieldCount;
        EInheritableBool _overshieldsRegenerate;

        InheritableFloat _deathWaitSeconds;
        InheritableFloat _waitAmountPerBetrayal;

        EInheritableBool _radarsEnabled;
        InheritableFloat _radarDistance;
        EInheritableBool _enemyWaypoints;
        EInheritableBool _allyWaypoints;

        public void InitDefaultInherited()
        {
            _allowCrouching = EInheritableBool.Inherited;
            _allowFlying = EInheritableBool.Inherited;
            _allowSwimming = EInheritableBool.Inherited;
            _allowSprinting = EInheritableBool.Inherited;
            _allowSprintingWhileCrouched = EInheritableBool.Inherited;
            _allowSprintingWhileFlying = EInheritableBool.Inherited;
            _allowSprintingWhileSwimming = EInheritableBool.Inherited;

            _walkingMultiplier = true;
            _crouchedMovementMultiplier = true;
            _flyingMultiplier = true;
            _swimmingMultiplier = true;
            _sprintingMultiplier = true;
            _crouchedSprintMultiplier = true;
            _flyingSprintMultiplier = true;
            _swimmingSprintMultiplier = true;

            _allowVehicleUse = EInheritableBool.Inherited;
            _allowGunPickup = EInheritableBool.Inherited;
            _allowMeleeWeaponPickup = EInheritableBool.Inherited;
            _allowUpgradePickup = EInheritableBool.Inherited;
            _allowHealthPickup = EInheritableBool.Inherited;
            _allowToolPickup = EInheritableBool.Inherited;
            _allowMagicPickup = EInheritableBool.Inherited;
            _allowStageInteraction = EInheritableBool.Inherited;

            _shieldsEnabled = EInheritableBool.Inherited;
            _shieldStrengthMultiplier = true;
            _shieldRecoveryEnabled = EInheritableBool.Inherited;
            _shieldsFixable = EInheritableBool.Inherited;
            _shieldFixWaitSeconds = true;
            _shieldRecoveryWaitSeconds = true;
            _shieldRecoverySeconds = true;
            _shieldBarDecaySeconds = true;
            _overshieldCount = true;
            _overshieldsRegenerate = EInheritableBool.Inherited;

            _deathWaitSeconds = true;
            _waitAmountPerBetrayal = true;

            _radarsEnabled = EInheritableBool.Inherited;
            _radarDistance = true;
            _enemyWaypoints = EInheritableBool.Inherited;
            _allyWaypoints = EInheritableBool.Inherited;
        }
        public void InitDefaultNonInherited()
        {
            _allowCrouching = EInheritableBool.True;
            _allowFlying = EInheritableBool.True;
            _allowSwimming = EInheritableBool.True;
            _allowSprinting = EInheritableBool.True;
            _allowSprintingWhileCrouched = EInheritableBool.True;
            _allowSprintingWhileFlying = EInheritableBool.True;
            _allowSprintingWhileSwimming = EInheritableBool.True;

            _walkingMultiplier = 1.0f;
            _crouchedMovementMultiplier = 1.0f;
            _flyingMultiplier = 1.0f;
            _swimmingMultiplier = 1.0f;
            _sprintingMultiplier = 1.0f;
            _crouchedSprintMultiplier = 1.0f;
            _flyingSprintMultiplier = 1.0f;
            _swimmingSprintMultiplier = 1.0f;

            _allowVehicleUse = EInheritableBool.True;
            _allowGunPickup = EInheritableBool.True;
            _allowMeleeWeaponPickup = EInheritableBool.True;
            _allowUpgradePickup = EInheritableBool.True;
            _allowHealthPickup = EInheritableBool.True;
            _allowToolPickup = EInheritableBool.True;
            _allowMagicPickup = EInheritableBool.True;
            _allowStageInteraction = EInheritableBool.True;

            _shieldsEnabled = EInheritableBool.True;
            _shieldStrengthMultiplier = 1.0f;
            _shieldRecoveryEnabled = EInheritableBool.True;
            _shieldsFixable = EInheritableBool.True;
            _shieldFixWaitSeconds = 6.0f;
            _shieldRecoveryWaitSeconds = 6.0f;
            _shieldRecoverySeconds = 3.0f;
            _shieldBarDecaySeconds = 0.0f;
            _overshieldCount = 0;
            _overshieldsRegenerate = EInheritableBool.False;

            _deathWaitSeconds = 3.0f;
            _waitAmountPerBetrayal = 3.0f;

            _radarsEnabled = EInheritableBool.True;
            _radarDistance = 100.0f;
            _enemyWaypoints = EInheritableBool.False;
            _allyWaypoints = EInheritableBool.True;
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
                if (fieldValueChild is EInheritableBool b)
                    field.SetValue(newTraits, b == EInheritableBool.Inherited ? fieldValueParent : fieldValueChild);
            }
            return newTraits;
        }
    }
}
