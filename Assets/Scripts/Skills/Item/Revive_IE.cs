using UnityEngine;

public class Revive_IE : ItemEffect
{
    public float HealPercent = 1f;
    public bool IsUsed;

    private void OnEnable()
    {
        _skill.Actor.OnPreDeath += () =>
        {
            Debug.Log("Pre DEath!");
            if (!IsUsed)
            {
                Debug.Log("Revive!");
                IsUsed = true;
                _skill.Actor.Stats.SetHealthPercent(HealPercent);
            }
        };
    }
}
