using UnityEngine;

public class SI_SwingPivot : MonoBehaviour
{
    float _targetAngle;
    [SerializeField] float _swingAngle = 45f;

    SkillInstance _skillInstance;

    int _dir = 1;

    private void Awake()
    {
        _skillInstance = GetComponentInParent<SkillInstance>();
    }

    private void OnEnable()
    {
        if (_swingAngle.Abs() > 0)
        {
            _dir = _skillInstance.Skill.GetDirection(true);
        }
        else
        {
            this.enabled = false;
        }
    }

    private void Update()
    {
        var durration = _skillInstance.Skill.Stats.Duration;
        float swingAngle = _dir * _swingAngle * durration.Value.Remap(durration.BaseValue, 2f * durration.BaseValue, 1f, 2.5f, false);

        // Angle Bonus
        var skillStats = _skillInstance.Skill.Stats;
        swingAngle *= skillStats.Duration.Value.Remap(skillStats.Duration.BaseValue, 2f * skillStats.Duration.BaseValue, 1f, 2f);
        swingAngle *= skillStats.Speed.Value.Remap(skillStats.Speed.BaseValue, 2f * skillStats.Speed.BaseValue, 1f, 2f);

        switch (_skillInstance.State)
        {
            case SkillInstance.SkillInstanceState.Start:
                _targetAngle = _skillInstance.StartPercent.Remap(0.25f, 1f, 0f, swingAngle);
                break;
            case SkillInstance.SkillInstanceState.Activated:
                _targetAngle = _skillInstance.ActivePercent.RemapPercent(swingAngle, -swingAngle);
                break;
            case SkillInstance.SkillInstanceState.End:
                break;
        }

        LerpAngle(_targetAngle, 25f);
    }

    void LerpAngle(float targetAngle, float speed)
    {
        float lerpAngle = Mathf.LerpAngle(transform.localRotation.eulerAngles.z, targetAngle, speed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(0f, 0f, lerpAngle);
        float angleDelta = targetAngle - transform.localRotation.eulerAngles.z;
        transform.localScale = new Vector3(_dir, 1f, 1f);
    }
}
