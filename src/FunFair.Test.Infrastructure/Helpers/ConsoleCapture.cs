using System;
using System.IO;

namespace FunFair.Test.Infrastructure.Helpers;

public sealed class ConsoleCapture : IDisposable
{
    private readonly StringWriter _stdErrWriter;
    private readonly StringWriter _stdOutWriter;
    private readonly TextWriter _originalOut;
    private readonly TextWriter _originalError;
    private bool _disposed;

    public ConsoleCapture()
    {
        this._stdOutWriter = new StringWriter();
        this._stdErrWriter = new StringWriter();
        this._originalOut = Console.Out;
        this._originalError = Console.Error;
        Console.SetOut(this._stdOutWriter);
        Console.SetError(this._stdErrWriter);
    }

    public string StdOut => this._stdOutWriter.ToString();

    public string StdErr => this._stdErrWriter.ToString();

    public void Dispose()
    {
        if (!this._disposed)
        {
            this._disposed = true;
            Console.SetOut(this._originalOut);
            Console.SetError(this._originalError);
            this._stdOutWriter.Dispose();
            this._stdErrWriter.Dispose();
        }
    }
}
