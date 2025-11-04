using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnOnDestroy : MonoBehaviour
{
    public float InheritVelocity;
    public Vector3 Offset;
    [SerializeField] List<GameObject> _objectsToSpawn = new List<GameObject>();
    [SerializeField] [Range(0f, 1f)] float _audioVolume = 1f;
    [SerializeField] [Range(0f, 2f)] float _pitch = 1f;
    [SerializeField] List<AudioClip> _audioClips = new List<AudioClip>();
    [SerializeField] float _kickForce;

    Rigidbody2D _parentBody;
    Vector2 _lerpParentVel;

    private void Awake()
    {
        _parentBody = GetComponentInParent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (_parentBody == null) return;
        _lerpParentVel = _lerpParentVel.Lerp(_parentBody.linearVelocity, 1f * Time.deltaTime);
    }

    bool _quitting = false;

    void OnApplicationQuit()
    {
        _quitting = true;
    }
    private void OnDestroy()
    {
        Trigger();
    }

    public void Trigger()
    {
        if (_quitting || !SceneManager.GetActiveScene().isLoaded)
            return;

        Vector2 spawnPos = transform.position;
        foreach (GameObject o in _objectsToSpawn)
        {
            GameObject spawnedObject = Instantiate(o, (Vector3)spawnPos + transform.TransformVector(Offset), Quaternion.identity);

            Rigidbody2D spawnedBody = spawnedObject.GetComponent<Rigidbody2D>();
            if (spawnedBody != null)
            {
                // Kick
                spawnedBody.AddForce(Random.insideUnitCircle * _kickForce, ForceMode2D.Impulse);

                // Inherit
                if (_parentBody != null)
                {
                    Debug.Log("Inheriting: " + InheritVelocity * _lerpParentVel);
                    spawnedBody.linearVelocity += InheritVelocity * _lerpParentVel;
                }
            }
        }

        foreach (AudioClip clip in _audioClips)
        {
            AudioSpawner.PlayAudio(clip, _audioVolume, _pitch, (Vector2)transform.position);
        }
    }
}
