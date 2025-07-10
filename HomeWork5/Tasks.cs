
using System.Data;
using System.Threading.Tasks;
using ConsoleTables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Npgsql;

public static class Tasks
{
    private static string connectionString = "Host=localhost;Username=postgres;Password=;Database=bloodwork";
    public static async Task FirstTaskAsync(string query)
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
    public static void SecondTask(string query)
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

    public static async Task FourthTaskAsync()
    {
        Console.WriteLine("\nВикористовуючи SqlCommand, створити новий запис у таблиці Orders:\nОберіть аналіз для замовлення:");
        int num = await GetAnalysesAsync();
        string? analysis;
        int anId;
        while (true)
        {
            analysis = Console.ReadLine();
            if (analysis.IsNullOrEmpty() || !int.TryParse(analysis, out int an_id) || an_id <= 0 || an_id > num)
            {
                Console.WriteLine("Неправильний id, спробуйте ще раз.");
                continue;
            }
            anId = int.Parse(analysis);
            break;
        }

        string insertQuery = @"INSERT INTO orders(ord_an) VALUES(@param1) RETURNING ord_id;";
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var command = new NpgsqlCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@param1", anId);
            var res = command.ExecuteScalar();
            if (res == null)
            {
                Console.WriteLine("Виникла помилка при замовленні.");
                return;
            }
            int newId = (int)res;
            await GetAnalysisByID(newId);
        }

    }

    public static async Task FifthTaskAsync()
    {
        Console.WriteLine("\nВикористовуючи SqlCommand, оновити один із записів у таблиці Orders.:\nОберіть замовлення для редагування:");
        int num = await GetOrdersAsync();
        string? orders;
        int ordId;
        while (true)
        {
            orders = Console.ReadLine();
            if (orders.IsNullOrEmpty() || !int.TryParse(orders, out int an_id))
            {
                Console.WriteLine("Неправильний id, спробуйте ще раз.");
                continue;
            }
            ordId = int.Parse(orders);
            break;
        }

        string updateQuery = @"UPDATE orders SET ord_an = @param1 WHERE ord_id = @param2 RETURNING ord_id;";
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            var command = new NpgsqlCommand(updateQuery, connection);
            command.Parameters.AddWithValue("@param1", 3);
            command.Parameters.AddWithValue("@param2", ordId);
            var res = command.ExecuteScalar();
            if (res == null)
            {
                Console.WriteLine("Виникла помилка при замовленні.");
                return;
            }
            int newId = (int)res;
            await GetAnalysisByID(newId);
        }

    }
    private static async Task GetAnalysisByID(int id)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            string query = @"SELECT o.ord_id AS ID, o.ord_datetime AS DATE, a.an_name AS NAME
                FROM orders o LEFT JOIN analysis a ON o.ord_an = a.an_id WHERE o.ord_id = @param1";

            var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("@param1", id);

            await using (var reader = await command.ExecuteReaderAsync())
            {
                await PrintUtil.PrintAsync(reader);
            }
        }
    }

    private static async Task<int> GetOrdersAsync()
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            string query = @"SELECT o.ord_id AS ID, o.ord_datetime AS DATE, a.an_name AS NAME 
                FROM orders o LEFT JOIN analysis a ON a.an_id = o.ord_an ";

            await using (var command = new NpgsqlCommand(query, connection))
            await using (var reader = await command.ExecuteReaderAsync())
            {
                return await PrintUtil.PrintAsync(reader);
            }
        }
    }

    private static async Task<int> GetAnalysesAsync()
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            string query = @"SELECT a.an_id AS ID, a.an_name AS NAME, a.an_price AS PRICE, g.gr_name AS GROUP 
                FROM analysis a LEFT JOIN groups g ON g.gr_id = a.an_group ";

            await using (var command = new NpgsqlCommand(query, connection))
            await using (var reader = await command.ExecuteReaderAsync())
            {
                return await PrintUtil.PrintAsync(reader);
            }
        }
    }
}