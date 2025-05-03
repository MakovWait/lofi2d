using Lofi2D;
using Lofi2D.Audio.Components;
using Lofi2D.Project;
using Lofi2D.Window.Components;

var app = new App(new CAudioDeviceInit()
{
    new CWindowsRl()
    {
        Project.GetRoot()
    }
});
await app.Run();
