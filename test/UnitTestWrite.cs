using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    [TestClass]
    public class UnitTestWrite
    {
        const string sampleFile = "sample1.zip";
        private static byte[] buffer;

        [ClassInitialize]
        public static void Initialize(TestContext test)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
            buffer = Encoding.UTF8.GetBytes(content);
        }

        [TestMethod]
        public void CreateFile_Test()
        {
            File.Delete(sampleFile);
            using (ZipStorer zip = ZipStorer.Create(sampleFile))
            {
            }
        }

        [TestMethod]
        public void AddStream_Test()
        {
            this.createSampleFile();

            var now1 = DateTime.Now;
            using (ZipStorer zip = ZipStorer.Open(sampleFile, FileAccess.Read))
            {
                var dir = zip.ReadCentralDir();
                Assert.IsFalse(dir.Count == 0);
                Assert.IsTrue(dir[0].FilenameInZip == "Lorem.txt");
            }            
        }

        [TestMethod]
        public void AddStreamDate_Test()
        {
            var now = DateTime.Now;

            this.createSampleFile();

            using (ZipStorer zip = ZipStorer.Open(sampleFile, FileAccess.Read))
            {
                var dir = zip.ReadCentralDir();
                Assert.IsFalse(dir.Count == 0);
                Assert.IsTrue(dir[0].CreationTime >= now, "Creation Time failed");
                Assert.IsTrue(dir[0].ModifyTime >= now, "Modify Time failed");
                Assert.IsTrue(dir[0].AccessTime >= now, "Access Time failed");
            }            
        }

        [TestMethod]
        public void Compression_Test()
        {
            var now = DateTime.Now;

            this.createSampleFile();

            using (ZipStorer zip = ZipStorer.Open(sampleFile, FileAccess.Read))
            {
                var dir = zip.ReadCentralDir();
                Assert.IsFalse(dir.Count == 0);
                Assert.IsTrue(dir[0].Method == ZipStorer.Compression.Deflate);
                Assert.IsTrue(dir[0].CompressedSize < buffer.Length);
            }            
        }

        public void createSampleFile()
        {
            using (var mem = new MemoryStream(buffer))
            {
                File.Delete(sampleFile);
                using (ZipStorer zip = ZipStorer.Create(sampleFile))
                {
                    zip.AddStream(ZipStorer.Compression.Deflate, "Lorem.txt", mem, DateTime.Now);
                }
            }
        }
    }
}
