using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Kryz.Stats
{
	[Serializable, InlineProperty, HideReferenceObjectPicker]
	public class Stat
	{
		public string Name { get; private set; }

		[HorizontalGroup("StatGroup", 75)] // Groups with the Value field
		[HideLabel] // Hides the default label
		public float BaseValue;

		protected bool isDirty = true;
		protected float lastBaseValue;

		bool _isInt;
		public float PositiveDir { get; private set; }
		public string Unit { get; private set; }

		protected float _value;
		public virtual float Value
		{
			get
			{
				if (isDirty || lastBaseValue != BaseValue)
				{
					lastBaseValue = BaseValue;

					float oldValue = _value;
					_value = CalculateFinalValue();
					
					isDirty = false;
					OnValueChanged?.Invoke(oldValue, _value);
				}
				return _value;
			}
		}

		[HideInInspector] protected List<StatModifier> statModifiers;
		public readonly ReadOnlyCollection<StatModifier> StatModifiers;

		public Action<float, float> OnValueChanged;

		public Stat()
        {
			statModifiers = new List<StatModifier>();
			StatModifiers = statModifiers.AsReadOnly();
		}

		public Stat(string name) : this()
		{
			this.Name = name;
		}

		public Stat(string name, string unit) : this()
        {
			this.Name = name;
			this.Unit = unit;
        }

		public Stat(float baseValue, string name, bool isInt = false, string unit = "", float positiveDir = 1) : this(name)
		{
			BaseValue = baseValue;
			_isInt = isInt;
			PositiveDir = positiveDir;
			Unit = unit;
		}

		public Stat(Stat otherStat) : this(otherStat.BaseValue, otherStat.Name)
		{
			statModifiers.AddRange(otherStat.statModifiers);
		}

		public virtual bool AddModifier(StatModifier mod)
		{
			if (!mod.IsStackable)
			{
				bool modAlreadyExists = false;
				statModifiers.ForEach(x =>
				{
					if (x.Source != null && x.Source == mod.Source && x.Value == mod.Value)
					{
						modAlreadyExists = true;
					}
				});
				if (modAlreadyExists)
				{
					return false;
				}
			}

			isDirty = true;
			statModifiers.Add(mod);
			return true;
		}
		public virtual void AddModifier(StatModifier mod, float time, MonoBehaviour mono)
		{
			AddModifier(mod);

			Utils.DelayedInvoke(mono, time, () =>
			{
				RemoveModifier(mod);
			});
		}

		public virtual bool RemoveModifier(StatModifier mod)
		{
			if (statModifiers.Remove(mod))
			{
				isDirty = true;
				return true;
			}
			return false;
		}

		public virtual bool RemoveAllModifiersFromSource(object source)
		{
			int numRemovals = statModifiers.RemoveAll(mod => mod.Source == source);

			if (numRemovals > 0)
			{
				isDirty = true;
				return true;
			}
			return false;
		}

		public virtual bool RemoveAllModifiersWithTag(string tag)
		{
			int numRemovals = statModifiers.RemoveAll(mod => mod.Tags.Contains(tag));

			if (numRemovals > 0)
			{
				isDirty = true;
				return true;
			}
			return false;
		}

		public virtual bool RemoveAllModifiers()
		{
			statModifiers.Clear();
			isDirty = true;
			return true;
		}

		public virtual List<StatModifier> GetAllModifiersWithTag(string tag)
		{
			return statModifiers.FindAll(x => x.Tags.Contains(tag));
		}

		protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
		{
			if (a.Order < b.Order)
				return -1;
			else if (a.Order > b.Order)
				return 1;
			return 0; //if (a.Order == b.Order)
		}

		protected virtual float CalculateFinalValue()
		{
			float finalValue = BaseValue;
			float sumPercentAdd = 0;

			if (statModifiers.Count > 1)
			{
				statModifiers.Sort(CompareModifierOrder);
			}

			for (int i = 0; i < statModifiers.Count; i++)
			{
				StatModifier mod = statModifiers[i];

				if (mod.Type == StatModType.Flat)
				{
					finalValue += mod.Value;
				}
				else if (mod.Type == StatModType.PercentAdd)
				{
					sumPercentAdd += mod.Value;

					if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
					{
						finalValue *= 1 + sumPercentAdd;
						sumPercentAdd = 0;
					}
				}
				else if (mod.Type == StatModType.PercentMult)
				{
					finalValue *= 1 + mod.Value;
				}
			}

			// Workaround for float calculation errors, like displaying 12.00001 instead of 12
			return (float)Math.Round(finalValue, 4);
		}

        public override string ToString()
        {
            return base.ToString() + Unit;
        }
    }
}
