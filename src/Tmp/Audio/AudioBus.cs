using System.Collections;

namespace Tmp.Audio;

public class AudioBus(string name) : IEnumerable<AudioBus>
{
    public string Name => name;
    public Decibels Volume { get; set; } = Decibels.Default;
    public IReadOnlyList<AudioBus> Children
    {
        init => _children = value.ToList();
    }

    public Decibels FinalVolume => _parent?.FinalVolume + Volume ?? Volume;

    private AudioBus? _parent;
    private bool _sealed;
    private readonly List<AudioBus> _children = [];
    
    public void Add(AudioBus bus)
    {
        if (_sealed)
        {
            throw new InvalidOperationException($"Cannot add bus to sealed AudioBus '{Name}'");
        }
        bus._parent = this;
        _children.Add(bus);
    }

    internal void SealRecursive()
    {
        _sealed = true;
        foreach (var child in _children)
        {
            child.SealRecursive();
        }
    }
    
    internal void AddToRegistryRecursive(Dictionary<string, AudioBus> registry)
    {
        if (registry.ContainsKey(Name))
        {
            throw new InvalidOperationException($"AudioBus with name '{Name}' already exists in the registry");
        }
        registry[Name] = this;
        foreach (var child in _children)
        {
            child.AddToRegistryRecursive(registry);
        }
    }

    public IEnumerator<AudioBus> GetEnumerator()
    {
        throw new NotSupportedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class MasterBus() : AudioBus("Master");

public class AudioBusLayout
{
    public MasterBus Master { get; }

    private readonly Dictionary<string, AudioBus> _allBuses = [];

    public AudioBusLayout(MasterBus masterBus)
    {
        Master = masterBus;
        Master.SealRecursive();
        Master.AddToRegistryRecursive(_allBuses);
    }
    
    public AudioBus GetBus(string name)
    { 
        return _allBuses[name];
    }
}