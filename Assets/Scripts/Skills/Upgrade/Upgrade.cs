using Kryz.Stats;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Upgrade
{
    // Fields
    [SerializeField] protected string _name;
    [SerializeField] protected Constants.Rarity _rarity;
    protected Sprite _icon;
    protected string _description;

    // Apply
    public abstract void ApplyUpgrade();

    // Getters
    public virtual string GetTitle() => _name;
    public virtual string GetLevel() => string.Empty;
    public virtual Sprite GetIcon() => _icon;
    public virtual string GetDescription() => _description;
    public virtual Color GetColor() => Color.white;
    public virtual string GetSlot() => string.Empty;
}
