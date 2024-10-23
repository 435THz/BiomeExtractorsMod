using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terraria;

namespace BiomeExtractorsMod.Common.Collections
{
    public class WeightedList<T> : IDictionary<T, Fraction>
    {
        private readonly Dictionary<T, Fraction> dictionary;
        public Fraction TotalWeight { get; private set; }
        public int Count => dictionary.Count;

        public bool IsReadOnly => false;

        public ICollection<T> Keys => dictionary.Keys;

        public ICollection<Fraction> Values => dictionary.Values;

        public Fraction this[T element]
        {
            get => dictionary[element];
            set => dictionary[element] = value;
        }

        public WeightedList()
        {
            dictionary = [];
            TotalWeight = Fraction.Zero;
        }

        public void Add(T element) => Add(element, Fraction.One);
        public void Add(T element, int weight_num, int weight_den) => Add(element, new Fraction(weight_num, weight_den));
        public void Add(T element, Fraction weight)
        {
            if (dictionary.ContainsKey(element))
                dictionary[element]+=weight;
            else
                dictionary.Add(element, weight);
            TotalWeight+=weight;
        }

        public void Clear()
        {
            dictionary.Clear();
            TotalWeight = Fraction.Zero;
        }

        public bool ContainsKey(T element)
            => dictionary.ContainsKey(element);

        public bool Remove(T element)
        {
            if (!dictionary.ContainsKey(element)) return false;
            TotalWeight -= dictionary[element];
            dictionary.Remove(element);
            return true;
        }

        public IEnumerator<KeyValuePair<T, Fraction>> GetEnumerator()
            => dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => dictionary.GetEnumerator();

        public bool TryGetValue(T key, [MaybeNullWhen(false)] out Fraction value)
            => dictionary.TryGetValue(key, out value);

        public T Roll()
        {
            if (dictionary.Count == 0) return default;
            Fraction roll = new(Main.rand.Next(TotalWeight.Num), TotalWeight.Den);

            T ret = FromWeight(roll);

            if (ret.Equals(default(T))) return Roll();
            return ret;
        }

        public T FromWeight(Fraction weight)
        {
            if (weight < 0) return default;

            Fraction current = new(0);
            T def = default;
            T ret = default;
            foreach (T key in Keys)
            {
                current += this[key];
                if (current > weight && EqualityComparer<T>.Default.Equals(ret, def)) ret = key;
            }
            TotalWeight = current;
            return ret;
        }

        public void Add(KeyValuePair<T, Fraction> item)
            => Add(item.Key, item.Value);

        public bool Contains(KeyValuePair<T, Fraction> item)
        {
            return ContainsKey(item.Key) && this[item.Key] == item.Value;
        }

        public void CopyTo(KeyValuePair<T, Fraction>[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach (KeyValuePair<T, Fraction> element in dictionary)
            {
                array[i] = element;
                i++;
            }
        }

        public bool Remove(KeyValuePair<T, Fraction> item)
        {
            if (Contains(item))
                return dictionary.Remove(item.Key);
            return false;
        }
    }
}