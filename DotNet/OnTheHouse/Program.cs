using System;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;

namespace OnTheHouse
{
    class Program
    {

        static void Main(string[] args)
        {
            DirectoryInfo baseDir = new DirectoryInfo(AppContext.BaseDirectory);

            properties = new ConcurrentQueue<Property>();

            Regex rgxPropertyFile = new Regex(@"^(\d+)\.json$", RegexOptions.IgnoreCase);

            Regex rgxRemoveJsonExtension = new Regex(@"\.json$", RegexOptions.IgnoreCase);

            List<FileInfo> postcodeFiles = new List<FileInfo>();

            var endFile = "end.json";

            if (File.Exists($@"{baseDir.FullName}/{endFile}"))
            {
                Console.WriteLine($"Download has Completed. Exit now. Please remove {endFile} to restart download.");
                return;
            }

            while (postcodeFiles.Count == 0)
            {
                Console.WriteLine($"{DateTime.Now}: Wait for postcode json files...");
                // try after 1 second
                Thread.Sleep(1000);
                postcodeFiles = Directory.GetFiles(baseDir.FullName)
                .Select(filename => new FileInfo(filename))
                .Where(fileInfo => rgxPropertyFile.IsMatch(fileInfo.Name))
                .ToList();
            }

            foreach (var file in baseDir.GetFiles("*.json"))
            {
                if (rgxPropertyFile.IsMatch(file.Name))
                {
                    // this is a property file
                    var json = File.ReadAllText(file.FullName);

                    var propertyList = JsonConvert.DeserializeObject<List<Property>>(json);

                    var match = rgxPropertyFile.Match(file.Name);

                    var postcode = match.Groups[1].Value;

                    // find the postcode folder and remove the existing ones

                    var foundDir = baseDir.GetDirectories().Where(di => di.Name == postcode);
                    if (foundDir.Any())
                    {
                        // remove the downloaded files
                        var postcodeFolder = foundDir.FirstOrDefault();

                        var propertyJsonFiles = postcodeFolder.GetFiles().Select(propertyJsonFile => rgxRemoveJsonExtension.Replace(propertyJsonFile.Name, "")).ToList();

                        propertyList.RemoveAll(p => propertyJsonFiles.Contains(p._key));

                    }
                    foreach (var p in propertyList)
                    {
                        properties.Enqueue(p);
                    }
                }
            }

            int total = 1;

            List<Spider> spiders = new List<Spider>();
            for (int i = 0; i < total; i++)
            {
                spiders.Add(new Spider());
            }

            Task.WaitAll(spiders.Select(s => s.Try()).ToArray());

            // write end.json to notify download finished
            File.WriteAllText($@"{baseDir.FullName}/{endFile}", JsonConvert.SerializeObject(DateTime.Now));

            // kill the controller
            KillController();
        }

        public static ConcurrentQueue<Property> properties = new ConcurrentQueue<Property>();

        static void KillController()
        {
            foreach (var othf in Process.GetProcessesByName("SpiderController"))
            {
                try
                {
                    othf.Kill();
                }
                catch (Exception ex) { }
            }
        }
    }
}
