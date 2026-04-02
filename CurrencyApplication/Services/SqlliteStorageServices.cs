using CurrencyApplication.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CurrencyApplication.Services
{
    public class SqliteStorageService
    {
        private readonly string _dbPath;

        public SqliteStorageService()
        {
            var dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (!Directory.Exists(dataDir))
            {
                Directory.CreateDirectory(dataDir);
            }

            _dbPath = Path.Combine(dataDir, "currencies.db");

            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
            CREATE TABLE IF NOT EXISTS Currencies (
                Id TEXT PRIMARY KEY,
                CharCode TEXT,
                Name TEXT,
                Nominal INTEGER,
                Value REAL,
                Previous REAL,
                IsUserAdded INTEGER
            );
            ";
            command.ExecuteNonQuery();
        }

        public async Task SaveAsync(List<Currency> currencies)
        {
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM Currencies;";
            await deleteCmd.ExecuteNonQueryAsync();

            foreach (var c in currencies)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText =
                @"INSERT INTO Currencies 
                (Id, CharCode, Name, Nominal, Value, Previous, IsUserAdded)
                VALUES ($id, $char, $name, $nom, $val, $prev, $user);";

                cmd.Parameters.AddWithValue("$id", c.Id);
                cmd.Parameters.AddWithValue("$char", c.CharCode);
                cmd.Parameters.AddWithValue("$name", c.Name);
                cmd.Parameters.AddWithValue("$nom", c.Nominal);
                cmd.Parameters.AddWithValue("$val", c.Value);
                cmd.Parameters.AddWithValue("$prev", c.Previous);
                cmd.Parameters.AddWithValue("$user", c.IsUserAdded ? 1 : 0);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<List<Currency>> LoadAsync()
        {
            var result = new List<Currency>();

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Currencies;";

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new Currency
                {
                    Id = reader.GetString(0),
                    CharCode = reader.GetString(1),
                    Name = reader.GetString(2),
                    Nominal = reader.GetInt32(3),
                    Value = reader.GetDecimal(4),
                    Previous = reader.GetDecimal(5),
                    IsUserAdded = reader.GetInt32(6) == 1
                });
            }

            return result;
        }
    }
}