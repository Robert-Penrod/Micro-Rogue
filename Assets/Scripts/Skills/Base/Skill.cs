using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public Actor Actor;

    public SkillStats Stats;

    public float CooldownPercent { get; private set; }

    private void OnEnable()
    {
        Actor = GetComponentInParent<Actor>();
        ShuffleCooldown();
    }

    private void FixedUpdate()
    {
        // Temp Stats
        float skillSpeed = 1f;
        float cooldown = Constants.SkillStats.Cooldown.Default;

        // Cooldown
        if(CooldownPercent < 1f)
        {
            CooldownPercent += (skillSpeed / cooldown) * Time.fixedDeltaTime;
            CooldownPercent = CooldownPercent.ClampMax(1f);
        }

        // No Targets
        if(Actor.Senses.EnemyActors.Count <= 0)
        {
            CooldownPercent = CooldownPercent.ClampMax(0.75f);
        }
    }

    public void ResetCooldown()
    {
        CooldownPercent = 0f;
        CooldownPercent += 0.1f * Random.Range(-1f, 1f);
    }

    public void ShuffleCooldown()
    {
        CooldownPercent = Random.Range(0f, 0.5f);
    }
}
