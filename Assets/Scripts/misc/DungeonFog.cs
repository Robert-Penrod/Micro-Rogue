using UnityEngine;

public class DungeonFog : MonoBehaviour
{
    SpriteRenderer _spriteRend;
    float _initSize;
    float _dungeonScaleMult = 1f;

    private void Awake()
    {
        _spriteRend = GetComponent<SpriteRenderer>();
        _spriteRend.enabled = false;
        _initSize = transform.localScale.x;
    }

    private void Start()
    {
        _spriteRend.enabled = true;

        DungeonManager.I.OnDungeonDataChanged += () =>
        {
            //_spriteRend.enabled = DungeonManager.I.Data.Coordinate.y > 0;
            Random.InitState(DungeonManager.I.Data.GetSeed());
            _dungeonScaleMult = Random.Range(0.8f, 1f);
        };
    }

    private void Update()
    {
        // Target Size
        float targetSize = _dungeonScaleMult * _initSize;

        // Upgrade Menu
        if (UpgradeMenu.I.IsOpen) targetSize *= 1.25f;

        // Underground
        if (DungeonManager.I?.Data?.Biome == DungeonManager.BiomeEnum.Underground) targetSize *= 0.75f;

        // Hub
        if(DungeonManager.I.Data.Coordinate.y <= 0)
        {
            targetSize *= 1.25f;
        }

        // Lerp Scale
        float lerpSize = transform.localScale.x.Lerp(targetSize, 6f * Time.deltaTime);
        transform.localScale = lerpSize * Vector3.one;
    }
}
