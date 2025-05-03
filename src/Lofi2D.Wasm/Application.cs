using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Lofi2D.Audio.Components;
using Lofi2D.Window.Components;

namespace Lofi2D.Wasm;

public partial class Application
{
    private static App? _app;

    public static async Task Main()
    {
        _app = new App(
            new CWindowsRl
            {
                new CAudioDeviceInit()
                {
                    Lofi2D.Project.Project.GetRoot()
                }
            }
        );
        _app.SetRunner(new AppRunnerBrowser());
        await _app.Run();
    }

    [JSExport]
    public static void UpdateFrame()
    {
        _app!.Update();
    }
}

internal class AppRunnerBrowser : IAppRunner
{
    public void Run(IRunnableApp app)
    {
        app.Start();
    }
}