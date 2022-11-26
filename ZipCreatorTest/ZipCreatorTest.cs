using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ZipC;

namespace ZipCreatorTest
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
            Assert.Throws<ArgumentException>(() => ZipCreator.CreateFromFile(new FileInfo("invalid")));
        }

        [Test]
        public void CreateFromFile_WithValidFile_SetsValueOfFilters()
        {
            var zipCreator = ZipCreator.CreateFromFile(new FileInfo(inputFile));

            Assert.That(zipCreator.Settings.Filters[0], Is.EqualTo(inputFileContents));
        }

        [Test]
        public void MakeZips_WithGlobInput_CreatesZipWithFilesMatchingGlob()
        {
            var extension = ".exe";
            var zipCreator = new ZipCreator();
            zipCreator.Settings.Filters = new List<string>() { $"{testFolder}/*{extension}" };
            zipCreator.Settings.Output = new FileInfo(outputZip);

            zipCreator.MakeZips();

            using (var archive = ZipFile.OpenRead(zipCreator.Settings.Output.FullName))
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
            var zipCreator = new ZipCreator();
            zipCreator.Settings.Filters = new List<string>() {
                $"{testFolder}/*",
                $"!{testFolder}/*{extension}"
            };
            zipCreator.Settings.Output = new FileInfo(outputZip);

            zipCreator.MakeZips();

            using (var archive = ZipFile.OpenRead(zipCreator.Settings.Output.FullName))
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
            var zipCreator = new ZipCreator();
            zipCreator.Settings.Filters = new List<string>() { $"{testFolder}/*" };
            zipCreator.Settings.Interceptors = new List<Action<FileInfo>>() { (fileInfo) => results.Add(fileInfo.Name) };
            zipCreator.Settings.Output = new FileInfo(outputZip);

            zipCreator.MakeZips();

            var expected = testFiles.OrderBy(name => name);
            var actual = results.OrderBy(name => name);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void MakeZips_WithOverwriteFalse_ReturnsOverwriteError()
        {
            var zipCreator = ZipCreator.CreateFromFile(new FileInfo(inputFile));
            zipCreator.Settings.Overwrite = false;
            zipCreator.Settings.Output = new FileInfo(outputZip);
            File.WriteAllText(outputZip, "hello world!");

            var result = zipCreator.MakeZips();

            Assert.That(result, Is.EqualTo(ZipCreatorResult.OverwriteError));
        }

        [Test]
        public void MakeZips_WithOverwriteTrue_OverwritesOutputFile()
        {
            var zipCreator = ZipCreator.CreateFromFile(new FileInfo(inputFile));
            zipCreator.Settings.Overwrite = true;
            zipCreator.Settings.Output = new FileInfo(outputZip);
            File.WriteAllText(outputZip, "hello world!");

            Assert.DoesNotThrow(() => zipCreator.MakeZips());
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