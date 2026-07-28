using Kryz.Stats;
using System;
using UnityEngine;

[System.Serializable]
public class ActorStats
{
    public enum ActorStatTypes { None = 0, MaxHealth = 10, MoveSpeed = 20, DodgeRate = 30, Defense = 40, Evasion = 50 }

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

            // Clamp
            //_health = (int)Mathf.Clamp(_health, 0, HealthMax.Value);

            Debug.Log("Health Changed??");
            OnHealthChanged?.Invoke(_health, (_health - oldHealth));
        }
    }

    public int Str;
    public int Dex;
    public int Int;

    public Stat HealthMax = new Stat("Max Health", 0, true, unit: "hp");
    public Stat MoveSpeed = new Stat("Move Speed", 0, unit: "m/s");
    public Stat DodgeRate = new Stat("Dodge Rate", 0, positiveDir: -1, unit: "s");
    public Stat Defense = new Stat("Defense", 0);
    public Stat Evasion = new Stat("Evasion", 0);

    /// <summary>
    /// ?.Invoke(newHealth, delta)
    /// </summary>
    public Action<float, float> OnHealthChanged;

    public void SetHealthPercent(float newPercent) 
    {
        //Debug.Log($"SetHealthPercent({newPercent}) => Health = {HealthMax.Value} * {newPercent}");
        Health = (int)((int)HealthMax.Value * newPercent); 
    }
    public float HealthPercent => (float)Health / (int)HealthMax.Value;

    public void AddArchetypeStats(int str, int dex, int intel)
    {
        this.Str += str;
        this.Dex += dex;
        this.Int += intel;
    }

    public Stat GetStat(ActorStatTypes actorStatName)
    {
        switch (actorStatName)
        {
            case ActorStatTypes.MaxHealth:
                return HealthMax;
            case ActorStatTypes.MoveSpeed:
                return MoveSpeed;
            case ActorStatTypes.DodgeRate:
                return DodgeRate;
            case ActorStatTypes.Defense:
                return Defense;
            case ActorStatTypes.Evasion:
                return Evasion;
            default:
                return null;
        }
    }
}
