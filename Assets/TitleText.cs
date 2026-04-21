using TMPro;
using UnityEngine;

public class TitleText : MonoBehaviour
{
    [SerializeField] float _lerpSpeed = 3f;
    TextMeshPro _textMesh;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        float targetAlpha = PlayerManager.I.PlayerList.Count > 0 ? 0f : 1f;
        if(_textMesh.color.a != targetAlpha)
        {
            _textMesh.color = _textMesh.color.Alpha(_textMesh.color.a.Lerp(targetAlpha, _lerpSpeed * Time.deltaTime));
        }
    }
}
