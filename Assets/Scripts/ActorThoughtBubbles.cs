using UnityEngine;

public class ActorThoughtBubbles : MonoBehaviour
{
    [SerializeField] SpriteRenderer _questionSprite;
    [SerializeField] SpriteRenderer _alertSprite;

    SimpleNPCBrain _npcBrain;
    Actor _actor;

    float _questionLerp;
    float _alertLerp;

    float _lerpSpeed = 1.5f;

    private void Awake()
    {
        _npcBrain = GetComponentInParent<SimpleNPCBrain>();
        _actor = GetComponentInParent<Actor>();

        _questionSprite.color = _questionSprite.color.Alpha(0f);
        _alertSprite.color = _alertSprite.color.Alpha(0f);
    }

    private void Start()
    {
        _npcBrain.OnNotice += () => TriggerNotice();
        _npcBrain.OnLostTrail += () => TriggerQuestion();
    }

    private void Update()
    {
        // Sprites
        SpriteUpdate();
    }

    void SpriteUpdate()
    {
        float min = -0.1f;
        _alertLerp = _alertLerp.Lerp(min, _lerpSpeed * Time.deltaTime).Clamp01();
        _questionLerp = _questionLerp.Lerp(min, _lerpSpeed * Time.deltaTime).Clamp01();
        _alertSprite.color = _alertSprite.color.Alpha(_alertLerp);
        _questionSprite.color = _questionSprite.color.Alpha(_questionLerp);
    }

    void TriggerNotice()
    {
        _alertLerp += 1f;
    }

    void TriggerQuestion()
    {
        _questionLerp += 1f;
    }
}
