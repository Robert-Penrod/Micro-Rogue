using UnityEngine;

public class HitParticlesManager : PersistantSingleton<HitParticlesManager>
{
    [SerializeField] ParticleSystem _hitParticlePrefab;

    public void SpawnHitParticles(Skill skill, Vector2 pos, Vector2 vel, float magnitude = 1f)
    {
        magnitude = magnitude.ClampMin(0.5f);

        var pSystem = Instantiate(_hitParticlePrefab).GetComponent<ParticleSystem>();
        pSystem.transform.position = (Vector3)pos + Vector3.forward * _hitParticlePrefab.transform.position.z;
        var main = pSystem.main;
        main.startColor = GamePaletteManager.I.Palette.GetActorSkillColor(skill).Lerp(Color.white, 0.1f).Alpha(0.5f * magnitude);

        main.startSpeed = new ParticleSystem.MinMaxCurve(2.5f * magnitude, 5f * magnitude);

        var velOverLifetime = pSystem.velocityOverLifetime;
        velOverLifetime.enabled = true;
        velOverLifetime.x = vel.x;
        velOverLifetime.y = vel.y;

        var emission = pSystem.emission;
        emission.SetBurst(0, new ParticleSystem.Burst(0f, (short)(2 * magnitude), (short)(3 * magnitude)));

        pSystem.Play();
    }
}
