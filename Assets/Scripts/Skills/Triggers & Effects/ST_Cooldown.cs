using UnityEngine;

public class ST_Cooldown : SkillTrigger
{
    private void Update()
    {
        if(_skill.CooldownPercent >= 1f)
        {
            OnTrigger?.Invoke();
            _skill.ResetCooldown();
        }
    }
}
