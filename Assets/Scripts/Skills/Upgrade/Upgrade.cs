using Kryz.Stats;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Upgrade
{
    // Fields
    [SerializeField] protected string _name;
    public Constants.Rarity Rarity;
    protected Sprite _icon;
    protected string _description;

    // Reference
    [HideInInspector] public Skill SourceSkill;

    // Apply
    public abstract void ApplyUpgrade();

    // Getters
    public virtual string GetTitle() => _name;
    public virtual Sprite GetIcon() => _icon;
    public virtual string GetDescription() => _description;
    public virtual Color GetColor() => Color.white;
    public virtual string GetSlot() => string.Empty;
    public virtual string GetLevel() => (SourceSkill != null && SourceSkill.Level > 0)? $"Lvl {SourceSkill.Level + 1}" : "New";
}
