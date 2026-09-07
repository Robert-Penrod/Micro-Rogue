using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GameTimerText : MonoBehaviour
{
    TextMeshProUGUI _textMesh;
    DungeonManager _dungeonManager;
    private void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _dungeonManager = DungeonManager.I;
        _textMesh.color = _textMesh.color.Alpha(0f);
    }

    private void Update()
    {
        float lerpSpeed = 3f;
        bool isTimerRunning = _dungeonManager.IsRunTimerRunning();

        _textMesh.enabled = _dungeonManager.IsRunStarted;
        _textMesh.text = _dungeonManager.RunInfoData.RunTime.SecondsToMonoTimeStringMinSec(_textMesh.fontSize); ;

        float targetAlpha = isTimerRunning ? 0.75f : 0.25f;
        float lerpAlpha = _textMesh.color.a.Lerp(targetAlpha, lerpSpeed * Time.deltaTime);
        _textMesh.color = _textMesh.color.Alpha(lerpAlpha);

        float targetSize = isTimerRunning ? 1f : 0.875f;
        _textMesh.transform.localScale = Vector3.one * _textMesh.transform.localScale.x.Lerp(targetSize, lerpSpeed * Time.deltaTime);
    }
}
