using ManaSprite.EasyPooling;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class ActorLootDropper : MonoBehaviour
{
    [SerializeField] float _kickForce = 1.5f;
    [SerializeField] float _angularForce = 1f;
    public WeightedList<GameObject> LootPrefabTable = new();
    public GameObject GemPrefab;

    Actor _actor;

    private void Awake()
    {
        _actor = GetComponent<Actor>();
        _actor.OnDeath += DropLoot;
    }

    void DropLoot()
    {
        float dropCount = Random.Range(1, 4) + _actor.GetLevel() / 2;
        if (DungeonManager.I.Data.IsBoss) dropCount++;
        dropCount += DungeonManager.I.Data.EliteTier;
        /*
        dropCount += DungeonManager.I.Data.EliteTier;
        dropCount *= DungeonManager.I.Data.EliteTier.Remap(0f, 2f, 1f, 2f);
        dropCount *= DungeonManager.I.Data.IsBoss ? 2f : 1f;
        dropCount *= DungeonManager.I.Data.RunTier.Remap(1f, 3f, 1f, 3f);
        */
        //dropCount = (int)(dropCount * Random.Range(0.5f, 2f));

        //Debug.Log($"Spawning {dropCount} loot");

        for (int i = 0; i < dropCount; i++)
        {
            DoSpawn();
        }
    }

    void DoSpawn()
    {
        var selectedItem = ((DungeonManager.I.Data.IsBoss && Random.value < 0.5f) || (Random.value < 0f * DungeonManager.I.Data.EliteTier))? GemPrefab : LootPrefabTable.SelectItem();
        if (selectedItem == null) return;
        var lootSpawn = selectedItem.PooledInstantiate(transform.position);
        lootSpawn.gameObject.SetActive(true);

        var lootBody = lootSpawn.GetComponent<Rigidbody2D>();
        if(lootBody != null)
        {
            lootBody.AddForce(Random.insideUnitCircle * _kickForce * lootBody.mass * lootBody.linearDamping, ForceMode2D.Impulse);
            lootBody.AddTorque(Random.Range(01f, 1f) * _angularForce);
        }
    }
}
