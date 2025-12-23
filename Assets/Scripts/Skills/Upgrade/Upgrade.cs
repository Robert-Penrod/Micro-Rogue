using Kryz.Stats;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Upgrade
{
    // Fields
    [SerializeField] protected string _name;
    [SerializeField] protected Sprite _icon;
    [SerializeField] protected string _description;

    // Apply
    public abstract void ApplyUpgrade();

    // Getters
    public virtual string GetTitle() => _name;
    public virtual string GetLevel() => string.Empty;
    public virtual Sprite GetIcon() => _icon;
    public virtual string GetDescription() => _description;
    public virtual Color GetColor() => Color.white;
}
