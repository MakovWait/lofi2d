using Lofi2D.Core.Comp;
using Raylib_cs;

namespace Lofi2D.Audio.Components;

public class CAudioDeviceInit : Component
{
    protected override Core.Comp.Components Init(INodeInit self)
    {
        Raylib.InitAudioDevice();
        self.OnLateCleanup(Raylib.CloseAudioDevice);
        return base.Init(self);
    }
}