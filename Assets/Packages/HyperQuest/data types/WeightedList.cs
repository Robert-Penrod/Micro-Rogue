using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedList<T>
{
    [System.Serializable]
    public class WeightedEntry
    {
        public T Item;
        public float Weight = 1f;

        public WeightedEntry(T item, float weight)
        {
            this.Item = item;
            this.Weight = weight;
        }
    }

    public List<WeightedEntry> Entries = new List<WeightedEntry>();

    public void AddRange(List<T> collection, float defaultWeight = 1f)
    {
        foreach (T item in collection)
        {
            Entries.Add(new WeightedEntry(item, defaultWeight));
        }
    }

    public void Add(T item, float weight = 1f)
    {
        Entries.Add(new WeightedEntry(item, weight));
    }

    public T SelectItem()
    {
        float weightSum = 0f;
        for (int i = 0; i < Entries.Count; i++)
        {
            weightSum += Entries[i].Weight;
        }

        float roll = Random.Range(0f, weightSum);
        for (int i = 0; i < Entries.Count; i++)
        {
            roll -= Entries[i].Weight;
            if (roll <= 0)
            {
                return Entries[i].Item;
            }
        }

        return default(T);
    }

    public WeightedList<T> Clone()
    {
        WeightedList<T> cloneList = new WeightedList<T>();
        Entries.ForEach(entry =>
        {
            cloneList.Entries.Add(new WeightedEntry(entry.Item, entry.Weight));
        });
        return cloneList;
    }
}
