using Tmp.Core.Comp;

namespace Tmp.Audio.Components;

public class CAudioBusLayout(MasterBus masterBus) : Component
{
    protected override Core.Comp.Components Init(INodeInit self)
    {
        self.CreateContext(new AudioBusLayout(masterBus));
        return base.Init(self);
    }
}

public static class CAudioBusLayoutEx
{
    public static AudioBus UseAudioBus(this INodeInit self, string name)
    {
        var layout = self.UseContext<AudioBusLayout>();
        return layout.GetBus(name);
    }
}