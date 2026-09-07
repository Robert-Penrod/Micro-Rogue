using UnityEngine;
using UnityEngine.UI;

public class DamageVignette : MonoBehaviour
{
    [SerializeField] float _maxAlpha = 0.5f;
    Image _image;
    PlayerManager _playerManager;

    private void Start()
    {
        _image = GetComponent<Image>();
        _playerManager = PlayerManager.I;
    }

    private void Update()
    {
        if (_playerManager == null || _playerManager.PlayerList.Count == 0) return;

        // Calculate amount of hits
        float totalHitValue = 0f;
        _playerManager.PlayerList.ForEach(player =>
        {
            totalHitValue += player.Actor.HitStop;
        });
        totalHitValue = totalHitValue.Clamp01();

        // Set Alpha
        float targetAlpha = totalHitValue * _maxAlpha;
        float lerpAlpha = _image.color.a.Lerp(targetAlpha, 12f * Time.deltaTime);
        _image.color = _image.color.Alpha(lerpAlpha);
    }
}
