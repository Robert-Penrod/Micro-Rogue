using HyperQuest.EasyPooling;
using UnityEngine;

public class SIE_S_Telegraph : SIE, IPoolable
{
    [Header("Config")]
    [SerializeField] bool _isHeld = true;

    float _telegraphPercent;

    float _cooldownTime => _skillInstance.Skill.Stats.Cooldown;
    float _size => _skillInstance.Skill.Stats.Size;
    float _telegraphTime => (0.2f + _cooldownTime * 0.2f);

    ColorController _colorController;

    MomentumTrackerEquipmentAnimator _momentumEquipAnim;

    protected override void Awake()
    {
        base.Awake();
        _colorController = GetComponentInParent<ColorController>();

        if (_isHeld)
        {
            _momentumEquipAnim = gameObject.GetOrAddComponent<MomentumTrackerEquipmentAnimator>();
            _skillInstance.OnActivated += () =>
            {
                if (_momentumEquipAnim != null) _momentumEquipAnim.enabled = false;
            };
        }
    }

    public void Initialize()
    {
        _telegraphPercent = 0f;
        transform.localScale = Vector3.zero;

        if (_isHeld)
        {
            _momentumEquipAnim.enabled = true;
            float z = transform.position.z;
            var skill = _skillInstance.Skill;
            var actor = skill.GetComponentInParent<Actor>();
            Debug.Log("Setting Hold");
            transform.SetParent(actor.transform, true);
            transform.localPosition = 0.35f * (Vector3)Random.insideUnitCircle;
            transform.position = new Vector3(transform.position.x, transform.position.y, -15f);
        }
    }

    private void Update()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Start) return;

        // Tick
        _telegraphPercent += Time.deltaTime / _telegraphTime;

        // Alpha
        float alphaPercent = Mathf.Pow(_telegraphPercent, 1.2f);
        float alpha = alphaPercent;
        alpha *= _telegraphPercent.Remap(0.9f, 1f, 0.9f, 1f);
        _colorController.SetAlpha(0.5f * 0.9f * alpha);

        // Scale
        transform.localScale = Vector3.one * _telegraphPercent.RemapPercent(0f, _size);

        // Next
        if (_telegraphPercent >= 1) _skillInstance.State++;
    }
}
