using Lofi2D.Asset;
using Lofi2D.Asset.Format;
using Lofi2D.Asset.Util;
using Lofi2D.Math;

namespace Lofi2D.Render.Texture;

public class TextureRegion2D(IAsset<ITexture2D> origin, Rect2 srcRect) : ITexture2D, ISerializable
{
    public void Draw(IDrawContext ctx, Vector2 position, Color modulate)
    {
        origin.Value.DrawTextureRectRegion(ctx, new Rect2(position, srcRect.Size), srcRect, modulate);
    }

    public void DrawTextureRect(IDrawContext ctx, Rect2 rect, Color modulate)
    {
        origin.Value.DrawTextureRectRegion(ctx, rect, srcRect, modulate);
    }

    public void DrawTextureRectRegion(IDrawContext ctx, Rect2 rect, Rect2 srcRectCustom, Color modulate)
    {
        origin.Value.DrawTextureRectRegion(ctx, rect, srcRect.Intersection(srcRectCustom), modulate);
    }

    void ISerializable.WriteTo(ISerializeOutput output)
    {
        output.Write(nameof(origin), origin);
        output.Write(nameof(srcRect), srcRect);
    }

    public class Deserializer : IAssetDeserializer<TextureRegion2D>
    {
        public TextureRegion2D From(ISerializeInput input)
        {
            return new TextureRegion2D(
                input.ReadSubRes<ITexture2D>(nameof(origin)),
                input.ReadRect2(nameof(srcRect))
            );
        }

        public Y Deserialize<Y>(ISerializeInput input, IResultMapper<Y> resultMapper)
        {
            return resultMapper.Map(From(input));
        }

        public bool MatchType(string type)
        {
            return type == nameof(TextureRegion2D);
        }
    }
}
