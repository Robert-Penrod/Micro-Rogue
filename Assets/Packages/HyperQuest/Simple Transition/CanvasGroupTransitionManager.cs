using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupTransitionManager : PersistantSingleton<CanvasGroupTransitionManager>
{
    const float _defaultTransitionTime = 0.1f;
    CanvasGroup _canvasGroup;

    protected override void Awake()
    {
        base.Awake();
        _canvasGroup = GetComponent<CanvasGroup>();
        //_canvasGroup.alpha = 0f;
        //SetObjectsActive(false);
    }

    public void RestartScene(float speedMult = 1f) => LoadScene(SceneManager.GetActiveScene().name, speedMult);

    Coroutine LoadScene_CI;
    public void LoadScene(string sceneName, float speedMult = 1f)
    {
        //SetObjectsActive(true);
        if (LoadScene_CI != null) StopCoroutine(LoadScene_CI);
        if (FadeScene_CI != null) StopCoroutine(FadeScene_CI);
        LoadScene_CI = StartCoroutine(LoadScene_Co());
        IEnumerator LoadScene_Co()
        {
            while (1f - _canvasGroup.alpha > 0.01f)
            {
                _canvasGroup.alpha = _canvasGroup.alpha.Lerp(1.1f, speedMult * (_defaultTransitionTime > 0.001f ? Time.unscaledDeltaTime / _defaultTransitionTime : 1f).Clamp01());
                yield return null;
            }
            Debug.Log("Loading");
            _canvasGroup.alpha = 1f;
            yield return null;
            SceneManager.LoadSceneAsync(sceneName);
            //SceneManager.LoadScene(sceneName);
        }
    }

    Coroutine FadeScene_CI;
    private void OnLevelWasLoaded(int level)
    {
        if(_canvasGroup.alpha > 0f)
        {
            if (FadeScene_CI != null) StopCoroutine(FadeScene_CI);
            if (LoadScene_CI != null) StopCoroutine(LoadScene_CI);
            FadeScene_CI = StartCoroutine(FadeToScene_Co());
            IEnumerator FadeToScene_Co()
            {
                yield return new WaitForSecondsRealtime(0.125f); // 0.125
                while (_canvasGroup.alpha > 0.01f)
                {
                    float t = (_defaultTransitionTime > 0.001f ? Time.unscaledDeltaTime / _defaultTransitionTime : 1f).Clamp01();
                    _canvasGroup.alpha = _canvasGroup.alpha.Lerp(0f, t);
                    yield return null;
                }
                _canvasGroup.alpha = 0f;
                //SetObjectsActive(false);
            }
        }
    }

    void SetObjectsActive(bool isActive)
    {
        transform.GetChild(0).gameObject.SetActive(isActive);
    }
}
