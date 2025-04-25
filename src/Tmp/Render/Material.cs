using Raylib_cs;
using Tmp.Asset;
using Tmp.Asset.Components;
using Tmp.Core.Comp;

namespace Tmp.Render;

public interface IMaterial
{
    public void Draw(IMaterialTarget item);
}

public class ShaderMaterial(Shader shader) : IMaterial
{
    public void Draw(IMaterialTarget item)
    {
        Raylib.BeginShaderMode(shader.ValueRl);
        item.Draw();
        Raylib.EndShaderMode();
    }
}

public static class MaterialEx
{
    public static ShaderMaterial UseShaderMaterial(this INodeInit self, AssetPath path)
    {
        var shader = self.UseAsset<Shader>(path);
        var material = new ShaderMaterial(shader.Value);
        return material;
    }
}