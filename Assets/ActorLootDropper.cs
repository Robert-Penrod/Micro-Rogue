using ManaSprite.EasyPooling;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class ActorLootDropper : MonoBehaviour
{
    [SerializeField] float _kickForce = 1.5f;
    [SerializeField] float _angularForce = 1f;
    public WeightedList<GameObject> LootPrefabTable = new();

    Actor _actor;

    private void Awake()
    {
        _actor = GetComponent<Actor>();
        _actor.OnDeath += DropLoot;
    }

    void DropLoot()
    {
        int dropCount = 1 + _actor.GetLevel();

        //dropCount = (int)(dropCount * Random.Range(0.5f, 2f));

        Debug.Log($"Spawning {dropCount} loot");

        for(int i = 0; i < dropCount; i++)
        {
            DoSpawn();
        }
    }

    void DoSpawn()
    {
        var selectedItem = LootPrefabTable.SelectItem();
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
