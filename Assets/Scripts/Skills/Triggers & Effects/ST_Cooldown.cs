using UnityEngine;

public class ST_Cooldown : SkillTrigger
{
    public bool ResetsCooldownOnTrigger = true;

    private void Start()
    {
        _skill.OnCooldown += () =>
        {
            DoTrigger();
        };
    }

    bool DoTrigger()
    {
        bool succesfulTrigger = true;
        OnTrigger?.Invoke();

        if (succesfulTrigger && ResetsCooldownOnTrigger)
        {
            _skill.ResetCooldown();
        }

        return succesfulTrigger;
    }
}
