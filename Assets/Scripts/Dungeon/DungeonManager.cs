using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonManager : PersistantSingleton<DungeonManager>
{
    [SerializeField] GameObject _roomGeneratorPrefab;
    [SerializeField] GameObject _encounterGeneratorPrefab;
    
    Transform _dungeonTransform;
    PlayerManager _playerManager;

    bool _isPortalVoteReady => _playerManager.PlayerList.Count > 0 && !_playerManager.PlayerList.Find(p => p.SelectedPortal == null);

    #region DungeonData
    [System.Serializable]
    public class DungeonData
    {
        public int Seed;
        public int RoomNumber;

    }
    public DungeonData Data;
    #endregion

    #region Generation
    public void GenerateLevel()
    {
        StartCoroutine(GenerateLevel_Co());
    }

    IEnumerator GenerateLevel_Co()
    {
        // Setup dungeonTransform
        if (_dungeonTransform == null) 
            _dungeonTransform = new GameObject("Dungeon").transform;
        else
        {
            foreach (Transform t in _dungeonTransform)
                Destroy(t.gameObject);
        }

        // Generator Instantiations
        Instantiate(_roomGeneratorPrefab, _dungeonTransform);
        yield return null;
        Instantiate(_encounterGeneratorPrefab, _dungeonTransform);
    }
    #endregion

    #region Init
    protected override void Awake()
    {
        base.Awake();
        _playerManager = PlayerManager.I;
    }

    private void Start()
    {
        //GenerateLevel();
    }
    #endregion

    private void Update()
    {
        HandlePortalTick();
    }

    #region Portal
    float _portalTick;
    void HandlePortalTick()
    {
        if (_isPortalVoteReady)
        {
            _portalTick += Time.deltaTime;
            if(_portalTick >= Constants.DungeonStats.PortalTime)
            {
                DoPortal();
            }
        }
        else
        {
            _portalTick = 0f;
        }
    }

    void DoPortal()
    {
        var selectedPortal = GetPlayerVotePortal();
        Destroy(selectedPortal.gameObject);
        Data.RoomNumber++;
        Debug.Log("Portaled!");
    }

    Portal GetPlayerVotePortal()
    {
        if (!_isPortalVoteReady) 
            return null;

        return _playerManager.PlayerList
            .GroupBy(p => p.SelectedPortal)
            .OrderByDescending(g => g.Count())
            .Rand()
            .Key;
    }
    #endregion
}
