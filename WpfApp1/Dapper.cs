using System;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    public class Dapper
    {
        private readonly string pathToDatabase = @"Data Source = 03_Beispieldatenbank.db;";

        public void saveData(string sqlBefehl, Dictionary<string, object> parameter)
        {
            DynamicParameters dynparameter = convertToDynamic(parameter);

            using (IDbConnection dbConnection = new SQLiteConnection(pathToDatabase))
            {
                dbConnection.Execute(sqlBefehl, dynparameter);
            }
        }

        public List<T> readData<T>(string sqlBefehl, Dictionary<string, object> parameter)
        {
            DynamicParameters dynparameter = convertToDynamic(parameter);

            using (IDbConnection dbConnection = new SQLiteConnection(pathToDatabase))
            {
                List<T> rows = dbConnection.Query<T>(sqlBefehl, dynparameter).ToList();
                return rows;
            }
        }

        private DynamicParameters convertToDynamic(Dictionary<string, object> parameter)
        {
            DynamicParameters output = new DynamicParameters();

            foreach (var item in parameter)
            {
                output.Add(item.Key, item.Value);
            }

            return output;
        }
    }
}
