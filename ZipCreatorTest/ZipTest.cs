using System.IO.Compression;
using ZipCreator;

namespace ZipTest
{
    public class Tests
    {
        private string inputFile;
        private string inputFileContents;
        private string testFolder;
        private List<string> testFiles;
        private string outputZip;

        [SetUp]
        public void Setup()
        {
            testFolder = "test";
            testFiles = new List<string>()
            {
                "test.txt",
                "test.exe",
                "test.dll"
            };

            Directory.CreateDirectory(testFolder);

            foreach (var testFile in testFiles)
            {
                File.WriteAllText(Path.Combine(testFolder, testFile), testFile);
            }

            inputFile = "input.txt";
            inputFileContents = $"{testFolder}/*";
            outputZip = "output.zip";

            File.WriteAllText(inputFile, inputFileContents);
        }

        [Test]
        public void CreateFromFile_WithInvalidFile_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Zip.CreateFromFile(new FileInfo("invalid")));
        }

        [Test]
        public void CreateFromFile_WithValidFile_SetsValueOfFilters()
        {
            var zip = Zip.CreateFromFile(new FileInfo(inputFile));

            Assert.That(zip.Settings.Filters[0], Is.EqualTo(inputFileContents));
        }

        [Test]
        public void MakeZips_WithGlobInput_CreatesZipWithFilesMatchingGlob()
        {
            var extension = ".exe";
            var zip = new Zip();
            zip.Settings.Filters = new List<string>() { $"{testFolder}/*{extension}" };
            zip.Settings.Output = new FileInfo(outputZip);

            zip.Write();

            using (var archive = ZipFile.OpenRead(zip.Settings.Output.FullName))
            {
                var expected = testFiles.Where(file => file.EndsWith(extension)).ToList();
                var results = archive.Entries.Select(entry => entry.Name).ToList();
                CollectionAssert.AreEqual(expected, results);
            }
        }

        [Test]
        public void MakeZips_WithExcludeFilter_CreatesZipIgnoringExcludeFilters()
        {
            var extension = ".txt";
            var zip = new Zip();
            zip.Settings.Filters = new List<string>() {
                $"{testFolder}/*",
                $"!{testFolder}/*{extension}"
            };
            zip.Settings.Output = new FileInfo(outputZip);

            zip.Write();

            using (var archive = ZipFile.OpenRead(zip.Settings.Output.FullName))
            {
                var expected = testFiles.Where(file => !file.EndsWith(extension)).OrderBy(name => name);
                var results = archive.Entries.Select(entry => entry.Name).OrderBy(name => name);
                CollectionAssert.AreEqual(expected, results);
            }
        }

        [Test]
        public void MakeZips_WithInterceptor_CallsInterceptorWithEachFile()
        {
            var results = new List<string>();
            var zip = new Zip();
            zip.Settings.Filters = new List<string>() { $"{testFolder}/*" };
            zip.Settings.Interceptors = new List<Action<FileInfo>>() { (fileInfo) => results.Add(fileInfo.Name) };
            zip.Settings.Output = new FileInfo(outputZip);

            zip.Write();

            var expected = testFiles.OrderBy(name => name);
            var actual = results.OrderBy(name => name);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeZips_WithOverwriteFalse_ReturnsOverwriteError()
        {
            var zip = Zip.CreateFromFile(new FileInfo(inputFile));
            zip.Settings.Overwrite = false;
            zip.Settings.Output = new FileInfo(outputZip);
            File.WriteAllText(outputZip, "hello world!");

            var result = zip.Write();

            Assert.That(result, Is.EqualTo(ZipWriteResult.OverwriteError));
        }

        [Test]
        public void MakeZips_WithOverwriteTrue_OverwritesOutputFile()
        {
            var zip = Zip.CreateFromFile(new FileInfo(inputFile));
            zip.Settings.Overwrite = true;
            zip.Settings.Output = new FileInfo(outputZip);
            File.WriteAllText(outputZip, "hello world!");

            Assert.DoesNotThrow(() => zip.Write());
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(inputFile))
            {
                File.Delete(inputFile);
            }

            if (File.Exists(outputZip))
            {
                File.Delete(outputZip);
            }

            if (Directory.Exists(testFolder))
            {
                Directory.Delete(testFolder, true);
            }
        }
    }
}