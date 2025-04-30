using Raylib_cs;
using Tmp.Core.Comp;

namespace Tmp.Audio.Components;

public class CAudioDeviceInit : Component
{
    protected override Core.Comp.Components Init(INodeInit self)
    {
        Raylib.InitAudioDevice();
        self.OnLateCleanup(Raylib.CloseAudioDevice);
        return base.Init(self);
    }
}