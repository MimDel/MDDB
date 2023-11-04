using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SAA_MDDB;

class MyList<T> : IEnumerable<T>
{
    private T[] _array;
    private int _capacity;
    public int Count { get; private set; }

    public T this[int index]
    {
        get
        {
            ValideteIndex(index);
            return _array[index];
        }
        set 
        {
            ValideteIndex(index);
            _array[index] = value;
        }
    }

    public MyList()
    {
        Count = 0;
        _capacity = 4;
        _array = new T[_capacity];
    }

    public void Add(T item)
    {
        Resize();
        _array[Count] = item;
        Count++;
    }

    public void Insert(int index, T item)
    {
        ValideteIndex(index);
        Resize();

        for (int i = Count; i >= index; i--)
            _array[i] = _array[i - 1];

        _array[index] = item;
        Count++;
    }

    public bool Remove(T item)
    {
        int index = -1;
        for (int i = 0; i < Count; i++)
        {
            if (_array[i].Equals(item))
            {
                index = i;
                RemoveAt(index);
                return true;
            }
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        ValideteIndex(index);

        for (int i = index; i < Count - 1; i++)
            _array[i] = _array[i + 1];

        Count--;
    }

    public void RemoveLast()
    {
        if (Count > 0)
            _array[Count - 1] = default;

        Count--;
    }

    public T[] ToArray()
    {
        var result  = new T[Count];
        for(int i = 0; i < result.Length;i++)
            result[i] = _array[i];
        return result;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return _array[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void Resize()
    {
        if (Count != _capacity)
            return;

        _capacity *= 2;
        var newArray = new T[_capacity];

        for (int i = 0; i < Count; i++)
            newArray[i] = _array[i];

        _array = newArray;
    }

    private void ValideteIndex(int index)
    {
        if (index < 0 || index >= Count)
            throw new IndexOutOfRangeException();
    }
}

