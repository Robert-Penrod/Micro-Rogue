using UnityEngine;

public class SE_Regen : SkillEffect
{
    private void Update()
    {
        if(_skill.CooldownPercent >= 1f)
        {
            var actor = _skill.Actor;
            if(actor.Stats.Health < actor.Stats.HealthMax.Value)
            {
                int heal = (int)(_skill.Actor.Stats.HealthMax.Value * _skill.Stats.Potency.Value);
                _skill.Actor.Heal(heal, null, _skill.Actor);
            }
            _skill.ResetCooldown();
        }
    }
}
