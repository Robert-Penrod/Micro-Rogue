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
        float dropCount = Random.Range(0, 3) + _actor.GetLevel() / 2;
        if(DungeonManager.I.Data.IsBoss || DungeonManager.I.Data.IsElite) dropCount++;
        dropCount *= DungeonManager.I.Data.IsElite ? 2f : 1f;
        dropCount *= DungeonManager.I.Data.IsBoss ? 2f : 1f;

        //dropCount = (int)(dropCount * Random.Range(0.5f, 2f));

        //Debug.Log($"Spawning {dropCount} loot");

        for(int i = 0; i < dropCount; i++)
        {
            DoSpawn();
        }
    }

    void DoSpawn()
    {
        var selectedItem = ((DungeonManager.I.Data.IsBoss && Random.value < 0.4) || (DungeonManager.I.Data.IsElite && Random.value < 0.2))? GemPrefab : LootPrefabTable.SelectItem();
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
