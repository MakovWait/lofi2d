namespace Lofi2D.Asset.Util;

public interface IResultMapper<Y>
{
    Y Map<T>(T result);
}

public static class ResultMapper<T>
{
    private static IResultMapper<T>? _asIs;
    public static IResultMapper<T> AsIs => _asIs ??= new ResultMapperAsIs();
    
    private class ResultMapperAsIs : IResultMapper<T>
    {
        public T Map<Y>(Y result)
        {
            if (result is T target)
            {
                return target;
            }
            throw new ArgumentException($"Result must be of type {typeof(T)}");
        }
    }
}