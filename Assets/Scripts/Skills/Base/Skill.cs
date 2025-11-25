using UnityEngine;

public class Skill : MonoBehaviour
{
    public SkillStats Stats;

    public float CooldownPercent { get; private set; }

    private void FixedUpdate()
    {
        float skillSpeed = 1f;
        float cooldown = Constants.SkillStats.Cooldown.Default;

        if(CooldownPercent < 1f)
        {
            CooldownPercent += (skillSpeed / cooldown) * Time.fixedDeltaTime;
            CooldownPercent = Mathf.Clamp01(CooldownPercent);
        }
    }

    public void ResetCooldown()
    {
        CooldownPercent = 0f;
    }

    public void ShuffleCooldown()
    {
        CooldownPercent = Random.Range(0f, 0.5f);
    }
}
