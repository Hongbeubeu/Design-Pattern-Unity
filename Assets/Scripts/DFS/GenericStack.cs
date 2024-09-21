using System.Collections.Generic;

public class GenericStack<T>
{
    private readonly List<(T from, T to)> _stack = new();

    public void Push((T from, T to) item)
    {
        _stack.Insert(0, item);
    }

    public void PushRange(List<(T from, T to)> items)
    {
        if (items == null || items.Count == 0) return;
        _stack.InsertRange(0, items);
    }

    public (T from, T to) Pop()
    {
        var item = _stack[0];
        _stack.RemoveAt(0);
        return item;
    }

    public (T from, T to) Peek()
    {
        var item = _stack[0];
        return item;
    }

    public void Clear()
    {
        _stack.Clear();
    }

    public int Count => _stack.Count;
}