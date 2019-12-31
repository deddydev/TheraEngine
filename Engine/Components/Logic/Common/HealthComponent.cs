using System;

namespace TheraEngine.Components.Logic.Common
{
    public class HealthComponent : LogicComponent
    {
        private float _currentHealth;
        private bool _isDead = false;

        public event Action Died;
        public event Action Revived;
        public event DelHealthChange HealthChanged;

        public bool IsDead
        {
            get => _isDead;
            set
            {
                if (_isDead == value)
                    return;
                _isDead = value;
                if (_isDead)
                    OnDied();
                else
                    OnRevived();
            }
        }

        protected void OnHealthChanged(float amount)
            => HealthChanged?.Invoke(amount);
        protected void OnDied()
            => Died?.Invoke();
        protected void OnRevived()
            => Revived?.Invoke();

        public delegate void DelHealthChange(float amount);

        public float MaxHealth { get; set; }
        public float CurrentHealth
        {
            get => _currentHealth;
            set
            {
                float delta = value - _currentHealth;
                bool nowDead = value <= 0.0f;
                _currentHealth = value;

                OnHealthChanged(delta);
                if (nowDead)
                {
                    CurrentHealth = 0.0f;
                    IsDead = true;
                }
                else if (IsDead && CurrentHealth > 0.0f)
                {
                    IsDead = false;
                }
            }
        }

        public void ScaleMaxHealth(float newHealth)
        {
            float ratio = newHealth / MaxHealth;
            CurrentHealth *= ratio;
            MaxHealth = newHealth;
        }
    }
}