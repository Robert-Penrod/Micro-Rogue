using Kryz.Stats;
using System;
using UnityEngine;

[System.Serializable]
public class ActorStats
{
    [SerializeField] int _health;
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            if (value == _health) return;
            int oldHealth = _health;
            _health = value;
            _health = (int)Mathf.Clamp(_health, 0, HealthMax.Value);
            OnHealthChanged?.Invoke( _health, (_health - oldHealth));
        }
    }

    public Stat HealthMax = new Stat(Constants.ActorStats.Health.Default, "Health Max", true, unit: "hp");
    public Stat MoveSpeed = new Stat(Constants.ActorStats.MoveSpeed.Default, "Move Speed", unit: "m/s");
    public Stat DodgeCooldown = new Stat(Constants.ActorStats.DodgeCooldown.Default, "Dodge Cooldown", positiveDir: -1, unit: "s");

    /// <summary>
    /// ?.Invoke(newHealth, delta)
    /// </summary>
    public Action<float, float> OnHealthChanged;

    public void SetHealthPercent(float newPercent) { Health = (int)(HealthMax.Value * newPercent); }
    public float HealthPercent => (float)Health / HealthMax.Value;
}
