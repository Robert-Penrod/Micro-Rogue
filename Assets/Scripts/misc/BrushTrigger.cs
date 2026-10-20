using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BrushTrigger : MonoBehaviour
{
    public float Vol = 0.125f;
    public AudioClip Sound;
    public float LerpSpeed = 10f;
    public float MinSpeed = 3f;
    public float HitboxDelay = 0.25f;
    float _hitboxTick = 0f;
    [Range(0f, 1f)] public float HitboxVariance = 0f;
    public float RotationAmp = 5f;
    public float PosAmp = 0.025f;
    float _targetRotationAngle = 0f;
    float _initRotationAngle = 0f;

    Vector3 _targetLocalPos = Vector3.zero;

    public UnityEvent OnTrigger;

    public WeightedList<GameObject> LootTable;
    int LootCount;
    float _lootTick;
    string _seed;

    float _velMult = 1f;

    private void OnEnable()
    {
        Init();
    }

    void Init()
    {
        _seed = ((int)transform.position.x).ToString() + "," + ((int)transform.position.y).ToString();
        //Random.InitState(_seed.GetHashCode());
        LootCount = 0;
        if (Random.value < 0.375f)
        {
            LootCount++;
            if (Random.value < 0.375f)
            {
                LootCount++;
                if (Random.value < 0.375f)
                {
                    LootCount++;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_hitboxTick > 0)
        {
            return;
        }

        Rigidbody2D collidingBody = collision.GetComponent<Rigidbody2D>();
        if (collidingBody == null) return;

        float speed = collidingBody.linearVelocity.magnitude;
        if (speed > MinSpeed)
        {
            Trigger(collision.gameObject);
        }
        _velMult = speed.Remap(MinSpeed, 10f, 1f, 2f);
    }

    private void Awake()
    {
        _initRotationAngle = transform.rotation.eulerAngles.z;
        _targetRotationAngle = _initRotationAngle;
    }

    private void Update()
    {
        if (_hitboxTick > 0)
        {
            _hitboxTick -= Time.deltaTime;
        }

        Lerp();
    }

    void Lerp()
    {
        float currentAngle = transform.rotation.eulerAngles.z;
        float lerpAngle = Mathf.LerpAngle(currentAngle, _targetRotationAngle, LerpSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, lerpAngle);

        Transform gfxHolder = transform.GetChild(0);
        Vector3 currentLocalPos = gfxHolder.transform.localPosition;
        Vector2 lerpPos = Vector2.Lerp(currentLocalPos, _targetLocalPos, LerpSpeed * Time.deltaTime);
        gfxHolder.localPosition = (Vector3)lerpPos + Vector3.forward * gfxHolder.localPosition.z;
    }

    int _rotDir = 1;

    void Trigger(GameObject triggeringObject)
    {
        _hitboxTick = HitboxDelay + HitboxVariance * Random.Range(-1f, 1f) * HitboxDelay;

        float vol = 0.25f; // 0.25f * Vol
        AudioSpawner.PlayAudioWithRandPitch(Sound, 0.2f, 1f, vol, (Vector2)transform.position).spatialBlend = 0.6f;

        _targetRotationAngle = _initRotationAngle + Random.value.RemapPercent(0.5f * RotationAmp, RotationAmp) * _rotDir;
        _rotDir *= -1;
        _targetLocalPos = Random.insideUnitCircle * PosAmp * transform.localScale.x;

        if (triggeringObject.CompareTag("Player"))
        {
            LootCheck();
        }

        OnTrigger?.Invoke();
    }

    void LootCheck()
    {
        if (LootTable.Entries.Count <= 0) return;
        if (LootCount <= 0) return;

        _lootTick += Random.Range(0.375f, 1.5f);
        while(_lootTick >= 1f && LootCount > 0)
        {
            _lootTick -= 1f;
            SpawnLoot();
        }
    }

    void SpawnLoot()
    {
        LootCount--;
        var spawnedLoot = Instantiate(LootTable.SelectItem(), transform.position, Quaternion.identity);
        Loot.KickBody(spawnedLoot.GetComponent<Rigidbody2D>(), 2f, 2f);
    }
}
