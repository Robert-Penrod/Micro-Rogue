using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedList<T> : IEnumerable<T>
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

    public void AddRange(WeightedList<T> collection)
    {
        foreach (var entry in collection.Entries)
        {
            Entries.Add(new WeightedEntry(entry.Item, entry.Weight));
        }
    }

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

    public WeightedEntry SelectEntry()
    {
        float weightSum = 0f;
        for (int i = 0; i < Entries.Count; i++)
        {
            weightSum += Entries[i].Weight;
        }

        float roll = Random.Range(0f, weightSum);
        for (int i = 0; i < Entries.Count; i++)
        {
            var weight = Entries[i].Weight;
            if (weight <= 0) continue;

            roll -= weight;
            if (roll <= 0)
            {
                return Entries[i];
            }
        }

        return null;
    }

    public T SelectItem()
    {
        var selectedEntry = SelectEntry();
        if (selectedEntry != null)
        {
            return selectedEntry.Item;
        }
        return default(T);
    }

    public T SelectAndRemoveItem()
    {
        var selectedEntry = SelectEntry();
        if (selectedEntry != null)
        {
            this.Entries.Remove(selectedEntry);
            return selectedEntry.Item;
        }
        return default(T);
    }

    public WeightedEntry SelectAndRemoveEntry()
    {
        var selectedEntry = SelectEntry();
        if (selectedEntry != null)
        {
            this.Entries.Remove(selectedEntry);
            return selectedEntry;
        }
        return null;
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

    #region IEnumerable
    public IEnumerator<T> GetEnumerator()
    {
        foreach (var entry in Entries)
            yield return entry.Item;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion
}
