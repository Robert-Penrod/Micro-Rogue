using ManaSprite.EasyPooling;
using System.Collections;
using UnityEngine;

public class SE_Spawn : SkillEffect
{
    public GameObject PrefabToSpawn;

    public override void Effect()
    {
        int amount = (int)_skill.Stats.Count.Value;
        float spawnDelay = 0.9f * _skill.TelegraphTime;

        StartCoroutine(Spawn_Co());
        IEnumerator Spawn_Co()
        {
            for (int i = 0; i < amount; i++)
            {
                DoSpawn();
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    void DoSpawn()
    {
        if (PrefabToSpawn == null) return;

        Vector3 spawnPos = transform.position;
        spawnPos.z = PrefabToSpawn.transform.position.z;

        GameObject newObj = PrefabToSpawn.PooledInstantiate();
        newObj.transform.position = transform.position;

        // Skill Instance
        var skillInstance = newObj.GetComponent<SkillInstance>();
        if(skillInstance)
        {
            skillInstance.Link(_skill);
        }

        newObj.SetActive(true);
    }
}
