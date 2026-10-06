using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceText : TMProSetter
{
    [SerializeField] float _fadeTime = -1f;
    List<Image> _imageList = new();
    public enum ResourceType { Gem, Relic}
    public ResourceType Type;
    string _prevString;
    float _lastStringChangeTime;

    float _lerpSpeed = 12f;

    private void Start()
    {
        _imageList.AddRange(transform.parent.GetComponentsInChildren<Image>());
    }

    private void Update()
    {
        // Updates
        if (Type == ResourceType.Gem)
        {
            TextMesh.text = Player.PlayerData.Gems.ToString();
        }
        else if (Type == ResourceType.Relic)
        {
            TextMesh.text = Player.PlayerData.Relics.ToString();
        }

        // String Change
        if (_prevString != TextMesh.text)
        {
            _lastStringChangeTime = Time.time;
            _prevString = TextMesh.text;
        }

        // Delta Time
        float timeSinceChange = Time.time - _lastStringChangeTime;

        // Fade
        float targetAlpha = 1f;
        if (_fadeTime > 0 && DungeonManager.I.IsRunStarted) targetAlpha = timeSinceChange.Remap(0.5f * _fadeTime, _fadeTime, 1f, 0f);
        float lerpAlpha = TextMesh.color.a.Lerp(targetAlpha, _lerpSpeed * Time.deltaTime);
        TextMesh.color = TextMesh.color.Alpha(lerpAlpha);
        _imageList.ForEach(x => x.color = x.color.Alpha(lerpAlpha));
    }
}
