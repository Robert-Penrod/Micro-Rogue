using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TagCollection
{
    public enum TagType { Str = 0, Dex = 1, Int = 2, Primal = 10, Martial = 20, Heavy = 30, Finesse = 40, Arcane = 45, Alchemy = 50, Pyro = 60, Frost = 70, Static = 80, Undead = 90, Trap = 100, Slime = 500}
    [System.Serializable]
    public struct TagStack
    {
        public TagType tag;
        [Min(0)] public int count;
    }

    [SerializeField] List<TagStack> _entries = new();

    public TagCollection Clone()
    {
        TagCollection newTagCollection = new();
        this._entries.ForEach(entry =>
        {
            newTagCollection._entries.Add(entry);
        });
        return newTagCollection;
    }

    public List<TagType> GetTagList()
    {
        var tagList = new List<TagType>();

        _entries.ForEach(x =>
        {
            tagList.Add(x.tag);
        });

        return tagList;
    }

    public bool HasTag(TagType tag)
    {
        return _entries.Find(x => x.tag == tag).count > 0;
    }

    public void AddTags(TagCollection otherCol)
    {
        otherCol._entries.ForEach(tagStack =>
        {
            this.AddTag(tagStack);
        });
    }

    public void AddTag(TagStack otherTagStack)
    {
        int index = _entries.FindIndex(x => x.tag == otherTagStack.tag);

        if (index >= 0)
        {
            var stack = _entries[index];
            stack.count += otherTagStack.count;
            _entries[index] = stack;
        }
        else
        {
            _entries.Add(otherTagStack);
        }
    }

    public float CalculateWeightMultiplier(TagCollection otherCol)
    {
        List<TagStack> tagOverlapList = otherCol == null ? new() : _entries.FindAll(x => otherCol._entries.Exists(y => y.tag == x.tag && y.count > 0));
        float multiplier = 0;
        tagOverlapList.ForEach(tagStack =>
        {
            float tagMult = 0.5f;
            if(tagStack.tag == TagType.Str || tagStack.tag == TagType.Dex || tagStack.tag == TagType.Int || tagStack.tag == TagType.Primal)
            {
                tagMult = 1f;
            }
            multiplier += tagMult * tagStack.count;
        });
        return 1 + 0.1f * multiplier;
    }
}
