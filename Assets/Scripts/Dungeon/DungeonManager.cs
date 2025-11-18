using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonManager : PersistantSingleton<DungeonManager>
{
    [SerializeField] Transform _hub;
    [SerializeField] GameObject _roomGeneratorPrefab;
    [SerializeField] GameObject _encounterGeneratorPrefab;
    
    Transform _dungeonTransform;
    PlayerManager _playerManager;

    bool _isPortalVoteReady => _playerManager.PlayerList.Count > 0 && !_playerManager.PlayerList.Find(p => p.SelectedPortal == null);

    public Portal SelectedPortal { get; private set; }
    public float PortalPercent { get; private set; }

    [SerializeField] AudioClip _portalFinishSound;

    #region DungeonData
    [System.Serializable]
    public class DungeonData
    {
        public int Seed;
        public int RoomNumber;

    }
    public DungeonData Data;
    #endregion

    #region GenerateLevel()
    public void GenerateLevel()
    {
        if(_hub != null) _hub.gameObject.SetActive(false);
        StartCoroutine(GenerateLevel_Co());
    }

    IEnumerator GenerateLevel_Co()
    {
        // Clear Dungeon
        foreach (Transform t in _dungeonTransform)
            Destroy(t.gameObject);

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
        _dungeonTransform = new GameObject("Dungeon").transform;
        _dungeonTransform.SetParent(this.transform);
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
        PortalPercent = _portalTick / Constants.DungeonStats.PortalTime;

        if (_isPortalVoteReady)
        {
            SelectedPortal = GetPlayerVotePortal();
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
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            // Get Info
            SelectedPortal = GetPlayerVotePortal();

            // Step data
            Data.RoomNumber++;

            // Destroy Portals
            foreach (Portal p in FindObjectsByType<Portal>(FindObjectsSortMode.None))
            {
                Destroy(p.gameObject);
            }

            // New Level
            GenerateLevel();

            yield return new WaitForFixedUpdate();

            // Teleport Players
            Random.InitState(Data.GetHashCode());
            Vector2 spawnPos = SpawnSystem.GetRandomEmptyPos(0.25f);
            Debug.DrawLine(spawnPos, (Vector3)spawnPos + Vector3.up * 10f, Color.red, 10000f);
            foreach(var player in _playerManager.PlayerList)
            {
                player.Actor.transform.position = spawnPos;// (Vector3)SpawnSystem.GetRandomEmptyPos(spawnPos, 2f) + Vector3.forward * player.Actor.transform.position.z;
            }

            // Extra
            Debug.Log("Portaled!");
            AudioSpawner.PlayAudioWithRandPitch(_portalFinishSound, 0.2f, 1f, 1f);
        }
    }

    Portal GetPlayerVotePortal()
    {
        if (!_isPortalVoteReady)
            return null;

        // Group by portal and compute counts
        var groups = _playerManager.PlayerList
            .GroupBy(p => p.SelectedPortal)
            .Select(g => new { Portal = g.Key, Count = g.Count() })
            .ToList();

        // Find the highest vote count
        int maxCount = groups.Max(g => g.Count);

        // Get all portals that have the highest count
        var topPortals = groups
            .Where(g => g.Count == maxCount)
            .Select(g => g.Portal)
            .ToList();

        int seed = Data.GetHashCode();
        foreach (var portal in topPortals)
        {
            seed = HashCode.Combine(seed, portal.name);  // or any stable property
        }
        Random.InitState(seed);

        // If only one, return it; if multiple, choose random
        return topPortals[Random.Range(0, topPortals.Count)];
    }
    #endregion
}
