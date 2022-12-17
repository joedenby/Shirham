using System.Collections;
using UnityEngine;
using System;

public class Heap<T> where T : IHeapItem<T>
{
    T[] elements;
    public int Length { get; private set; }
    public int calls { get; private set; }

    public Heap(int maxSize) { 
        elements = new T[maxSize];
        calls = 0;
    }

    public void Add(T item) {
        if (item.HeapIndex >= elements.Length) {
            Debug.LogError($"Can't expand out of Heap bounds. Max size {elements.Length}");
        }

        item.HeapIndex = Length;
        elements[Length] = item;
        SortUp(item);
        Length++;
    }

    public T RemoveFirst() { 
        T firstItem = elements[0];
        Length--;
        elements[0] = elements[Length];
        elements[0].HeapIndex = 0;
        SortDown(elements[0]);
        return firstItem;
    }

    public void UpdateItem(T item) {
        SortUp(item);
    }

    public int Count {
        get {
            return Length;
        }
    }

    public bool Contains(T item) {
        return Equals(elements[item.HeapIndex], item);
    }

    private void SortDown(T item) {
        while (true) {
            calls++;
            int l = item.HeapIndex * 2 + 1;
            int r = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (l < Length)
            {
                swapIndex = l;

                if (r < Length)
                {
                    if (elements[l].CompareTo(elements[r]) < 0)
                    {
                        swapIndex = r;
                    }
                }

                if (item.CompareTo(elements[swapIndex]) < 0)
                {
                    Swap(item, elements[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else {
                return;
            }
        }
    }

    private void SortUp(T item) {
        int parentIndex = (item.HeapIndex - 1) / 2;
        calls++;
        while (true) {
            T parentItem = elements[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    private void Swap(T a, T b) {
        elements[a.HeapIndex] = b;
        elements[b.HeapIndex] = a;

        int aIndex = a.HeapIndex;
        a.HeapIndex = b.HeapIndex;
        b.HeapIndex = aIndex;
    }
}

public interface IHeapItem<T> :IComparable<T> { 
    int HeapIndex { get; set; }
}