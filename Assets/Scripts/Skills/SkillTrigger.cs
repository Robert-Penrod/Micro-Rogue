using System;
using UnityEngine;

public class SkillTrigger : MonoBehaviour
{
    public Action OnTrigger;

    protected Skill _skill;

    private void Awake()
    {
        _skill = GetComponent<Skill>();
    }
}
