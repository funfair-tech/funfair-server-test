using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Xunit.Abstractions;

namespace FunFair.Test.Common
{
    /// <summary>
    ///     Base class that automatically cleans up temp folders.
    /// </summary>    [SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "Base class for further tests")]
    public abstract class LoggingFolderCleanupTestBase : LoggingTestBase
    {
        /// <summary>
        ///     Constructor,s
        /// </summary>
        /// <param name="output"></param>
        protected LoggingFolderCleanupTestBase(ITestOutputHelper output)
            : base(output)
        {
            this.TempFolder = Path.Combine(Path.GetTempPath(),
                                           Guid.NewGuid()
                                               .ToString());

            Directory.CreateDirectory(this.TempFolder);
            this.Output.WriteLine($"Using Temp folder: {this.TempFolder}");
        }

        /// <summary>
        ///     The temporary folder that was created.
        /// </summary>
        protected string TempFolder { get; }

        /// <inheritdoc />
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
            catch
            {
                // don't care
            }
        }
    }
}
