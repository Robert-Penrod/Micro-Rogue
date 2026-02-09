using System.Collections;
using UnityEngine;

public class AmbientNotePlayer : Singleton<AmbientNotePlayer>
{
    [SerializeField] [Min(0f)] float _minNoteTime;
    [SerializeField] [Min(0f)] float _maxNoteTime;
    TickTimer _noteTimer;
    NoteSampler _noteSampler;

    protected override void Awake()
    {
        base.Awake();
        _noteSampler = GetComponent<NoteSampler>();
        ResetNoteTimer();
    }

    private void Update()
    {
        if(!_noteTimer.IsDone())
        {
            _noteTimer.Tick(Time.deltaTime);
        }
        else
        {
            PlayNote();
            ResetNoteTimer();
        }
    }

    void PlayNote()
    {
        _noteSampler.TestPlayFMajPentatonic();
    }

    public void PlayRiff(float magnitude = 1f)
    {
        int noteCount = (int)(magnitude * Random.Range(2, 5));
        float timeBetweenNotes = (int)(magnitude * Random.Range(0.15f, 0.25f));

        Debug.Log("RiF of " + noteCount.ToString() + " notes");

        if (riffCoroutine == null)
        {
            //Debug.Log("Starting " + noteCount + " note riff");
            riffCoroutine = StartCoroutine(PlayRiff_Coroutine(noteCount, timeBetweenNotes));
        }
        else
        {
            //Debug.Log("Adding " + noteCount + " notes to riff");
            riffNoteCount++;
        }
        
    }

    int riffNoteCount;
    Coroutine riffCoroutine;
    IEnumerator PlayRiff_Coroutine(int noteCount, float timeBetweenNotes)
    {
        riffNoteCount = noteCount;
        for(; riffNoteCount > 0; riffNoteCount--)
        {
            PlayNote();
            float time = Mathf.Max(0.01f, timeBetweenNotes + Random.Range(-1f, 1f) * 0.05f);
            yield return new WaitForSeconds(timeBetweenNotes);
        }
        riffCoroutine = null;
    }

    void ResetNoteTimer()
    {
        _noteTimer = new TickTimer(Random.Range(_minNoteTime, _maxNoteTime));
    }
}
