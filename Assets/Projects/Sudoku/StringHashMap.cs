using System;
using System.Collections;
using System.Collections.Generic;

public class StringHashMap<T> : IEnumerable<(string, T)>
{
    class Entry
    {
        public string key;
        public T value;
        public Entry next;

        public Entry(string key, T value)
        {
            this.key = key;
            this.value = value;
        }
    }

    readonly Entry[] data;

    public T this[string key]
    {
        get
        {
            if (TryGetValue(key, out var entry))
                return entry;
            else
                throw new Exception("Key Missing");
        }
        set
        {
            var entry = GetEntry(key);
            if (entry == null)
                Add(key, value);
            else
                entry.value = value;
        }
    }

    Entry GetEntry(string key)
    {
        int index = GetIndex(key);

        Entry node = data[index];

        while (node != null)
        {
            if (node.key.Equals(key)) return node;
            node = node.next;
        }

        return null;
    }

    public StringHashMap(int capacity = 128)
    {
        data = new Entry[capacity];
    }

    int GetIndex(string key)
    {
        int hash = key.GetHashCode();

        return Math.Abs(hash % data.Length);
    }

    public void Add(string key, T value)
    {
        var index = GetIndex(key);

        if (data[index] == null)
        {
            data[index] = new Entry(key, value);
            return;
        }

        if (data[index].key.Equals(key))
            return;

        var last = data[index];
        while (last.next != null)
        {
            last = last.next;
            if (last.key.Equals(key))
                return;
        }

        last.next = new Entry(key, value);
    }

    public void RemoveKey(string key)
    {
        int index = GetIndex(key);

        if (data[index].next == null)
        {
            data[index] = null;
            return;
        }

        Entry prev = null;
        Entry toRemove = data[index];
        while (!toRemove.key.Equals(key))
        {
            prev = toRemove;
            toRemove = toRemove.next;
        }

        if (prev == null)
            data[index] = toRemove.next;
        else
            prev.next = toRemove.next;

        toRemove.next = null;
    }

    public bool ContainsKey(string key)
    {
        var index = GetIndex(key);

        Entry node = data[index];
        while (node != null)
        {
            if (node.key.Equals(key))
                return true;
            node = node.next;
        }

        return false;
    }

    public bool TryGetValue(string key, out T value)
    {
        int index = GetIndex(key);

        Entry node = data[index];

        while (node != null)
        {
            if (node.key.Equals(key))
            {
                value = node.value;
                return true;
            }
            node = node.next;
        }

        value = default;
        return false;
    }

    public IEnumerator<(string, T)> GetEnumerator()
    {
        foreach (var item in data)
        {
            var current = item;
            while (current != null)
            {
                yield return (item.key, item.value);
                current = current.next;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}