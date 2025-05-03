using Lofi;
using Lofi.Audio.Components;
using Lofi.Project;
using Lofi.Window.Components;

var app = new App(new CAudioDeviceInit()
{
    new CWindowsRl()
    {
        Project.GetRoot()
    }
});
await app.Run();
