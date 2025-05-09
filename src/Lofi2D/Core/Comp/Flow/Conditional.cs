namespace Lofi2D.Core.Comp.Flow;

public class Conditional : Component
{
    public required Signal<bool> When { get; init; }
    
    protected override Components Init(INodeInit self)
    {
        self.UseSignal(
            When, 
            new SignalTarget<bool>(Update).Throttled(self)
        );
        // TODO fix
        When.Emit(When.Value);

        return [];
    }
    
    private void Update(bool when)
    {
        if (when)
        {
            CreateChildrenAndMount(Children);   
        }
        else
        {
            ClearChildren();
        }
    }
}