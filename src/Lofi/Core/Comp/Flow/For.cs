using System.Collections;
using System.Diagnostics;

namespace Lofi.Core.Comp.Flow;

public class CFor<T> : Component
{
    public required Signal<IReadOnlyList<T>> In { get; init; }

    public required Func<T, string> ItemKey { get; init; }
    
    public required Func<T, int, IComponent> Render { get; init; }
    
    protected override Components Init(INodeInit self)
    {
        Dictionary<string, Node> nodes = new();
        Dictionary<string, Node> nodesCopy = new();

        self.UseSignal(
            In,
            new SignalTarget<IReadOnlyList<T>>(Update).Throttled(self)
        );

        return [];
        
        void Update(IReadOnlyList<T> items)
        {
            foreach (var node in nodes)
            {
                nodesCopy.Add(node.Key, node.Value);
                Self.RemoveChild(node.Value);
            }
            nodes.Clear();

            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (nodesCopy.TryGetValue(ItemKey(item), out var n))
                {
                    RegisterNode(ItemKey(item), n);
                    nodesCopy.Remove(ItemKey(item));
                    Self.AddChild(n);
                }
                else
                {
                    var node = CreateChildAndMount(Render(item, i));
                    RegisterNode(ItemKey(item), node);
                }
            }
        
            foreach (var node in nodesCopy.Values)
            {
                node.Free();
            }
            nodesCopy.Clear();
        }
        
        void RegisterNode(string key, Node node)
        {
            Debug.Assert(!nodes.ContainsKey(key));
            nodes.Add(key, node);
        }
    }
}

public class ReactiveList<T> : IEnumerable<T>
{
    public readonly Signal<IReadOnlyList<T>> Changed = new();

    private readonly List<T> _items = [];
    private readonly List<T> _queuedToRemove = [];

    public int Count => _items.Count;

    public T this[int idx] => _items[idx];
    
    public void Add(T item)
    {
        _items.Add(item);
        EmitChanged();
    }

    public void QueueRemove(T item)
    {
        _queuedToRemove.Add(item);
    }

    public void FlushRemoveQueue()
    {
        foreach (var item in _queuedToRemove)
        {
            _items.Remove(item);
        }
        _queuedToRemove.Clear();
        EmitChanged();
    }
    
    public void Remove(T item)
    {
        _items.Remove(item);
        EmitChanged();
    }
    
    public void Clear()
    {
        _items.Clear();
        EmitChanged();
    }
    
    public List<T>.Enumerator GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void EmitChanged()
    {
        Changed.Emit(_items);
    }
}
