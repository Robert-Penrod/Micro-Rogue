using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineCoordinator
{
    List<IEnumerator> _coroutineQueue = new List<IEnumerator>();
    MonoBehaviour _mb;

    public int QueueSize => _coroutineQueue.Count;

    public CoroutineCoordinator(MonoBehaviour mb)
    {
        _mb = mb;
        Start();
    }

    public IEnumerator Coordinate_Co()
    {
        while (true)
        {
            while (_coroutineQueue.Count > 0)
            {
                IEnumerator coroutine = _coroutineQueue[0];
                _coroutineQueue.RemoveAt(0);
                yield return _mb.StartCoroutine(coroutine);
                
            }
            yield return null;
        }
    }

    public void Add(IEnumerator coroutine)
    {
        _coroutineQueue.Add(coroutine);
    }

    public void AddNext(IEnumerator coroutine)
    {
        _coroutineQueue.Insert(0, coroutine);
    }

    public void Clear()
    {
        _coroutineQueue.Clear();
    }

    public void Stop()
    {
        _mb.StopCoroutine(Coordinate_Co());
    }

    public void Start()
    {
        _mb.StartCoroutine(Coordinate_Co());
    }
}
