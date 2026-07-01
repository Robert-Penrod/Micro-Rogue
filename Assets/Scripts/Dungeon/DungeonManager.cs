using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DungeonManager : Singleton<DungeonManager>
{
    [SerializeField] Transform _hub;
    [SerializeField] GameObject _roomGeneratorPrefab;
    [SerializeField] GameObject _encounterGeneratorPrefab;
    [SerializeField] GameObject _portalPrefab;

    [SerializeField] Material _wallMat;
    [SerializeField] Material _floorMat;

    public bool IsEncounterOver { get; private set; }
    
    public Transform DungeonTransform { get; private set; }
    PlayerManager _playerManager;

    bool _isPortalVoteReady => _playerManager.PlayerList.Count > 0 && !_playerManager.PlayerList.Find(p => p.SelectedPortal == null);

    public Portal SelectedPortal { get; private set; }
    public float PortalPercent { get; private set; }

    [SerializeField] AudioClip _portalFinishSound;

    // Events
    public Action OnDungeonDataChanged;
    public Action OnPortalTransitionStart;
    public Action OnGenerateLevel;

    public enum BiomeEnum { Forest = 1, Cave = 2, Dungeon = 3 }
    public Sprite WildsIcon;
    public Sprite UndergroundIcon;
    public Sprite DungeonIcon;
    [SerializeField] BiomeData _wildsData;
    [SerializeField] BiomeData _undergroundData;
    [SerializeField] BiomeData _dungeonData;
    public Sprite GetBiomeSprite(BiomeEnum biome)
    {
        Sprite sprite = biome switch
        {
            BiomeEnum.Forest => WildsIcon,
            BiomeEnum.Cave => UndergroundIcon,
            BiomeEnum.Dungeon => DungeonIcon,
            _ => null
        };
        return sprite;
    }
    public Color GetBiomeColor(BiomeEnum biome)
    {
        Color c = biome switch
        {
            BiomeEnum.Forest => Color.green,
            BiomeEnum.Cave => Color.grey,
            BiomeEnum.Dungeon => Color.blue,
            _ => Color.black
        };
        c = c.SetSaturation(0.7f * c.GetSaturation());
        return c;
    }

    public void SetBiomeWilds()
    {
        Data.Biome = BiomeEnum.Forest;
        GenerateRandomizedRoom();
    }
    public void SetBiomeUnderground()
    {
        Data.Biome = BiomeEnum.Cave;
        GenerateRandomizedRoom();
    }
    public void SetBiomeDungeon()
    {
        Data.Biome = BiomeEnum.Dungeon;
        GenerateRandomizedRoom();
    }

    void GenerateRandomizedRoom()
    {
        StartCoroutine(GenerateLevel_Co(System.DateTime.Now.Ticks.GetHashCode()));
    }

    #region DungeonData
    [System.Serializable]
    public class DungeonData
    {
        public int RunTier => 1 + ((Coordinate.y-1) / 10);

        public int Seed;
        public int RoomNumber => Coordinate.y;
        public Vector2Int Coordinate;
        public BiomeEnum Biome;
        public TagCollection Tags;

        public int EliteTier = 0;
        public bool IsBoss;
        public bool IsFinalBoss;

        public DungeonData(int seed, Vector2Int coordinate, int runLevel = 1, int eliteTier = -1)
        {
            // Seeding
            this.Seed = seed;
            this.Coordinate = coordinate;
            Random.InitState(GetSeed());

            // Encounter Type Sampling
            this.IsFinalBoss = this.Coordinate.y % 10 == 0;
            this.IsBoss = !IsFinalBoss && this.Coordinate.y % 5 == 0;
            if (eliteTier >= 0 && !IsFinalBoss && !IsBoss) this.EliteTier = eliteTier;
            else this.EliteTier = (!this.IsBoss && !this.IsFinalBoss && this.Coordinate.y > 1)? Random.Range(1, 3) : 0;

            // Biome Sampling
            this.Biome = DungeonManager.I?.Data?.Biome ?? BiomeEnum.Forest;// DungeonManager.SampleBiome(this.Seed, this.Coordinate, runLevel);
        }

        public int GetSeed()
        {
            string seed = "";
            seed += Seed.ToString();
            seed += Coordinate.ToString();
            int seedHash = seed.GetHashCode();
            //Debug.Log("Seed: " + seedHash.ToString());
            return seedHash;
        }
    }
    public DungeonData Data;
    public DungeonData PreviousData;
    #endregion

    private void Start()
    {
        UpdateBiomeTextures();
    }

    public static BiomeEnum SampleBiome(int seed, Vector2Int coord, int runTier)
    {
        return runTier switch
        {
            1 => BiomeEnum.Forest,
            2 => BiomeEnum.Cave,
            3 => BiomeEnum.Dungeon,
            _ => BiomeEnum.Forest
        };

        int biomeIndex = ((5 * (runTier-1)) + (coord.y - 1)) % 15; // (1, 15, 30) -> (1, 1, 0)
        if(biomeIndex <= 4)
        {
            return BiomeEnum.Forest;
        }
        else if(biomeIndex <= 9)
        {
            return BiomeEnum.Cave;
        }
        else
        {
            return BiomeEnum.Dungeon;
        }

        Random.InitState(seed.GetHashCode());
        int bossFloor = 5;
        int size = 16;      // resolution (keep low, gizmos are expensive)
        float scale = 6f;   // noise scale
        float offset = 1000 * Random.Range(1f, 5f);

        float wildsValue = Mathf.PerlinNoise(
                    (offset + coord.x) / (float)size * scale,
                    (offset + coord.y) / (float)size * scale
                );
        float undergroundValue = Mathf.PerlinNoise(
                    (offset * 0.25f + coord.x) / (float)size * scale,
                    (offset * 4.25f + coord.y) / (float)size * scale
                );
        float dungeonValue = Mathf.PerlinNoise(
                    (offset * 9.45f + coord.x) / (float)size * scale,
                    (offset * 3.15f + coord.y) / (float)size * scale
                );
        undergroundValue *= coord.y.Remap(1f, bossFloor, 0f, 1f);
        dungeonValue *= coord.y.Remap(1f, 2f * bossFloor, 0f, 1f);
        float maxValue = Mathf.Max(wildsValue, undergroundValue, dungeonValue);

        if(wildsValue == maxValue)
        {
            return BiomeEnum.Forest;
        }
        else if(undergroundValue == maxValue)
        {
            return BiomeEnum.Cave;
        }
        else if(dungeonValue == maxValue)
        {
            return BiomeEnum.Dungeon;
        }

        return BiomeEnum.Forest;
    }

    #region GenerateLevel()
    public void GenerateLevel()
    {
        if(_hub != null) _hub.gameObject.SetActive(false);
        StartCoroutine(GenerateLevel_Co());
    }

    public void UpdateBiomeTextures()
    {
        Random.InitState(Data.GetSeed());
        var biomeData = Data.Biome switch
        {
            BiomeEnum.Forest => _wildsData,
            BiomeEnum.Cave => _undergroundData,
            BiomeEnum.Dungeon => _dungeonData
        };

        int textureIndex = ((Data.Coordinate.y - 1).ClampMin(0) / 5);
        int wallIndex = textureIndex % biomeData._textures.Count;
        int floorIndex = (textureIndex + 1) % biomeData._textures.Count;
        _wallMat.SetTexture("_Texture", biomeData._textures[wallIndex].texture);
        _wallMat.SetVector("_Offset", new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
        _floorMat.SetTexture("_Texture", biomeData._textures[floorIndex].texture);
        _floorMat.SetVector("_Offset", new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
    }

    IEnumerator GenerateLevel_Co(int seed = 0)
    {
        if(DungeonTransform == null) DungeonTransform = new GameObject("Dungeon").transform;

        PlayerPrefs.SetInt("LastBiomeIndex", (int)Data.Biome);
        Debug.Log("Saving Last Biome: " + Data.Biome);

        // Clear Dungeon
        foreach (Transform t in DungeonTransform)
            Destroy(t.gameObject);

        // Texture
        UpdateBiomeTextures();

        // Generator Instantiations
        Random.InitState(seed + Data.GetSeed());
        Instantiate(_roomGeneratorPrefab, DungeonTransform);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        Random.InitState(Data.GetSeed());
        Instantiate(_encounterGeneratorPrefab, DungeonTransform);
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        //SpawnPortals();
        //if (Random.value > 0.5) SpawnPortals();

        OnGenerateLevel?.Invoke();
    }
    #endregion

    #region Init
    protected override void Awake()
    {
        base.Awake();
        _playerManager = PlayerManager.I;
        if (DungeonTransform == null) DungeonTransform = new GameObject("Dungeon").transform;
        Data.Seed = DateTime.Now.Ticks.GetHashCode(); // Randomize Seed
        Data.Biome = (BiomeEnum)PlayerPrefs.GetInt("LastBiomeIndex", 0);
        Debug.Log("Setting Biome to " + Data.Biome);
    }
    #endregion
    
    void OnDrawGizmos()
    {
        // For dev preview only: Running this seeds randomness to same value every frame
        /*
        int size = 16;      // resolution (keep low, gizmos are expensive)
        float scale = 6f;   // noise scale
        float spacing = 0.3f;
        float offset = 1000;

        for (int y = 0; y < size * 2; y++)
            for (int x = -size; x < size; x++)
            {
                Color c = Color.white;
                var biome = DungeonManager.SampleBiome(DungeonManager.I?.Data.Seed ?? 0, new Vector2Int(x, y));
                switch (biome)
                {
                    case BiomeEnum.Wilds:
                        c = Color.green;
                        break;
                    case BiomeEnum.Underground:
                        c = Color.grey;
                        break;
                    case BiomeEnum.Dungeon:
                        c = Color.blue;
                        break;
                }

                if(y > 0)
                {
                    if(y % 5 == 0)
                    {
                        c = c.Lerp(Color.red, 0.5f);
                    }
                }
                else
                {
                    c = c.Lerp(Color.white, 0.75f);
                }

                Gizmos.color = c;

                Gizmos.DrawCube(
                    transform.position + new Vector3(x * spacing, y * spacing, 0),
                    Vector3.one * spacing
                );
            }
        */
    }

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
            if(_portalTick == 0)
            {
                OnPortalTransitionStart?.Invoke();
            }

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

    public void SpawnPortals()
    {
        IsEncounterOver = true;
        StartCoroutine(SpawnPortals_Co());
        IEnumerator SpawnPortals_Co()
        {
            // Init
            Random.InitState(Data.Coordinate.GetHashCode());
            var portalList = new List<Portal>();
            int count = Random.Range(1, 3 + 1);
            if (count == 1 && Random.value < 0.5f) count++;
            int offset = count % 2 != 0 ? 0 : -Random.Range(0, 2);
            if ((Data.Coordinate.y + 1) % 5 == 0) count = 1; // Boss Portal
            count = 1; // testing
    
            // Loop
            for (int i = 0; i < count; i++)
            {
                Random.InitState((Data.Coordinate.ToString() + " - " + i.ToString()).GetHashCode());

                // Spawn Pos
                Vector2 spawnPos = SpawnSystem.GetRandomEmptyPos(1f);
                var portal = Instantiate(_portalPrefab, spawnPos, Quaternion.identity, DungeonTransform).GetComponent<Portal>();

                // End
                portalList.Add(portal);

                yield return new WaitForFixedUpdate();
                yield return new WaitForFixedUpdate();
            }
            portalList.Sort((a, b) => (int)(b.transform.position.x - a.transform.position.x).Sign());
            for(int i = 0; i < portalList.Count; i++)
            {
                var portal = portalList[i];

                // Dungeon Data
                // - coordinate
                Vector2Int coordinate = Data.Coordinate;
                coordinate.y = Data.RoomNumber + 1;
                coordinate.x = Data.Coordinate.x + ((count / 2) - i);
                if (count % 2 == 0) coordinate.x += offset;// On evens have random chance to sheft left to keep left right traversal balanced

                // Portal Levels
                //float lvledPortalChanceMult = 0.05f;
                //float chanceMult = Data.RoomNumber.Remap(1f, 3f, 0f, 1f);
                //int coordY = Data.RoomNumber + 1;
                //if (Random.value < chanceMult * lvledPortalChanceMult) coordinate.y = coordY + 1;
                //if (Random.value < chanceMult * lvledPortalChanceMult * 0.5f) coordinate.y = coordY + 2;

                // Set Data
                portal.SetData(new DungeonData(Data.Seed, coordinate, eliteTier: i));
            }
        }
    }

    public void StartGame()
    {
        DoPortal();
    }

    void DoPortal()
    {
        IsEncounterOver = false;
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            // Get Info
            SelectedPortal = GetPlayerVotePortal();

            // Step data
            PreviousData = Data;
            if (SelectedPortal != null)
            {
                Data = SelectedPortal.DungeonData;// new DungeonData(this.Data.Seed, SelectedPortal.DungeonData.Coordinate);
                // Take Money?
                Player.PlayerData.Gem -= SelectedPortal.GemCost;
            }
            else
            {
                Data.Coordinate.y++;
                //Data.Biome = BiomeEnum.Wilds;
            }

            // Starting run
            if(PreviousData.Coordinate.y == 0)
            {
                // Randomize Seed
                Data.Seed = System.DateTime.Now.Ticks.GetHashCode();
            }

            /*
            Data.IsBoss = SelectedPortal.DungeonData.IsBoss;
            Data.EliteTier = SelectedPortal.DungeonData.EliteTier;
            Data.IsFinalBoss = SelectedPortal.DungeonData.IsFinalBoss;
            Data.RunTier = SelectedPortal.DungeonData.RunTier;
            */

            // Destroy Portals
            foreach (Portal p in FindObjectsByType<Portal>(FindObjectsSortMode.None))
            {
                Destroy(p.gameObject);
            }

            // Upgrade
            yield return UpgradeManager.I.UpgradePlayers_Co(SelectedPortal?.Level ?? 1);

            // New Level
            GenerateLevel();

            yield return new WaitForFixedUpdate();

            // Teleport Players
            int seed = Data.GetSeed();
            //Debug.Log(seed);
            Random.InitState(seed);
            Vector2 spawnPos = SpawnSystem.GetRandomEmptyPos(0.25f);
            Debug.DrawLine(spawnPos, (Vector3)spawnPos + Vector3.up * 10f, Color.red, 10000f);
            foreach(var player in _playerManager.PlayerList)
            {
                // Clear Scent
                player.Actor.transform.position = spawnPos;// (Vector3)SpawnSystem.GetRandomEmptyPos(spawnPos, 2f) + Vector3.forward * player.Actor.transform.position.z;
                player.Actor.GetComponentInChildren<ScentSystem>().ClearScentSystem();

                // Shuffle Cooldowns
                new List<Skill>(player.Actor.GetComponentsInChildren<Skill>()).ForEach(playerSkill =>
                {
                    playerSkill.ShuffleCooldown();
                });

                // Health Increase
                if (Data.RoomNumber > 1)
                {
                    
                    float percent = player.Actor.Stats.HealthPercent;
                    player.Actor.Stats.HealthMax.AddModifier(new Kryz.Stats.StatModifier(Constants.ActorStats.HealthGain, Kryz.Stats.StatModType.Flat));
                    player.Actor.Stats.Health = (int)(percent * player.Actor.Stats.HealthMax.Value);
                }
            }

            // Extra
            //Debug.Log("Portaled!");
            AudioSpawner.PlayAudioWithRandPitch(_portalFinishSound, 0.2f, 1f, 1f);

            OnDungeonDataChanged?.Invoke();
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

        int seed = Data.GetSeed();
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
