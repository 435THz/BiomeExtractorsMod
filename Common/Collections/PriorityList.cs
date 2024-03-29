using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace BiomeExtractorsMod.Common.Collections
{
    internal class PriorityList<T> : IDictionary<int, List<T>>
    {
        private readonly LinkedList<int> priorityOrder;
        private readonly Dictionary<int, List<T>> dictionary;

        public List<T> this[int key]
        { 
            get => dictionary[key];
            set
            {
                dictionary[key] = value;
                AddPriority(key);
            }
        }

        private void AddPriority(int n)
        {
            LinkedListNode<int> node = priorityOrder.First;
            while(node != null)
            {
                if (n > node.Value)
                {
                    priorityOrder.AddBefore(node, n);
                    return;
                }
                else if (n == node.Value)
                    return;
                node = node.Next;
            }
            priorityOrder.AddLast(n);
        }

        public ICollection<int> Keys => dictionary.Keys;

        public ICollection<List<T>> Values => dictionary.Values;

        public int Count => dictionary.Count;

        public int ElementCount
        {
            get
            {
                int c = 0;
                foreach (int k in Keys)
                    c += dictionary[k].Count;
                return c;
            }
        }

        public bool IsReadOnly => false;

        public PriorityList()
        {
            dictionary = [];
            priorityOrder = [];
        }

        public void Add(int key, List<T> value)
        {
            if (!ContainsKey(key)) 
            {
                dictionary[key] = value;
                AddPriority(key);
            }
            else
            {
                foreach (T elem in value)
                    Add(key, elem);
            }
        }

        public void Add(int key, T element)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = [];
                AddPriority(key);
            }
            dictionary[key].Add(element);
        }

        public void Clear()
        {
            dictionary.Clear();
            priorityOrder.Clear();
        }
        public void Clear(int key)
            => Remove(key);

        public bool ContainsKey(int key)
            => dictionary.ContainsKey(key);

        public bool ContainsElement(T element)
        {
            foreach (int k in Keys)
                if(dictionary[k].Contains(element)) return true;
            return false;
        }

        public bool KeyContainsElement(int key, T element)
            => dictionary[key].Contains(element);

        public int GetPriorityOf(T element, int failValue = int.MinValue)
        {
            foreach (int k in Keys)
                if (dictionary[k].Contains(element)) return k;
            return failValue;
        }

        public bool Remove(int key)
        {
            priorityOrder.Remove(key);
            return dictionary.Remove(key);
        }

        public bool RemoveElement(int key, T element)
        {
            bool removed = dictionary[key].Remove(element);
            if (dictionary[key].Count == 0)
                Remove(key);
            return removed;
        }

        public bool TryGetValue(int key, [MaybeNullWhen(false)] out List<T> value)
            => dictionary.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<int, List<T>>> GetEnumerator()
            => dictionary.GetEnumerator();

        public IEnumerable<KeyValuePair<int,T>> EnumerateInOrder()
        {
            foreach (int p in priorityOrder)
                foreach(T elem in dictionary[p])
                    yield return new KeyValuePair<int, T> (p, elem);
        }




        public void Add(KeyValuePair<int, List<T>> item)
            => Add(item.Key, item.Value);

        public bool Contains(KeyValuePair<int, List<T>> item)
            => dictionary.Contains(item);

        public void CopyTo(KeyValuePair<int, List<T>>[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach (KeyValuePair<int, List<T>> element in dictionary)
            {
                array[i] = element;
                i++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => dictionary.GetEnumerator();

        public bool Remove(KeyValuePair<int, List<T>> item)
        {
            if (Contains(item))
                return Remove(item.Key);
            return false;
        }
    }
}