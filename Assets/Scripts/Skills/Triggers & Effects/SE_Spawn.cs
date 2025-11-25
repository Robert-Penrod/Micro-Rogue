using HyperQuest.EasyPooling;
using System.Collections;
using UnityEngine;

public class SE_Spawn : SkillEffect
{
    public GameObject PrefabToSpawn;

    public override void Effect()
    {
        int amount = 1;
        float projectileDelay = 0.25f;

        StartCoroutine(Spawn_Co());
        IEnumerator Spawn_Co()
        {
            for(int i = 0; i < amount; i++)
            {
                DoSpawn();
                yield return new WaitForSeconds(projectileDelay);
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
            Debug.Log("Setting [Skill Instance] [Skill] Reference");
            skillInstance.Skill = _skill;
        }

        newObj.SetActive(true);
    }
}
