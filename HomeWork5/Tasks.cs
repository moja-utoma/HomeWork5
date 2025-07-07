
using System.Data;
using ConsoleTables;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public static class Tasks
{
    private static string query = @"SELECT ord_id AS ID, ord_datetime AS TIMESTAMP, ord_an AS ANALYSIS 
    FROM orders WHERE EXTRACT(YEAR FROM ord_datetime) >= 2021 ORDER BY ord_datetime";
    private static string connectionString = "Host=localhost;Username=postgres;Password=;Database=bloodwork";
    public static async Task FirstTaskAsync()
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            Console.WriteLine("Використовуючи SqlCommand і SqlDataReader, вивести на консоль усі замовлення (Orders) за останній рік:");


            await using (var command = new NpgsqlCommand(query, connection))
            await using (var reader = await command.ExecuteReaderAsync())
            {
                await PrintUtil.PrintAsync(reader);
            }
        }
        Console.ReadKey();
    }
    public static void SecondTask()
    {
        Console.WriteLine("\nВиконати умову завдання 1, але з використанням SqlDataAdapter і DataSet:");
        NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connectionString);
        DataSet dataset = new DataSet();
        adapter.Fill(dataset);
        PrintUtil.Print(dataset.Tables[0]);
        Console.ReadKey();
    }

    public static async Task ThirdTaskAsync()
    {
        Console.WriteLine("\nВиконати умову завдання 1, але з використанням EF Core:");
        using (var db = new AppDbContext())
        {
            var blogs = await db.Orders
                .Where(b => b.Datetime.Year >= 2021)
                .Select(b => new
                {
                    b.Id,
                    b.Datetime,
                    b.AnalysisId
                })
                .OrderBy(b => b.Datetime).ToListAsync();

            ConsoleTable
               .From(blogs)
               .Write(Format.Minimal);
        }
        Console.ReadKey();
    }
}