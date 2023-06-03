using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace EasyPlot.Utilities;

internal ref struct ValueStringBuilder
{
    private readonly CultureInfo? _culture;

    private char[]? _rentChars;
    private Span<char> _buffer;
    private uint _length;

    private bool Disposed
    {
        readonly get => (_length & 0x80000000) == 0x80000000;
        set
        {
            if (value) _length ^= 0x80000000;
            else _length &= 0x7FFFFFFF;
        }
    }

    /// <summary>
    /// 文字列の長さです
    /// </summary>
    public int Length
    {
        readonly get => unchecked((int)(_length & 0x7FFFFFFF));
        set
        {
            _length = unchecked((uint)value);
            if (Length < value)
                ThrowOverflowException();
        }
    }

    public ValueStringBuilder() : this(null) { }

    public ValueStringBuilder(CultureInfo? cultureInfo)
    {
        _culture = cultureInfo;
        _length = 0;

        _rentChars = ArrayPool<char>.Shared.Rent(256);
        _buffer = _rentChars.AsSpan();
    }

    public void Append(string value) => Append(value.AsSpan());

    public void Append(ReadOnlySpan<char> value)
    {
        try
        {
            if (Disposed)
                ThrowObjectDisposedException();

            EnsureBuffer(Length + value.Length);

            value.CopyTo(_buffer[Length..]);
            Length += value.Length;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public void Append<T>(T value, ReadOnlySpan<char> format = default) where T : ISpanFormattable
    {
        if (Disposed)
            ThrowObjectDisposedException();

        int charsWritten;
        while (!value.TryFormat(_buffer[Length..], out charsWritten, format, _culture))
        {
            EnsureBuffer(_buffer.Length + 10);
        }

        Length += charsWritten;
    }

    public void Append([InterpolatedStringHandlerArgument("")] scoped AppendInterporatedHandler handler)
    {
        ref var childBuilder = ref handler.GetInnerBuilder();

        if (Disposed)
            ThrowObjectDisposedException();
        else if (Length != handler.ParentLength)
            ThrowInvalidOperationException();

        if (_rentChars == childBuilder._rentChars)
        {
            Length = childBuilder.Length;
            return;
        }

        if (_rentChars is not null)
            ArrayPool<char>.Shared.Return(_rentChars);
        _rentChars = childBuilder._rentChars;
        _buffer = _rentChars.AsSpan();
        Length = childBuilder.Length;
    }

    /// <summary>
    /// destination に書き込みます。 charsWritten には書き込まれた長さが返されます。
    /// </summary>
    /// <returns>書き込みに成功した場合は true です。</returns>
    public readonly bool TryWriteTo(Span<char> destination, out int charsWritten)
    {
        if (Disposed || Length > destination.Length)
        {
            charsWritten = 0;
            return false;
        }

        charsWritten = Length;
        return _buffer[..Length].TryCopyTo(destination);
    }

    public override readonly string ToString()
    {
        if (Disposed)
            ThrowObjectDisposedException();

        return _buffer[..Length].ToString();
    }

    /// <summary>
    /// 直接バッファを操作できます
    /// </summary>
    public readonly Span<char> GetRawSpan() => Disposed ? default : _buffer[..Length];

    /// <summary>
    /// 
    /// </summary>
    /// <returns>破棄済みの場合は false</returns>
    [MemberNotNullWhen(true, nameof(_rentChars))]
    private bool EnsureBuffer(int required)
    {
        if (Disposed)
            return false;

        if (_rentChars is null)
        {
            _rentChars = ArrayPool<char>.Shared.Rent(int.Max(256, required));
            _buffer = _rentChars.AsSpan();
        }
        else if (_rentChars.Length < required)
        {
            var next = ArrayPool<char>.Shared.Rent(required);
            _buffer[..Length].CopyTo(next);
            ArrayPool<char>.Shared.Return(_rentChars);
            _rentChars = next;
            _buffer = _rentChars.AsSpan();
        }
        return true;
    }

    private void ApplyFrom(scoped in ValueStringBuilder childBuilder, int parentLength)
    {
        if (Disposed)
            ThrowObjectDisposedException();
        else if (Length != parentLength)
            ThrowInvalidOperationException();

        if (_rentChars == childBuilder._rentChars)
        {
            Length = childBuilder.Length;
            return;
        }

        if (_rentChars is not null)
            ArrayPool<char>.Shared.Return(_rentChars);
        _rentChars = childBuilder._rentChars;
        _buffer = _rentChars.AsSpan();
        Length = childBuilder.Length;
    }

    public void Dispose()
    {
        if (Disposed)
            return;

        if (_rentChars is not null)
        {
            ArrayPool<char>.Shared.Return(_rentChars);
            _rentChars = null;
        }
        Disposed = true;
    }

    [DoesNotReturn]
    static void ThrowObjectDisposedException() => throw new ObjectDisposedException(nameof(ValueStringBuilder));

    [DoesNotReturn]
    static void ThrowInvalidOperationException() => throw new InvalidOperationException();

    [DoesNotReturn]
    static void ThrowOverflowException() => throw new OverflowException();

    [InterpolatedStringHandler]
    public ref struct AppendInterporatedHandler
    {
        private ValueStringBuilder _builder;
        public readonly int ParentLength;

        public AppendInterporatedHandler(int literalLength, int formattedCount, in ValueStringBuilder builder)
        {
            _builder = builder;
            ParentLength = builder.Length;
            _builder.EnsureBuffer(literalLength + formattedCount * 4);
        }

        public AppendInterporatedHandler(int literalLength, int formattedCount, in AppendInterporatedHandler handler)
        {
            _builder = handler._builder;
            _builder.EnsureBuffer(literalLength + formattedCount * 4);
        }

        public void AppendLiteral(string literal)
        {
            _builder.Append(literal);
        }

        public void AppendFormatted(string value)
        {
            _builder.Append(value);
        }

        public void AppendFormatted(ReadOnlySpan<char> value)
        {
            _builder.Append(value);
        }

        public void AppendFormatted<T>(T value) where T : ISpanFormattable
        {
            _builder.Append(value);
        }

        public void AppendFormatted<T>(T value, ReadOnlySpan<char> format) where T : ISpanFormattable
        {
            _builder.Append(value, format);
        }

        public void AppendFormatted([InterpolatedStringHandlerArgument("")] in AppendInterporatedHandler handler)
        {
            _builder.ApplyFrom(handler._builder, handler.ParentLength);
        }

        /// <summary>
        /// 内部の ValueStringBuilder を取得します。
        /// </summary>
        [UnscopedRef]
        internal ref ValueStringBuilder GetInnerBuilder()
        {
            return ref _builder;
        }
    }
}
