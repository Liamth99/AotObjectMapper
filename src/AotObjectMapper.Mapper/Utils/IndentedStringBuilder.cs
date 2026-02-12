using System;
using System.Text;

namespace AotObjectMapper.Mapper.Utils;

/// <summary>
/// A utility class for building strings with support for custom indentation and line formatting.
/// </summary>
public sealed class IndentedStringBuilder
{
    private readonly StringBuilder _sb;
    private          bool          _atStartOfLine = true;

    public string IndentString { get; }
    public int    IndentLevel  { get; set; }

    public IndentedStringBuilder(string indentString = "    ")
    {
        _sb          = new StringBuilder();
        IndentString = indentString ?? throw new ArgumentNullException(nameof(indentString));
    }

    public IndentedStringBuilder(StringBuilder stringBuilder, string indentString = "    ")
    {
        _sb          = stringBuilder;
        IndentString = indentString ?? throw new ArgumentNullException(nameof(indentString));
    }

    public IDisposable Indent()
    {
        return new IndentScope(this);
    }

    private sealed class IndentScope : IDisposable
    {
        private IndentedStringBuilder _parent;
        private bool                  _disposed;

        public IndentScope(IndentedStringBuilder parent)
        {
            _parent = parent;
            _parent.IndentLevel++;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _parent.IndentLevel--;
                _disposed = true;
            }
        }
    }

    public IDisposable IndentBlock(string header, string open = "{", string close = "}")
    {
        return new IndentBlockScope(this, header, open, close);
    }

    private sealed class IndentBlockScope : IDisposable
    {
        private IndentedStringBuilder _parent;
        private bool                  _disposed;
        private string                _close;

        public IndentBlockScope(IndentedStringBuilder parent, string header, string open = "{", string close = "}")
        {
            _parent = parent;
            _parent.AppendLine(header);
            _parent.AppendLine(open);
            _parent.IndentLevel++;
            _close = close;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _parent.IndentLevel--;
                _parent.AppendLine(_close);
                _disposed = true;
            }
        }
    }

    private void WriteIndentIfNeeded()
    {
        if (_atStartOfLine)
        {
            for (int i = 0; i < IndentLevel; i++)
            {
                _sb.Append(IndentString);
            }

            _atStartOfLine = false;
        }
    }

    /// <inheritdoc cref="StringBuilder.Append(string)"/>
    public IndentedStringBuilder Append(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return this;

        foreach (char c in value!)
        {
            WriteIndentIfNeeded();
            _sb.Append(c);

            if (c is '\n')
                _atStartOfLine = true;
        }

        return this;
    }

    /// <inheritdoc cref="StringBuilder.AppendLine()"/>
    public IndentedStringBuilder AppendLine()
    {
        _sb.AppendLine();
        _atStartOfLine = true;
        return this;
    }

    /// <inheritdoc cref="StringBuilder.AppendLine(string)"/>
    public IndentedStringBuilder AppendLine(string value)
    {
        Append(value);
        _sb.AppendLine();
        _atStartOfLine = true;
        return this;
    }

    /// <inheritdoc cref="StringBuilder.ToString()"/>
    public override string ToString()
    {
        return _sb.ToString();
    }

    /// <inheritdoc cref="StringBuilder.Clear()"/>
    public void Clear()
    {
        _sb.Clear();
        _atStartOfLine = true;
        IndentLevel    = 0;
    }
}