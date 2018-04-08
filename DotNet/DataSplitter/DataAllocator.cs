using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using CsvHelper;
using HouseDataImport;
using System.IO;
using System.IO.Compression;
using Xunit;
using Newtonsoft.Json;

namespace DataSplitterTests
{

    public class DataAllocator
    {
        [Fact(DisplayName = "Data.CreatePackage")]
        public void CreatePackage()
        {
            var postcodes = new int[] { 4000, 4005, 4006, 4059, 4064, 4066, 4067, 4068, 4101, 4102, 4169, 4109, 4108, 4122, 4113, 4123, 4119, 4112, 4113, 4116, 4115, 4121 };

            var numberOfSpiders = 15;

            var postCodeStrings = postcodes.Select(code => code.ToString()).ToList();



            string csvAddress = DataUtil.PropertyLocationCSVPath();

            List<Property> properties = new List<Property>();

            using (TextReader text = new StreamReader(csvAddress))
            {
                using (var csv = new CsvReader(text))
                {
                    csv.Configuration.BadDataFound = null;

                    while (csv.Read())
                    {
                        var row = csv.GetRecord<dynamic>();

                        var dict = row as IDictionary<string, object>;

                        // var doc = string.Join("\n", dict.Keys.Select(key => $"prop. = dict[\"{key}\"];"));

                        var prop = new Property();

                        prop.UnitNumber = dict["UNIT NUMBER"] as string;
                        prop.HouseNumber = dict["HOUSE NUMBER"] as string;
                        prop.StreetName = dict["STREET NAME"] as string;
                        prop.StreetType = dict["STREET TYPE"] as string;
                        prop.StreetSuffix = dict["STREET SUFFIX"] as string;
                        prop.Suburb = dict["SUBURB"] as string;
                        prop.Postcode = dict["POSTCODE"] as string;
                        prop.AddressUseType = dict["ADDRESS USE TYPE"] as string;
                        prop.WardName = dict["WARD NAME"] as string;
                        prop.PropertyDescription = dict["PROPERTY DESCRIPTION"] as string;

                        prop.BuildKey();

                        if (postCodeStrings.Contains(prop.Postcode))
                            properties.Add(prop);

                    }
                }


            }

            int batchCount = (int) Math.Ceiling((double) properties.Count / (double) numberOfSpiders);


            // this is the correct method for the rest

            foreach(var list in properties.Split(numberOfSpiders)) {
                
            }
        }

        [Fact(DisplayName = "Generate Per Postcode List")]
        public void GetHousesPerPostcode()
        {
            var postcodes = new int[] { 4000, 4005, 4006, 4059, 4064, 4066, 4067, 4068, 4101, 4102, 4169, 4109, 4108, 4122, 4113, 4123, 4119, 4112, 4113, 4116, 4115, 4121 };

            foreach(var postcode in postcodes)
            {
                var list = ReadPostcode(postcode.ToString());

                var filename = $@"{CreatePostcodeDir()}\{postcode}.json";

                File.WriteAllText(filename, JsonConvert.SerializeObject(list, Formatting.Indented));
            }
        }

        public string CreatePostcodeDir()
        {
            var postcodeDir = $@"{AppContext.BaseDirectory}\postcode";
            if(!Directory.Exists(postcodeDir))
                Directory.CreateDirectory(postcodeDir);
            return postcodeDir;
        }

        public string CreatePostcodeUpdatedDir()
        {
            var postcodeDir = $@"{AppContext.BaseDirectory}\postcode\updated";
            if (!Directory.Exists(postcodeDir))
                Directory.CreateDirectory(postcodeDir);
            return postcodeDir;
        }

        public List<Property> ReadPostcode(string postcode)
        {
            string csvAddress = $@"{AppContext.BaseDirectory}\..\..\..\..\..\PropertyLocations\20171215000odaddress.csv";

            List<Property> properties = new List<Property>();


            using (TextReader text = new StreamReader(csvAddress))
            {
                using (var csv = new CsvReader(text))
                {
                    csv.Configuration.BadDataFound = null;

                    
                    while (csv.Read())
                    {
                        var row = csv.GetRecord<dynamic>();

                        var dict = row as IDictionary<string, object>;

                        // var doc = string.Join("\n", dict.Keys.Select(key => $"prop. = dict[\"{key}\"];"));

                        var prop = new Property();

                        prop.UnitNumber = dict["UNIT NUMBER"] as string;
                        prop.HouseNumber = dict["HOUSE NUMBER"] as string;
                        prop.StreetName = dict["STREET NAME"] as string;
                        prop.StreetType = dict["STREET TYPE"] as string;
                        prop.StreetSuffix = dict["STREET SUFFIX"] as string;
                        prop.Suburb = dict["SUBURB"] as string;
                        prop.Postcode = dict["POSTCODE"] as string;
                        prop.AddressUseType = dict["ADDRESS USE TYPE"] as string;
                        prop.WardName = dict["WARD NAME"] as string;
                        prop.PropertyDescription = dict["PROPERTY DESCRIPTION"] as string;

                        prop.BuildKey();

                        if(prop.Postcode == postcode)
                            properties.Add(prop);
                    }
                }
            }

            return properties;
        }

        [Fact(DisplayName = "Remove Downloaded From List")]
        public void RemoveDownloaded()
        {
            // remove downloaded from the list

            var postcodeDir = CreatePostcodeDir();

            var dataDir = new DirectoryInfo(@"D:\VSTS\Repos\Machine Learning Lecture\Spider\OnTheHouse\data");

            var updateDir = CreatePostcodeUpdatedDir();

            foreach(var filename in Directory.GetFiles(postcodeDir))
            {
                var fInfo = new FileInfo(filename);
                var postcode = fInfo.Name.Replace(".json", "");
                if (Directory.Exists($@"{dataDir.FullName}\{postcode}"))
                {
                    var downloaded = Directory.GetFiles($@"{dataDir.FullName}\{postcode}").Select(f => new FileInfo(f).Name).ToList();

                    // remove the entries from files 
                    List<Property> list = JsonConvert.DeserializeObject<List<Property>>(File.ReadAllText(fInfo.FullName));

                    list.RemoveAll(p => downloaded.Contains(p.BuildKey() + ".json"));

                    File.WriteAllText($@"{updateDir}\{fInfo.Name}", JsonConvert.SerializeObject(list));
                }
                else
                {
                    File.Copy(fInfo.FullName, $@"{updateDir}\{fInfo.Name}");
                }
            }

            foreach(var pcDir in dataDir.GetDirectories())
            {

            }

        }
    }
}