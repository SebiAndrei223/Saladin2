using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Data
{
    public class CacheService
    {
        private readonly ILogger<CacheService> _logger;
        private readonly string _filePath = @"C:\\Cache\";
        private readonly string _fileName = @"Cache.txt";

        public CacheService(ILogger<CacheService> logger)
        {
            _logger = logger;
        }

        public bool Read<T>(string queryString, out T values)
        {
            try
            {
                if (!File.Exists(@$"{_filePath}{_fileName}"))
                {
                    values = default;
                    return false;
                }

                string fileText = File.ReadAllText(@$"{_filePath}{_fileName}");

                if (string.IsNullOrWhiteSpace(fileText))
                {
                    values = default;
                    return false;
                }

                Dictionary<string, string> file = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileText);

                if (file.ContainsKey(queryString))
                {
                    values = JsonConvert.DeserializeObject<T>(file[queryString]);
                    return true;
                }

                values = default;
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }

        public T Write<T>(string queryString, T values)
        {
            try
            {
                if (!File.Exists(@$"{_filePath}{_fileName}"))
                {
                    Directory.CreateDirectory(_filePath);
                    using FileStream fileStream = File.Create(@$"{_filePath}{_fileName}");
                    fileStream.Close();
                }

                string fileText = File.ReadAllText(@$"{_filePath}{_fileName}");

                Dictionary<string, string> file = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileText) ?? new();

                if (!file.ContainsKey(queryString))
                {
                    file.Add(queryString, JsonConvert.SerializeObject(values));
                    File.WriteAllText(@$"{_filePath}{_fileName}", JsonConvert.SerializeObject(file));
                }

                return values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
