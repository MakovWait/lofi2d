using Lofi.Core.Comp;
using Raylib_cs;

namespace Lofi.Audio.Components;

public class CAudioDeviceInit : Component
{
    protected override Core.Comp.Components Init(INodeInit self)
    {
        Raylib.InitAudioDevice();
        self.OnLateCleanup(Raylib.CloseAudioDevice);
        return base.Init(self);
    }
}