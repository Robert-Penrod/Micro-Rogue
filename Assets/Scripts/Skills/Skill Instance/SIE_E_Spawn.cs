using ManaSprite.EasyPooling;
using UnityEngine;

public class SIE_E_Spawn : SIE, IPoolable
{
    [SerializeField] GameObject _spawnPrefab;
    bool _didSpawn;

    float _speed => _skillInstance.Skill.Stats.Speed.Value;

    public void Initialize()
    {
        _didSpawn = false;
    }

    private void Update()
    {
        if (_skillInstance.State != SkillInstance.SkillInstanceState.End) return;
        if (_didSpawn) return;

        _didSpawn = true;
        DoSpawn();
    }

    void DoSpawn()
    {
        var spawn = _spawnPrefab.PooledInstantiate(null, transform.position + transform.up * 0.25f);
        spawn.SetActive(true);

        var skillPart = spawn.GetComponent<SkillPart>();
        if(skillPart != null)
        {
            skillPart.SetSourceSkillInstance(_skillInstance);
        }

        var spawnBody = spawn.GetComponent<Rigidbody2D>();
        if (spawnBody != null)
        {
            spawnBody.linearVelocity = 0.25f * _speed * transform.up;
        }
    }
}
