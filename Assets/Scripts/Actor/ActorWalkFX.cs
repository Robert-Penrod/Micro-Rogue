using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ActorWalkFX : MonoBehaviour
{
    public SpriteRenderer ActorSpriteRend;
    public float LerpSpeed = 16f;
    public float Frequency = 1f;
    public float Angle = 10f;
    public float Lean = 0f;
    public float Height = 0.175f;
    public AudioClip WalkSound;

    //MoveController _moveController;
    Rigidbody2D _rb;
    float _t;

    bool _wasWalking = false;
    int _lastWalkDir = -1;

    float _lastAngle = 0f;

    public Action OnStep;

    Actor _actor;

    private void Awake()
    {
        //_moveController = GetComponentInParent<MoveController>();
        _rb = GetComponentInParent<Rigidbody2D>();
        _actor = GetComponentInParent<Actor>();
    }

    private void Update()
    {
        // Detect if we are walking & if we have started walking
        Vector2 moveDir = _actor.MoveInput.normalized;
        bool isWalking = _actor.MoveInput.magnitude > 0.1f &&  _rb.linearVelocity.magnitude > 0.001f;
        bool startedWalkingThisFrame = isWalking && !_wasWalking;

        float slideMagnitude = Vector2.Dot(moveDir, _rb.linearVelocity.normalized).Remap(-1f, 1f, 1f, 0f);

        // Init walk
        if (startedWalkingThisFrame) 
        {
            _t = Mathf.PI * (_lastWalkDir > 0 ? 0 : 1); // Alternate which foot we start on
        }

        // Play walk sound when stop walking and in middle of hop
        if (!isWalking && _wasWalking && Mathf.Sign(_lastAngle) == Mathf.Sign(_lastWalkDir))
        {
            Step();
        }
        
        // Step anim time if walking
        if(isWalking)
        {
            _t += _rb.linearVelocity.magnitude * Frequency * Mathf.PI * Time.deltaTime * (1f - slideMagnitude);
        }

        // Angle
        // Calculate and lerp rotation to angle
        float currentAngle = Mathf.DeltaAngle(0, transform.localRotation.eulerAngles.z);
        float targetAngle = 0f;
        if(isWalking)
        {
            targetAngle = Angle * Mathf.Sin(_t).Remap(-0.9f, 0.9f, -1f, 1f);
        }
        //
        // Lean Angle
        targetAngle += (_actor.Body.linearVelocity.x / 3f) * Lean;
        //
        float lerpAngle = Mathf.LerpAngle(currentAngle, targetAngle, LerpSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(0f, 0f, lerpAngle);

        // Y Offset
        // Calculate and lerp y offset
        float currentY = transform.localPosition.y;
        float targetY = 0;
        if(isWalking)
        {
            targetY = Height * Mathf.Cos(_t*2f);
            if (targetY < 0f) targetY = 0f;
        }
        float lerpY = Mathf.Lerp(currentY, targetY, LerpSpeed * Time.deltaTime);
        Vector3 p = transform.localPosition;
        transform.localPosition = new Vector3(p.x, lerpY, p.z);

        // Detect walk direction
        int walkDir = (int)Mathf.Sign(currentAngle - _lastAngle);

        // Play walk sound when changing walk direction
        if (isWalking && !startedWalkingThisFrame)
        {
            bool changedWalkingDir = walkDir != _lastWalkDir;
            if (changedWalkingDir)
            {
                Step();
            }
        }

        HandleSpriteFlip();

        // Cache our walking state
        _wasWalking = isWalking;
        _lastWalkDir = walkDir;
        _lastAngle = currentAngle;
    }

    float _flipAmount = 1f;
    float _flipAnim = 1f;
    void HandleSpriteFlip()
    {
        Vector2 moveDir = _actor.MoveInput.normalized;

        // Get dir
        SpriteRenderer spriteRend = ActorSpriteRend;
        float moveX = -moveDir.x;
        float speedMult = _actor.Body.linearVelocity.magnitude;
        float prevFlipAmount = _flipAmount;
        _flipAmount = Mathf.Lerp(_flipAmount, moveX * 2f, 2f * speedMult * Time.deltaTime);

        if(Mathf.Sign(prevFlipAmount) != Mathf.Sign(_flipAmount))
        {
            _flipAmount = Mathf.Sign(_flipAmount) * 2f;
        }

        //spriteRend.flipX = _flipAmount > 0;
        Vector3 s = spriteRend.transform.localScale;
        int dir = _flipAmount > 0? -1 : 1;
        float x = Mathf.Abs(s.x) * dir;
        spriteRend.transform.localScale = new Vector3(x, s.y, s.z);
    }

    void Step()
    {
        PlayWalkSound();
        OnStep?.Invoke();
    }

    void PlayWalkSound()
    {
        float basePitch = _actor.Body.linearVelocity.magnitude;
        float vol = 0.04f * Random.Range(0.8f, 1.2f);
        vol *= _rb.transform.localScale.x.Remap(1f, 2f, 1f, 3f, false);
        AudioSpawner.PlayAudioWithRandPitch(WalkSound, 0.2f, basePitch, vol, transform.position).spatialBlend = 0.5f;
    }
}
