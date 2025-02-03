using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace TransactionsDemo;

internal class WithSqlClient
{
    public static async Task Run()
    {
        await using SqlConnection connection = new SqlConnection("");
        await connection.OpenAsync();
        
        await using SqlTransaction transaction = connection.BeginTransaction();
        SqlCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        
        try
        {
            command.CommandText = "INSERT INTO dbo.Products (Name, Price) VALUES ('Product1', 100)";
            await command.ExecuteNonQueryAsync();
            
            //savepoint
            transaction.Save("name");


            command.CommandText = "INSERT INTO dbo.Products (Name, Price) VALUES ('Product2', 200)";
            await command.ExecuteNonQueryAsync();
            
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
        }
    }
}