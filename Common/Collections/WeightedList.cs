using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terraria;

namespace BiomeExtractorsMod.Common.Collections
{
    public class WeightedList<T> : IDictionary<T, int>
    {
        private readonly Dictionary<T, int> dictionary;
        public int TotalWeight { get; private set; }
        public int Count => dictionary.Count;

        public bool IsReadOnly => false;

        public ICollection<T> Keys => dictionary.Keys;

        public ICollection<int> Values => dictionary.Values;

        public int this[T element]
        {
            get => dictionary[element];
            set => dictionary[element] = value;
        }

        public WeightedList()
        {
            dictionary = [];
            TotalWeight = 0;
        }

        public void Add(T element)
        {
            if (dictionary.ContainsKey(element))
                dictionary[element]++;
            else
                dictionary.Add(element, 1);
            TotalWeight++;
        }
        public void Add(T element, int weight)
        {
            if (weight <= 0) return;
            if (dictionary.ContainsKey(element))
                dictionary[element]+=weight;
            else
                dictionary.Add(element, weight);
            TotalWeight+=weight;
        }

        public void Clear()
        {
            dictionary.Clear();
            TotalWeight = 0;
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

        public IEnumerator<KeyValuePair<T, int>> GetEnumerator()
            => dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => dictionary.GetEnumerator();

        public bool TryGetValue(T key, [MaybeNullWhen(false)] out int value)
            => dictionary.TryGetValue(key, out value);

        public T Roll()
        {
            if (dictionary.Count == 0) return default;
            int roll = Main.rand.Next(TotalWeight);

            T ret = FromWeight (roll);

            if (ret.Equals(default(T))) return Roll();
            return ret;
        }

        public T FromWeight(int weight)
        {
            if (weight < 0) return default;

            int current = 0;
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

        public void Add(KeyValuePair<T, int> item)
            => dictionary.Add(item.Key, item.Value);

        public bool Contains(KeyValuePair<T, int> item)
        {
            return ContainsKey(item.Key) && this[item.Key] == item.Value;
        }

        public void CopyTo(KeyValuePair<T, int>[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach (KeyValuePair<T, int> element in dictionary)
            {
                array[i] = element;
                i++;
            }
        }

        public bool Remove(KeyValuePair<T, int> item)
        {
            if (Contains(item))
                return dictionary.Remove(item.Key);
            return false;
        }
    }
}