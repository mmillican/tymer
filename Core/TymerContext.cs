using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace tymer.Core
{
    public class TymerContext
    {
        public List<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

        public string DataFileLocation
        {
            get
            {
                if (TymerConfig.StoreInDocs)
                {
                    var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    return Path.Combine(docsPath, "Tymer", TymerConfig.DataFile);
                }

                return TymerConfig.DataFile;
            }
        }

        public TymerContext(bool loadDataFile = true)
        {
            if (loadDataFile)
            {
                LoadEntries();
            }
        }

        public void LoadEntries()
        {
            if (!File.Exists(DataFileLocation))
            {
                TimeEntries = new List<TimeEntry>();
                return;
            }

            try
            {
                using(var sr = new StreamReader(DataFileLocation))
                {
                    var fileContents = sr.ReadToEnd();

                    var data = JsonSerializer.Deserialize<TymerData>(fileContents,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

                    this.TimeEntries = data.TimeEntries;
                }
            }
            catch(Exception)
            {
                Console.WriteLine("Data file does not exist or could not be read.");
            }
        }

        public void SaveEntries()
        {
            EnsureFolderExists();

            var data = new TymerData
            {
                TimeEntries = TimeEntries
            };

            var serialized = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            using(var sw = new StreamWriter(DataFileLocation))
            {
                sw.Write(serialized);
            }
        }

        private void EnsureFolderExists()
        {
            if (TymerConfig.StoreInDocs)
            {
                var dirPath = DataFileLocation.Replace(TymerConfig.DataFile, "");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
            }
        }

        public class TymerData
        {
            public List<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
        }
    }
}
