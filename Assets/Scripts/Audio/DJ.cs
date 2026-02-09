using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJ : PersistantSingleton<DJ>
{
    AudioSourceLerper _deck1;
    AudioSourceLerper _deck2;

    [field: Header("Music")]
    [SerializeField] List<AudioClip> _songList = new List<AudioClip>();

    List<AudioClip> _songQueue = new List<AudioClip>();

    bool _isInTransition = false;
    int _transitionDir = 1;
    float _transitionTimer = 0f;

    bool _hasStarted = false;

    float _deck1StopPos = 0f;
    float _deck2StopPos = 0f;

    private void OnValidate()
    {
        ClearNullSongs();
    }

    protected override void Awake()
    {
        base.Awake();

        CreateAudioSources(); 

        _songQueue.AddRange(_songList);
        ClearNullSongs();
    }

    void CreateAudioSources()
    {
        if (_deck1 == null) _deck1 = CreateAudioDeck("deck 1");
        if (_deck2 == null) _deck2 = CreateAudioDeck("deck 2");
    }

    AudioSourceLerper CreateAudioDeck(string deckName = "")
    {
        AudioSourceLerper audioDeck = new GameObject(deckName).AddComponent<AudioSourceLerper>();
        audioDeck.transform.SetParent(this.transform);
        return audioDeck;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            if (_deck1.AudioSource.enabled)
            {
                _deck1StopPos = _deck1.AudioSource.time;
                _deck2StopPos = _deck2.AudioSource.time;
            }

            _deck1.AudioSource.enabled = !_deck1.AudioSource.enabled;
            _deck2.AudioSource.enabled = _deck1.AudioSource.enabled;

            //if (_deck1.Volume.Target > 0)
            {
                _deck1.AudioSource.time = _deck1StopPos;
                _deck1.AudioSource.Play();
            }
            //if (_deck2.Volume.Target > 0)
            {
                _deck2.AudioSource.time = _deck2StopPos;
                _deck2.AudioSource.Play();
            }

            _deck1.enabled = _deck1.AudioSource.enabled;
            _deck2.enabled = _deck2.AudioSource.enabled;
            _hasStarted = false;
        }
        if (!_deck1.enabled) return;

        // Starting
        if(_deck1.Volume.Value <= 0f && _deck2.Volume.Value <= 0f && _transitionTimer <= 0f) NextSong();

        // Ran out of audio
        //if (_hasStarted && !_deck1.AudioSource.isPlaying && !_deck2.AudioSource.isPlaying && Time.timeScale > 0.1f) NextSong(0.5f);

        // Input
        if (Input.GetKeyDown(KeyCode.N)) NextSong(4f);
        else if (_transitionTimer <= 0f) NextSong();

        // Tick transition timer
        if (_transitionTimer > 0)  _transitionTimer -= Time.deltaTime;
    }

    void ClearNullSongs()
    {
        _songList.RemoveAll(x => x == null);
        _songQueue.RemoveAll(x => x == null);
    }

    void NextSong(float transMult = 1f)
    {
        if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
        _transitionCoroutine = StartCoroutine(TransitionToSong_Co(transMult));
    }

    Coroutine _transitionCoroutine;
    IEnumerator TransitionToSong_Co(float transMult = 1f)
    {
        // Get random clip to play next
        Random.InitState(System.DateTime.Now.Ticks.GetHashCode());
        if (_songQueue.Count == 0) _songQueue.AddRange(_songList);
        AudioClip randomClip = _songQueue.GetRandomElement();
        _songQueue.Remove(randomClip);

        // Setup Transition
        AudioSourceLerper targetDeck = _transitionDir > 0 ? _deck2 : _deck1;
        AudioSourceLerper otherDeck = _transitionDir > 0 ? _deck1 : _deck2;
        otherDeck.Volume.Target = -0.1f;

        // Setup Timing
        float minPlayTime = 20f;
        float maxPlayTime = (randomClip.length).ClampMax(100f);
        _transitionTimer = Random.Range(minPlayTime, maxPlayTime);
        _transitionDir = _transitionDir > 0 ? -1 : 1;

        // Transition pause
        yield return new WaitForSecondsRealtime(Random.Range(0f, 2f) / transMult);

        targetDeck.AudioSource.clip = randomClip;
        targetDeck.Volume.Target = 1.1f;

        targetDeck.AudioSource.time = Random.Range(0f, 0.75f * targetDeck.AudioSource.clip.length);
        targetDeck.Volume.Value = 0f;
        targetDeck.AudioSource.Play();

        targetDeck.Volume.LerpMult = transMult * Random.Range(0.1f, 0.4f);
        otherDeck.Volume.LerpMult = transMult * Random.Range(0.1f, 0.4f);

        yield return null;

        _hasStarted = true;
    }
}
