using ManaSprite.EasyPooling;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplosion : SkillPart, IPoolable
{
    [SerializeField] AudioClip _damageAudio;
    [SerializeField] float _damageMult = 0.5f;
    [SerializeField] float _sizeMult = 1f;
    [SerializeField] float _explosionTime = 1f;
    float _explosionTick;

    [SerializeField] ParticleSystem _pSystem;
    float _radius => transform.localScale.x;

    float _damage => _damageMult * _sourceSkill.Stats.Damage.Value;
    float _knockback => 2f;
    float _size => _sourceSkill.Stats.Size.Value;

    List<Collider2D> _colCache = new();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private void OnEnable()
    {
        var main = _pSystem.main;
        main.startLifetime = _explosionTime;

        this.DelayedInvoke(0.1f, () =>
        {
            DoExplosion();
        });
    }

    public override void SetSourceSkill(Skill sourceSkill)
    {
        base.SetSourceSkill(sourceSkill);
        var main = _pSystem.main;
        main.startColor = GamePaletteManager.I.Palette.GetActorSkillColor(_sourceSkill).Alpha(main.startColor.color.a);
        transform.localScale = _sizeMult * _size * Vector3.one;
    }

    private void Update()
    {
        _explosionTick += Time.deltaTime;

        if(!_pSystem.isPlaying)
        {
            this.gameObject.DestroyOrRecycle();
        }
    }

    void DoExplosion()
    {
        /*
        Collider2D[] colArray = Physics2D.OverlapCircleAll(transform.position, _radius);
        foreach(Collider2D col in colArray)
        {
            HandleCollision(col);
        }
        */
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_explosionTick / _explosionTime) > 0.25f) return;

        if (_colCache.Contains(collision)) return;
        _colCache.Add(collision);
        HandleCollision(collision);
    }

    void HandleCollision(Collider2D col)
    {
        float dist = Vector2.Distance(col.transform.position, this.transform.position);

        // Knockback
        var body = col.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            Vector2 dir = body.transform.position - transform.position;
            Vector2 knockbackVector = _size * _knockback * dir.normalized;// * dist.Remap(0f, _radius, 1f, 0f);
            body.AddDampForce(knockbackVector, ForceMode2D.Impulse);
        }

        //  Damage
        var hitActor = col.GetComponent<Actor>();
        if (hitActor != null)
        {
            if (!_sourceSkill.Actor.IsEnemyOf(hitActor)) return;

            int damageTaken = hitActor.TakeDamage((int)_damage);

            if (damageTaken == 0) return;

            // Popup
            string colorString = "#" + ColorUtility.ToHtmlStringRGB(GamePaletteManager.I.Palette.GetActorSkillColor(_sourceSkill).Lerp(Color.white, 0.25f));// hitActor.Faction == Actor.FactionType.Player ? "#FF9900" : "#FFFFFF";
            string popupString = "<color=" + colorString + ">-" + damageTaken.ToString() + "</color>";
            Vector3 popupPos = Vector2.Lerp(transform.position, hitActor.transform.position, hitActor.IsAlive ? 0.5f : 1f);
            popupPos += 0.25f * (Vector3)Random.insideUnitCircle;
            TextPopup2DManager.I.CreatePopup(popupPos, popupString, null, hitActor.IsAlive ? hitActor.transform : null);

            // Screen Shake
            CamShaker.Instance.Shake(Random.Range(0.2f, 0.3f), Random.Range(2.5f, 3.5f));

            // Audio
            float audioDelay = 0.125f * Random.Range(0.75f, 1.25f);
            AudioSpawner.PlayAudioWithRandPitch(_damageAudio, 0.2f, 1f, 1f, delay: audioDelay);
        }
    }

    public void Initialize()
    {
        _colCache.Clear();
    }
}
