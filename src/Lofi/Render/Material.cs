using Lofi.Asset;
using Lofi.Core.Comp;
using Raylib_cs;
using Lofi.Asset.Components;

namespace Lofi.Render;

public interface IMaterial
{
    public void Draw(IMaterialTarget item);
}

public class ShaderMaterial(Shader shader, IShaderMaterialParameters? parameters = null) : IMaterial
{
    public void Draw(IMaterialTarget item)
    {
        parameters?.Apply(shader);
        Raylib.BeginShaderMode(shader.ValueRl);
        item.Draw();
        Raylib.EndShaderMode();
    }
}

public interface IShaderMaterialParameters
{
    void Apply(Shader shader);
    
    public class Lambda(Action<Shader> lambda) : IShaderMaterialParameters
    {
        public void Apply(Shader shader)
        {
            lambda(shader);
        }
    }
}

public static class MaterialEx
{
    public static ShaderMaterial UseShaderMaterial(this INodeInit self, AssetPath path, IShaderMaterialParameters? parameters = null)
    {
        var shader = self.UseAsset<Shader>(path);
        var material = new ShaderMaterial(shader.Value, parameters);
        return material;
    }
}