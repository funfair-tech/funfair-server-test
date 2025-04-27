using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Xunit;

namespace FunFair.Test.Common;

[SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "Base class for further tests")]
public abstract class LoggingFolderCleanupTestBase : LoggingTestBase
{
    protected LoggingFolderCleanupTestBase(ITestOutputHelper output)
        : base(output)
    {
        this.TempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        DirectoryInfo created = Directory.CreateDirectory(this.TempFolder);

        this.Output.WriteLine($"Using Temp folder: {created.FullName}");
    }

    protected string TempFolder { get; }

    protected override void Dispose(bool disposing)
    {
        MurderTempFolder(this.TempFolder);
        base.Dispose(disposing);
    }

    private static void MurderTempFolder(string folderToMurder)
    {
        try
        {
            Directory.Delete(path: folderToMurder, recursive: true);
        }
        catch (Exception exception)
        {
            // don't care
            Trace.WriteLine(exception.Message);
        }
    }
}
