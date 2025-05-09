using System.Collections;
using Lofi2D.Core.Comp.Flow;

namespace Lofi2D.Core.Comp;

public class Component : IComponent, IEnumerable<IComponent>
{
    public Components Children { get; init; } = [];
    public string? Name { get; set; } = null;

    private Node? _self;
    private Tree? _tree;

    protected Tree Tree => _tree!;
    protected Node Self => _self!;

    public Node Build(Tree tree, Node? parent)
    {
        _tree = tree;
        _self = tree.CreateNode(Name ?? DefaultName());
        parent?.AddChild(_self);
        
        _self.Init(_ =>
        {
            CreateChildren(Init(_self));
        });
        
        return _self;
    }

    private string DefaultName()
    {
        var name = GetType().Name;
        // if (name.Equals("Component") || name.Equals("ComponentFunc"))
        // {
        //     name = "Node";
        // }
        return name;
    }
    
    public Component WithName(string name)
    {
        Name = name;
        return this;
    }

    protected virtual Components Init(INodeInit self)
    {
        return Children;
    }
    
    protected Node CreateChild(IComponent component)
    {
        return component.Build(Tree, Self);
    }
    
    protected Node CreateChildAndMount(IComponent component)
    {
        var child = CreateChild(component);
        child.Mount();
        return child;
    }
    
    protected void CreateChildren(Components components)
    {
        foreach (var component in components)
        {
            component.Build(Tree, Self);
        }
    }
    
    protected void CreateChildrenAndMount(Components components)
    {
        foreach (var component in components)
        {
            var child = component.Build(Tree, Self);
            child.Mount();
        }
    }

    protected void ClearChildren()
    {
        Self.ClearChildren();
    }
    
    protected void ReplaceChildren(Components components)
    {
        ClearChildren();
        CreateChildren(components);
    }
    
    public void Add(IComponent component)
    {
        Children.Add(component);
    }

    IEnumerator<IComponent> IEnumerable<IComponent>.GetEnumerator()
    {
        return Children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Children.GetEnumerator();
    }
}

public class CFunc(Func<INodeInit, Components, Components> init) : Component
{
    public CFunc(Func<INodeInit, Components> init): this((self, _) => init(self))
    {
        
    }
    
    protected override Components Init(INodeInit self)
    {
        return init(self, Children);
    }
}

public class Components : IEnumerable<IComponent>
{
    private List<IComponent> Children { get; init; } = [];
    
    public static implicit operator Components(Component component) => component.AsChildren();
    
    public void Add(IComponent component)
    {
        Children.Add(component);
    }

    public List<IComponent>.Enumerator GetEnumerator()
    {
        return Children.GetEnumerator();
    }
    
    IEnumerator<IComponent> IEnumerable<IComponent>.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public static class ComponentEx
{
    public static Components AsChildren(this IComponent self)
    {
        return [self];
    }
    
    public static Conditional If(this IComponent self, Signal<bool> condition)
    {
        return new Conditional
        {
            When = condition,
            Children = self.AsChildren()
        };
    }
    
    public static Conditional If(this IComponent self, bool condition)
    {
        return self.If(new Signal<bool>(condition));
    }
}

public interface IComponent
{
    Node Build(Tree tree, Node? parent);
}