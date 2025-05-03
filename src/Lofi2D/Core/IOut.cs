using System.Diagnostics;
using Lofi2D.Core.Comp;

namespace Lofi2D.Core;

public interface IOut<T> : IScalar<T?>, IDisposable
{
    internal void Init(T value);
}

public static class OutEx
{
    public static void Init<T>(this INodeInit self, IOut<T> outToInit, T value)
    {
        outToInit.Init(value);
        self.AutoDispose(outToInit);
    }
}

public class Out<T> : IOut<T>
{
    public T? Value
    {
        get
        {
            Debug.Assert(!_disposed);
            return _value;
        }
        private set => _value = value;
    }

    private bool _initialized;
    private bool _disposed;
    private T? _value;

    public void Init(T value)
    {
        Debug.Assert(!_initialized);
        Value = value;
        _initialized = true;
    }

    public void Dispose()
    {
        _disposed = true;
        GC.SuppressFinalize(this);
        Value = default;
    }
}
