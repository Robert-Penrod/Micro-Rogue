using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GemToCoinInteractable : InteractableTrigger
{
    [SerializeField] int _gemCost = 2;
    [SerializeField] TextMeshPro _tmPro;
    public GameObject _tradeSpawn;
    [SerializeField] float _maxForce = 3f;
    [SerializeField] float _minForce = 1f;

    private void OnValidate()
    {
        _tmPro.text = _gemCost.ToString();
    }

    protected override void Interact(Player player)
    {
        if (Player.PlayerData.Gems >= _gemCost)
        {
            Player.PlayerData.Gems -= _gemCost;
            var spawnObj = Instantiate(_tradeSpawn, transform.position, Quaternion.identity);
            var spawnBody = spawnObj.GetComponent<Rigidbody2D>();
            if (spawnBody != null)
            {
                Utils.RandomSeed();
                spawnBody.AddDampForce(Random.insideUnitCircle.normalized * Random.Range(_minForce, _maxForce), ForceMode2D.Impulse);
            }
        }
    }

    protected override bool CanInteract(Player player)
    {
        return Player.PlayerData.Gems >= _gemCost;
    }
}
