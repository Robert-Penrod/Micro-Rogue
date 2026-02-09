using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class NoteSampler : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] float _volume = 0.5f;
    [SerializeField] AudioClip _sample;
    AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayNote(string note, int transpose = 0)
    {
        int noteNum = GetNoteNum(note);
        noteNum += transpose;
        //Debug.Log(noteNum);
        float pitch = Mathf.Pow(2, (noteNum) / 12f);
        AudioSpawner.PlayAudio(_sample, _volume, pitch);
        
        /*
        _audioSource.volume = _volume;
        _audioSource.pitch = pitch;
        _audioSource.PlayOneShot(_sample);
        */
    }

    public void TestPlayFMajPentatonic()
    {
        string noteString = "";
        switch (Random.Range(0, 5))
        {
            case 0:
                noteString = "f";
                break;
            case 1:
                noteString = "g";
                break;
            case 2:
                noteString = "a";
                break;
            case 3:
                noteString = "c";
                break;
            case 4:
                noteString = "d";
                break;
        }
        noteString += (4 + Random.Range(-1, 1)).ToString();
        PlayNote(noteString);
    }

    int GetNoteNum(string note)
    {
        //"(letter)(number)(alteration)"
        string noteLetter = note.Substring(0, 1).ToLower();
        int letterNum = int.MinValue;
        switch (noteLetter)
        {
            /*
            case "a":
                letterNum = -3;
                break;
            case "b":
                letterNum = -1;
                break;
            case "c":
                letterNum = 0;
                break;
            case "d":
                letterNum = 2;
                break;
            case "e":
                letterNum = 4;
                break;
            case "f":
                letterNum = 5;
                break;
            case "g":
                letterNum = 7;
                break;
            */
            case "a":
                letterNum = 0;
                break;
            case "b":
                letterNum = 2;
                break;
            case "c":
                letterNum = 3;
                break;
            case "d":
                letterNum = 4;
                break;
            case "e":
                letterNum = 6;
                break;
            case "f":
                letterNum = 7;
                break;
            case "g":
                letterNum = 9;
                break;
        }

        string transpose = string.Empty;
        if (note.Length == 2)
        {
            transpose = note.Substring(1, 1);
        }
        else
        {
            transpose = note.Substring(2, 1);
        }

        int alteration = 0;
        if (note.Length > 2)
        {
            string altString = note.Substring(1, 1);
            if (altString.Equals("#"))
            {
                alteration = +1;
            }
            else if (altString.Equals("b"))
            {
                alteration = -1;
            }
        }

        // 0 == C5
        int totalNoteNum = 12 * (Int32.Parse(transpose) - 5) + letterNum + alteration;
        return totalNoteNum;
    }
}
