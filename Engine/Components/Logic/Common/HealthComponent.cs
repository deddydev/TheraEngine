using System;

namespace TheraEngine.Components.Logic.Common
{
    public class HealthComponent : LogicComponent
    {
        public event Action Died;
        public event Action Revived;
        public event DelHealthChange HealthRestored;
        public event DelHealthChange Damaged;

        public delegate void DelHealthChange(float amount);

        public float MaxHealth { get; set; }
        public float CurrentHealth { get; set; }
    }
}