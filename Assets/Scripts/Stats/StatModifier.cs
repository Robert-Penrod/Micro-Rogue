using System.Collections.Generic;

namespace Kryz.Stats
{
	public enum StatModType
	{
		Flat = 100,
		PercentAdd = 200,
		PercentMult = 300,
	}

	[System.Serializable]
	public class StatModifier
	{
		public float Value;
		public StatModType Type;
		public int Order;
		public object Source;
		public bool IsStackable;
		public List<string> Tags = new List<string>();

		public StatModifier(float value, StatModType type, int order, List<string> tagList, object source, bool isStackable)
		{
			Value = value;
			Type = type;
			Order = order;
			Source = source;
			IsStackable = isStackable;
			Tags = new List<string>();
			if (tagList != null)
			{
				Tags.AddRange(tagList);
			}
		}

		public StatModifier(StatModifier modToCopy)
        {
			Value = modToCopy.Value;
			Type = modToCopy.Type;
			Order = modToCopy.Order;
			Source = modToCopy.Source;
			IsStackable = modToCopy.IsStackable;
			Tags = new List<string>();
			Tags.AddRange(modToCopy.Tags);
        }

		public StatModifier(float value, StatModType type) : this(value, type, (int)type, null, null, false) { }

		public StatModifier(float value, StatModType type, int order) : this(value, type, order, null, null, false) { }

		public StatModifier(float value, StatModType type, object source) : this(value, type, (int)type, null, source, false) { }

		public StatModifier(float value, StatModType type, object source, bool isStackable) : this(value, type, (int)type, null, source, isStackable) { }

		public StatModifier(float value, StatModType type, List<string> tagList, object source = null, bool isStackable = false) : this(value, type, (int)type, tagList, source, isStackable) { }
		public StatModifier(float value, StatModType type, string[] tagList, object source = null, bool isStackable = false) : this(value, type, (int)type, null, source, isStackable) 
		{
			Tags = new List<string>();
			Tags.AddRange(tagList);
		}

		/*
        public override string ToString()
        {
			string s = string.Empty;

            if(Value >= 0)
            {
				s += "+";
            }

			s += Value.ToString();

			if(Type != StatModType.Flat)
            {
				s += "%";
            }

			return s;
        }
		*/

		public float CalculatePreviewValue(Stat stat, object source = null)
		{
			Stat previewStat = new Stat(stat);
			previewStat.AddModifier(this);
			return previewStat.Value;
		}

		public override string ToString()
		{
			string signString = Value > 0 ? "+" : string.Empty;
			switch (Type)
			{
				case StatModType.Flat:
					return signString + Value;
				default:
					return signString + (Value * 100f) + "%";
			}
			return "ERROR";
		}
	}
}
