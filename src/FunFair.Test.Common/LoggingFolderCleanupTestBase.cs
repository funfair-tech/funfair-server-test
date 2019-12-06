using System;
using System.IO;
using Xunit.Abstractions;

namespace FunFair.Test.Common
{
    /// <summary>
    /// Base class that automatically cleans up temp folders.
    /// </summary>
    public abstract class LoggingFolderCleanupTestBase : LoggingTestBase
    {
        /// <summary>
        /// Constructor,s
        /// </summary>
        /// <param name="output"></param>
        protected LoggingFolderCleanupTestBase(ITestOutputHelper output)
            : base(output)
        {
            this.TempFolder = Path.Combine(Path.GetTempPath(),
                                              Guid.NewGuid()
                                                  .ToString());

            Directory.CreateDirectory(this.TempFolder);
        }

        /// <summary>
        /// The temporary folder that was created.
        /// </summary>
        protected string TempFolder { get; }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            MurderTempFolder(this.TempFolder);
            base.Dispose(disposing);
        }

        private static void MurderTempFolder(string folderToMurder)
        {
            try
            {
                Directory.Delete(folderToMurder, recursive: true);
            }
            catch
            {
                // don't care
            }
        }
    }
}