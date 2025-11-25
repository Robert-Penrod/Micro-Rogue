using UnityEngine;

public class Skill : MonoBehaviour
{
    public Actor Actor => GetComponentInParent<Actor>();

    public SkillStats Stats;

    public float CooldownPercent { get; private set; }

    private void OnEnable()
    {
        ShuffleCooldown();
    }

    private void FixedUpdate()
    {
        float skillSpeed = 1f;
        float cooldown = Constants.SkillStats.Cooldown.Default;

        if(CooldownPercent < 1f)
        {
            CooldownPercent += (skillSpeed / cooldown) * Time.fixedDeltaTime;
            CooldownPercent = CooldownPercent.ClampMax(1f);
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
