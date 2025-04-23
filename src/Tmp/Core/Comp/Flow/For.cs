using System.Collections;
using System.Diagnostics;

namespace Tmp.Core.Comp.Flow;

public class For<T> : Component
{
    public required ReactiveList<T> In { get; init; }

    public required Func<T, string> ItemKey { get; init; }
    
    public required Func<T, int, IComponent> Render { get; init; }
    
    protected override Components Init(INodeInit self)
    {
        Dictionary<string, Node> nodes = new();
        Dictionary<string, Node> nodesCopy = new();

        self.UseSignal(
            In.Changed,
            new SignalTarget(Update).Throttled(self)
        );

        return [];
        
        void Update()
        {
            var items = In;
            
            foreach (var node in nodes)
            {
                nodesCopy.Add(node.Key, node.Value);
                Self.RemoveChild(node.Value);
            }
            nodes.Clear();
        
            var idx = 0;
            foreach (var item in items)
            {
                if (nodesCopy.TryGetValue(ItemKey(item), out var n))
                {
                    RegisterNode(ItemKey(item), n);
                    nodesCopy.Remove(ItemKey(item));
                    Self.AddChild(n);
                }
                else
                {
                    var node = CreateChildAndMount(Render(item, idx));
                    RegisterNode(ItemKey(item), node);
                }
                idx++;
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
    public readonly Signal Changed = new();

    private readonly List<T> _items = [];
    private readonly List<T> _queuedToRemove = [];

    public int Count => _items.Count;

    public T Get(int idx)
    {
        return _items[idx];
    }
    
    public void Add(T item)
    {
        _items.Add(item);
        Changed.Emit();
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
        Changed.Emit();
    }
    
    public void Remove(T item)
    {
        _items.Remove(item);
        Changed.Emit();
    }
    
    public void Clear()
    {
        _items.Clear();
        Changed.Emit();
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
}
