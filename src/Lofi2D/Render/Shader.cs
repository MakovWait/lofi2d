using System.Numerics;
using Lofi2D.Asset;
using Lofi2D.Asset.Format;
using Lofi2D.Asset.Util;
using Lofi2D.IO;
using Raylib_cs;

namespace Lofi2D.Render;

public class Shader(IRlShaderSource shaderSource) : IDisposable
{
    public _Shader ValueRl => GetOrLoad();

    private _Shader? _shader;

    public void SetUniform(string name, float value)
    {
        var shader = GetOrLoad();
        var loc = Raylib.GetShaderLocation(shader, name);
        Raylib.SetShaderValue(shader, loc, value, ShaderUniformDataType.Float);
    }
    
    public void SetUniform(string name, Color value)
    {
        var shader = GetOrLoad();
        var loc = Raylib.GetShaderLocation(shader, name);
        Raylib.SetShaderValue(shader, loc, new Vector4(value.R, value.G, value.B, value.A), ShaderUniformDataType.Vec4);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Unload();
    }

    public void Reload()
    {
        Unload();
        GetOrLoad();
    }
    
    private _Shader GetOrLoad()
    {
        return _shader ??= shaderSource.Load();
    }

    private void Unload()
    {
        if (_shader.HasValue)
        {
            Raylib.UnloadShader(_shader.Value);
        }
    }
    
    public class Deserializer : IAssetDeserializer<Shader>
    {
        public Shader From(ISerializeInput input)
        {
            return new Shader(
                new RlShaderSourceFile(
                    AssetPath.FromPath(input.ReadString("vs")),
                    AssetPath.FromPath(input.ReadString("fs"))
                )
            );
        }

        public bool MatchType(string type)
        {
            return type == nameof(Shader);
        }

        public Y Deserialize<Y>(ISerializeInput input, IResultMapper<Y> resultMapper)
        {
            return resultMapper.Map(From(input));
        }
    }
}

public interface IRlShaderSource
{
    public _Shader Load();
}

public class RlShaderSourceFile(FilePath vs, FilePath fs) : IRlShaderSource
{
    public _Shader Load()
    {
        return Raylib.LoadShader(vs.ValueOrNullIfEmpty, fs.ValueOrNullIfEmpty);
    }
}