using Tmp;
using Tmp.Audio.Components;
using Tmp.Project;
using Tmp.Window.Components;

var app = new App(new CAudioDeviceInit()
{
    new CWindowsRl()
    {
        Project.GetRoot()
    }
});
await app.Run();
