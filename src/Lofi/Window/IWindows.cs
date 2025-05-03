using Lofi.Core;
using Lofi.Window.Components;

namespace Lofi.Window;

public interface IWindows
{
    IWindow Main { get; }

    void Start(WindowSettings settings, Input input);
    
    void Close();
}