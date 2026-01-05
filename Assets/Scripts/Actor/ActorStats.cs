using Kryz.Stats;
using System;
using UnityEngine;

[System.Serializable]
public class ActorStats
{
    public enum ActorStatTypes { None = 0, MaxHealth = 10, MoveSpeed = 20, DodgeCooldown = 30, Defense = 40, Evasion = 50}

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

    public int Str;
    public int Dex;
    public int Int;

    public Stat HealthMax = new Stat(0, "Health Max", true, unit: "hp");
    public Stat MoveSpeed = new Stat(0, "Move Speed", unit: "m/s");
    public Stat DodgeCooldown = new Stat(0, "Dodge Cooldown", positiveDir: -1, unit: "s");
    public Stat Defense = new Stat(0, "Defense");
    public Stat Evasion = new Stat(0, "Evasion");

    /// <summary>
    /// ?.Invoke(newHealth, delta)
    /// </summary>
    public Action<float, float> OnHealthChanged;

    public void SetHealthPercent(float newPercent) { Health = (int)(HealthMax.Value * newPercent); }
    public float HealthPercent => (float)Health / HealthMax.Value;

    public void AddArchetypeStats(int str, int dex, int intel)
    {
        this.Str += str;
        this.Dex += dex;
        this.Int += intel;
    }
}
