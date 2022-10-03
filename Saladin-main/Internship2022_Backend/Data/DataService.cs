using Data.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    public class DataService
    {
        private readonly ILogger<DataService> _logger;
        private readonly CacheService _cacheService;
        private readonly string _connectionString = "Server=ts-internship.database.windows.net;Database=Saladin;User Id=sa_admin;Password=schimba-MA";

        public DataService(ILogger<DataService> logger, CacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<List<Knight>> GetKights(string queryString, CancellationToken cancellationToken)
        {
            _cacheService.Read<List<Knight>>(queryString, out List<Knight> knights);

            if (knights != null)
            {
                return knights;
            }

            using SqlConnection connection = new(_connectionString);
            SqlCommand command = new(queryString, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

                knights = new();
                while (await reader.ReadAsync(cancellationToken))
                {
                    
                        Knight knight = new()
                        {
                            KnightId = Int32.Parse(reader["KnightId"].ToString()),
                            Name = reader["Name"].ToString(),
                            DictionaryKnightTypeName = reader["DictionaryKnightTypeName"].ToString(),
                            LegionName = reader["LegionName"].ToString(),
                            BattleName = reader["BattleName"] != System.DBNull.Value ? reader["BattleName"].ToString() : null,
                            CoinsAwardedPerBattle = reader["CoinsAwardedPerBattle"] != System.DBNull.Value ? Int32.Parse(reader["CoinsAwardedPerBattle"].ToString()) : 0
                        };

                        Console.WriteLine($"Adding knight to list {JsonConvert.SerializeObject(knight)}");
                        knights.Add(knight);
                    
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message, ex);
            }

            return _cacheService.Write(queryString, knights);
        }

        public async Task ChangeCoinsAwardedPerBattle(string commandString, CancellationToken cancellationToken)
        {
            using SqlConnection connection = new(_connectionString);
            SqlCommand command = new(commandString, connection);

            List<Knight> knights = new();

            try
            {
                connection.Open();
                int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message, ex);
            };
        }
    }
}
