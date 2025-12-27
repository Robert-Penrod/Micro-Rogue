using ManaSprite.EasyPooling;
using UnityEngine;

public class SIE_S_Telegraph : SIE, IPoolable
{
    [Header("Config")]
    [SerializeField] float _telegraphMult = 1f;
    [SerializeField] bool _isHeld = true;

    float _telegraphPercent;

    float _size => _skillInstance.Skill.Stats.Size.Value;
    float _telegraphTime => _telegraphMult * _skillInstance.Skill.TelegraphTime;

    ColorController _colorController;

    MomentumTrackerEquipmentAnimator _momentumEquipAnim;

    Vector3 _initScale;

    protected override void Awake()
    {
        _initScale = transform.localScale;
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
            transform.SetParent(actor.transform, true);
            transform.localPosition = 0.35f * (Vector3)Random.insideUnitCircle;
            transform.position = new Vector3(transform.position.x, transform.position.y, -15f);
        }
    }

    private void Update()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.Start) return;

        // Tick
        float dodgeMult = _skillInstance.Skill.Actor.MoveController.IsDodging ? 0f : 1f;
        _telegraphPercent += dodgeMult * Time.deltaTime / _telegraphTime;

        // Alpha
        float alphaPercent = Mathf.Pow(_telegraphPercent.Remap(0f, 0.5f, 0f, 1f), 1.25f);
        float alpha = alphaPercent;
        alpha *= _telegraphPercent.Remap(0.9f, 1f, 0.9f, 1f);
        _colorController.SetAlpha(0.9f * Constants.SkillStats.BaseAlpha * alpha);

        // Scale
        //transform.SetLossyScale(Vector3.one * _telegraphPercent.RemapPercent(0f, _size));
        transform.localScale = _initScale * _telegraphPercent.RemapPercent(0f, _size);

        // Next
        if (_telegraphPercent >= 1) _skillInstance.State++;
    }
}
