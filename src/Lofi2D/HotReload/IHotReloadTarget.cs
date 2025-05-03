namespace Lofi2D.HotReload;

public interface IHotReloadTarget
{
    public void ClearCache(Type[]? types);

    public void UpdateApplication(Type[]? types);
}