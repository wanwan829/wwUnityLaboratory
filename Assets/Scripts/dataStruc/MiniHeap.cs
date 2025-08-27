using System;
using System.Collections.Generic;

public class MiniHeap<T> where T : IComparable<T>
{
    private readonly List<T> _heap = new List<T>();
    public int Count { get { return _heap.Count; } }

    public void Push(T item)
    {
    
        _heap.Add(item);
        ShiftUp(_heap.Count - 1);
    }

    public T Pop()
    {
      
        T top = _heap[0];
        _heap[0] = _heap[_heap.Count-1];
        _heap.RemoveAt(_heap.Count - 1);
        ShiftDown(0);
        return top;
    }

    private void ShiftUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (_heap[parent].CompareTo(_heap[index]) <= 0) break;
            (_heap[parent], _heap[index]) = (_heap[index], _heap[parent]);
            index = parent;
        }
    }

    private void ShiftDown(int index)
    {
        int left = 2 * index + 1;
        while (left < _heap.Count)
        {
            int smaller = left;
            int right = left + 1;
            if (right < _heap.Count && _heap[right].CompareTo(_heap[left]) < 0)
                smaller = right;

            if (_heap[index].CompareTo(_heap[smaller]) <= 0) break;
            (_heap[index], _heap[smaller]) = (_heap[smaller], _heap[index]);
            index = smaller;
            left = 2 * index + 1;
        }
    }
}