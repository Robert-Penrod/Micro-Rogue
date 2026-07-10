using ManaSprite.EasyPooling;
using System.Collections;
using UnityEngine;

public class SE_Spawn : SkillEffect
{
    public GameObject PrefabToSpawn;
    [SerializeField] float _randPos = 0f;
    [SerializeField] bool _randRot = false;

    public override void TriggerEffect()
    {
        int amount = (int)_skill.Stats.Count.Value;

        StartCoroutine(Spawn_Co());
        IEnumerator Spawn_Co()
        {
            for (int i = 0; i < amount; i++)
            {
                DoSpawn();
                yield return new WaitForSeconds(_skill.SpawnDelayTime);
            }
        }
    }

    void DoSpawn()
    {
        if (PrefabToSpawn == null) return;

        // Random
        Random.InitState(System.DateTime.Now.Ticks.GetHashCode());

        // Spawn
        GameObject newObj = PrefabToSpawn.PooledInstantiate();

        // Position
        Vector3 spawnPos = transform.position;
        spawnPos.z = PrefabToSpawn.transform.position.z;
        if(_randPos > 0f) spawnPos += (Vector3)Random.insideUnitCircle * _randPos;
        newObj.transform.position = spawnPos;

        // Rotation
        if(_randRot) newObj.transform.rotation = Quaternion.Euler(0f, 0f, 360f * Random.Range(0f, 1f));

        // Skill Instance
        var skillInstance = newObj.GetComponent<SkillInstance>();
        if(skillInstance)
        {
            skillInstance.Link(_skill);
            skillInstance.ParentBody = _skill?.Actor?.Body;
        }

        //Debug.Log("SPAWNED");

        newObj.SetActive(true);
    }
}
