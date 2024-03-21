using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Terraria;

public class WeightedList<T> : IDictionary<T, int>
{
    private Dictionary<T,int> dictionary;
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
        dictionary = new Dictionary<T,int>();
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
        if (dictionary.ContainsKey(element))
            dictionary[element]++;
        else
            dictionary.Add(element, 1);
        TotalWeight++;
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
        if(dictionary.Count == 0) return default;
        int roll = Main.rand.Next(TotalWeight);
        int current = 0;
        T def = default;
        T ret = default;
        foreach(T key in Keys)
        {
            current += this[key];
            if (current >= roll && ret.Equals(def)) ret = key;
        }
        TotalWeight = current;
        if (ret.Equals(def)) return Roll();
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
        foreach(KeyValuePair<T, int> element in dictionary)
        {
            array[i] = element;
            i++;
        }
    }

    public bool Remove(KeyValuePair<T, int> item)
    {
        if(Contains(item))
            return dictionary.Remove(item.Key);
        return false;
    }
}