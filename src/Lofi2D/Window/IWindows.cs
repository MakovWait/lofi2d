using Lofi2D.Core;
using Lofi2D.Window.Components;

namespace Lofi2D.Window;

public interface IWindows
{
    IWindow Main { get; }

    void Start(WindowSettings settings, Input input);
    
    void Close();
}