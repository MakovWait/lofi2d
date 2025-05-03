namespace Lofi.Core.Plugins;

public interface IPluginTarget<out T>
{
	Task Add(IPlugin<T> plugin);
}