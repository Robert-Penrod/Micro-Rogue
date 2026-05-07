using UnityEngine;

public class BattleTester : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] [Min(1)] int _battleNum = 1;
    [SerializeField] float _battleTimeScale = 2f;
    int _battleCount = 0;
    [SerializeField] Actor _actorAPrefab;
    [SerializeField] Actor _actorBPrefab;

    [Header("Statistics")]
    int _aWins;
    int _bWins;
    float _aWinPercent;
    float _bWinPercent;

    bool _isBattleStarted = false;
    Actor _actorAInstance;
    Actor _actorBInstance;

    void StartBattle()
    {
        float dist = 5f;
        _actorAInstance = Instantiate(_actorAPrefab.gameObject, -dist * Vector2.right, Quaternion.identity).GetComponent<Actor>();
        _actorAInstance.gameObject.SetActive(true);
        _actorBInstance = Instantiate(_actorBPrefab.gameObject, dist * Vector2.right, Quaternion.identity).GetComponent<Actor>();
        _actorBInstance.gameObject.SetActive(true);
        Utils.SetFullTimeScale(_battleTimeScale);
        _isBattleStarted = true;
        _battleCount++;
    }

    private void Update()
    {
        if(!_isBattleStarted)
        {
            if(_battleCount < _battleNum) StartBattle();
        }
        else
        {
            bool isGameOver = false;
            if(HasActorLost(_actorAInstance))
            {
                _bWins++;
                isGameOver = true;
            }
            if(HasActorLost(_actorBInstance))
            {
                _aWins++;
                isGameOver = true;
            }
            if(isGameOver)
            {
                UpdateWinPercents();
                ResetBattleTester();
            }
        }
    }

    void UpdateWinPercents()
    {
        int winTotal = _aWins + _bWins;
        _aWinPercent = (float)_aWins / winTotal;
        _bWinPercent = (float)_bWins / winTotal;
    }

    void ResetBattleTester()
    {
        if (_actorAInstance != null) Destroy(_actorAInstance.gameObject);
        if (_actorBInstance != null) Destroy(_actorBInstance.gameObject);
        _isBattleStarted = false;
        Utils.SetFullTimeScale(1f);
    }

    bool HasActorLost(Actor actor) => actor == null || !actor.IsAlive;
}
