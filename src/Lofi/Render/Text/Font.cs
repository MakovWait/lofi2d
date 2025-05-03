using Lofi.Asset;
using Lofi.Asset.Util;
using Raylib_cs;

namespace Lofi.Render.Text;

public class Font(_Font origin)
{
    public void DrawText(
        IDrawContext ctx, Vector2 pos, string text, float size, Color color, TextOutline? outline = null
    )
    {
        // var defaultSize = origin.BaseSize;
        // var spacing = size / defaultSize;
        ctx.DrawText(origin, pos, text, 0f, size, color, outline);
    }
}

public class DefaultFontLoader : IAssetLoader
{
    private static readonly string[] SupportedExtensions = ["ttf"];

    public bool MatchPath(AssetPath path)
    {
        return SupportedExtensions.Contains(path.Extension);
    }

    public T Load<T>(AssetPath path, IAssetsSource subAssets, IResultMapper<T> target)
    {
        return target.Map(new Font(Raylib.LoadFont(path.FilePath)));
    }
}