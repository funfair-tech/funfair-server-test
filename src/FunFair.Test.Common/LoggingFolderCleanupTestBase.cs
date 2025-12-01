using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Xunit;

namespace FunFair.Test.Common;

public abstract class LoggingFolderCleanupTestBase : LoggingTestBase
{
    protected LoggingFolderCleanupTestBase(ITestOutputHelper output)
        : base(output)
    {
        this.TempFolder = CreateTestUniqueTempDir(output);
    }

    protected string TempFolder { get; }

    private static string CreateTestUniqueTempDir(ITestOutputHelper output)
    {
        string? tempPath = GetTempPath();

        Assert.False(string.IsNullOrEmpty(tempPath), userMessage: "Temp Path is empty");
        string testTempPath = Path.Combine(path1: tempPath, Guid.NewGuid().ToString());

        DirectoryInfo created = Directory.CreateDirectory(testTempPath);

        output.WriteLine($"Using Temp folder: {created.FullName}");

        Assert.True(condition: created.Exists, $"Temp folder {testTempPath} doesn't exist");

        return created.FullName;
    }

    private static string? GetTempPath()
    {
        if (CheckPath(variable: "XDG_RUNTIME_DIR", out string? tempPath))
        {
            return tempPath;
        }

        if (CheckPath(variable: "TMP", path: out tempPath))
        {
            return tempPath;
        }

        if (CheckPath(variable: "TEMP", path: out tempPath))
        {
            return tempPath;
        }

        if (CheckPath(variable: "TMPDIR", path: out tempPath))
        {
            return tempPath;
        }

        return null;
    }

    private static bool CheckPath(string variable, [NotNullWhen(true)] out string? path)
    {
        string? p = Environment.GetEnvironmentVariable(variable);

        if (!string.IsNullOrEmpty(p))
        {
            path = p;

            return true;
        }

        path = null;

        return false;
    }

    protected override void Dispose(bool disposing)
    {
        MurderTempFolder(this.TempFolder);
        base.Dispose(disposing);
    }

    protected string CreateFolderInTempFolder(string name)
    {
        string fullPath = Path.Combine(path1: this.TempFolder, path2: name);
        this.EnsureDirectoryExists(fullPath);

        return fullPath;
    }

    protected void EnsureDirectoryExists(string fullPath)
    {
        if (Directory.Exists(fullPath))
        {
            this.Output.WriteLine($"Directory {fullPath} already exists");

            return;
        }

        try
        {
            DirectoryInfo di = Directory.CreateDirectory(fullPath);
            this.Output.WriteLine($"Directory {fullPath} created as {di.FullName} on {di.CreationTimeUtc}");
        }
        catch (Exception exception)
        {
            this.Output.WriteLine($"Directory {fullPath} could not be created: {exception.Message}");

            throw;
        }
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
