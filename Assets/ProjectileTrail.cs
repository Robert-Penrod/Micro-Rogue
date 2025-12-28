using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class ProjectileTrail : MonoBehaviour
{
    TrailRenderer _trailRend;
    SkillInstance _skillInstance;

    private void Awake()
    {
        _trailRend = GetComponent<TrailRenderer>();
        _skillInstance = GetComponentInParent<SkillInstance>();

        _skillInstance.OnActivated += () =>
        {
            _trailRend.startColor = _trailRend.endColor = GamePaletteManager.I.Palette.GetActorSkillColor(_skillInstance.Skill).Alpha(0f);
            _trailRend.emitting = true;
        };

        _skillInstance.OnEnd += () =>
        {
            _trailRend.emitting = false;
        };
    }
}
