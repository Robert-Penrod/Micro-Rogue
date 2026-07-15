using System.Collections.Generic;
using UnityEngine;

public class Zoo_SkillViewBox : MonoBehaviour
{
    [Header("Params")]
    [SerializeReference] List<Skill> _skillToPreview;

    [Header("Reference")]
    [SerializeField] ActorSkillSystem _skillSystem;

    private void OnValidate()
    {
        this.gameObject.name = (_skillToPreview.Count > 0? _skillToPreview[0].name : string.Empty) + "_PreviewBox";
    }

    private void Start()
    {
        _skillSystem.RemoveAllSkills();
        foreach(var skill in _skillToPreview)
        {
            _skillSystem.AddSkill(skill);
        }
    }
}
