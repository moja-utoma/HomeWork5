
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
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        Console.WriteLine("Використовуючи SqlCommand і SqlDataReader, вивести на консоль усі замовлення (Orders) за останній рік:");

        var command = new NpgsqlCommand(query, connection);
        await using (var reader = await command.ExecuteReaderAsync())
        {
            await PrintUtil.PrintAsync(reader);
        }

    }
    public static void SecondTask(string query)
    {
        Console.WriteLine("\nВиконати умову завдання 1, але з використанням SqlDataAdapter і DataSet:");
        NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, connectionString);
        DataSet dataset = new DataSet();
        adapter.Fill(dataset);
        PrintUtil.Print(dataset.Tables[0]);
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
    }

    public static async Task FourthTaskAsync()
    {
        Console.WriteLine("\nВикористовуючи SqlCommand, створити новий запис у таблиці Orders:");
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var getIdsCommand = new NpgsqlCommand("SELECT an_id FROM analysis", connection);
        var analysisIds = new List<int>();

        await using (var reader = await getIdsCommand.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
                analysisIds.Add(reader.GetInt32(0));
        }

        if (analysisIds.Count == 0)
        {
            Console.WriteLine("Немає доступних аналізів.");
            return;
        }

        int selectedAnId;
        try
        {
            selectedAnId = await GetRandomAnalysisIdAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
            return;
        }

        Console.WriteLine($"Обрано аналіз з ID = {selectedAnId}");

        var insertCommand = new NpgsqlCommand(
            "INSERT INTO orders (ord_an) VALUES (@an_id) RETURNING ord_id", connection);
        insertCommand.Parameters.AddWithValue("@an_id", selectedAnId);

        object? result = await insertCommand.ExecuteScalarAsync();

        if (result is not null && result is int newOrderId)
        {
            Console.WriteLine($"Створено замовлення з ID = {newOrderId}");
            await GetAnalysisByID(newOrderId);
        }
        else
        {
            Console.WriteLine("Помилка при створенні замовлення.");
        }
    }

    public static async Task FifthTaskAsync()
    {
        Console.WriteLine("\nВикористовуючи SqlCommand, оновити один із записів у таблиці Orders.");

        int randomOrderId = await GetRandomOrderIdAsync();

        Console.WriteLine("\nПоточне замовлення:");
        var oldOrder = await GetAnalysisByID(randomOrderId);

        if (oldOrder == null)
        {
            Console.WriteLine("Замовлення не знайдено.");
            return;
        }

        int oldAnalysisId = Convert.ToInt32(oldOrder["an_id"]);

        int newAnalysisId;
        do
        {
            newAnalysisId = await GetRandomAnalysisIdAsync();
        } while (newAnalysisId == oldAnalysisId);

        Console.WriteLine($"\nНовий аналіз (an_id): {newAnalysisId}");

        string updateQuery = @"UPDATE orders SET ord_an = @param1 WHERE ord_id = @param2 RETURNING ord_id;";

        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var command = new NpgsqlCommand(updateQuery, connection);
        command.Parameters.AddWithValue("@param1", newAnalysisId);
        command.Parameters.AddWithValue("@param2", randomOrderId);

        var res = await command.ExecuteScalarAsync();

        if (res == null)
        {
            Console.WriteLine("Виникла помилка при оновленні.");
            return;
        }

        Console.WriteLine("\nОновлене замовлення:");
        await GetAnalysisByID((int)res);
    }

    public static async Task SixthTaskAsync()
    {
        Console.WriteLine("\nВикористовуючи SqlCommand, оновити один із записів у таблиці Orders.");

        int randomOrderId = await GetRandomOrderIdAsync();
        Console.WriteLine("\nПоточне замовлення:");
        var oldOrder = await GetAnalysisByID(randomOrderId);

        if (oldOrder == null)
        {
            Console.WriteLine("Замовлення не знайдено.");
            return;
        }

        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        string deletequery = "DELETE FROM orders WHERE ord_id = @id";
        var command = new NpgsqlCommand(deletequery, connection);
        command.Parameters.AddWithValue("@id", randomOrderId);
        int affectedRows = await command.ExecuteNonQueryAsync();
        if (affectedRows > 0)
        {
            Console.WriteLine($"Замовлення з ID = {randomOrderId} успішно видалено.");
        }
        else
        {
            Console.WriteLine($"Замовлення з ID = {randomOrderId} не знайдено або не видалено.");
        }

    }

    public static async Task SeventhTaskAsync()
    {
        using (var db = new AppDbContext())
        {
            // додавання
            var analysis = await GetRandomAnalysisAsync(db);
            if (analysis == null)
            {
                Console.WriteLine("Немає доступних аналізів для замовлення.");
                return;
            }

            var order = new Order
            {
                Analysis = analysis
            };

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            Console.WriteLine($"Створено нове замовлення:");
            PrintUtil.PrintOrder(order);

            Console.WriteLine(new string('-', 20));

            // редагування
            order = await GetRandomOrderAsync(db);
            if (order == null)
            {
                Console.WriteLine("Немає замовлень для оновлення.");
                return;
            }

            Console.WriteLine($"Замовлення {order.Id}:");
            PrintUtil.PrintOrder(order);

            Analysis? newAnalysis;

            do
            {
                newAnalysis = await GetRandomAnalysisAsync(db);
            } while (newAnalysis is not null && newAnalysis.Id == order.AnalysisId);

            if (newAnalysis == null)
            {
                Console.WriteLine("Не вдалося отримати новий аналіз.");
                return;
            }

            order.Analysis = newAnalysis;
            await db.SaveChangesAsync();

            Console.WriteLine($"\nЗамовлення {order.Id} оновлено:");
            PrintUtil.PrintOrder(order);

            Console.WriteLine(new string('-', 20));

            // видалення
            order = await GetRandomOrderAsync(db);
            if (order == null)
            {
                Console.WriteLine("Немає замовлень для видалення.");
                return;
            }

            db.Orders.Remove(order);
            await db.SaveChangesAsync();

            Console.WriteLine($"Замовлення з ID {order} видалено.");
        }

    }

    private static async Task<Analysis?> GetRandomAnalysisAsync(AppDbContext db)
    {
        return await db.Analyses
            .OrderBy(a => Guid.NewGuid())
            .FirstOrDefaultAsync();
    }

    private static async Task<Order?> GetRandomOrderAsync(AppDbContext db)
    {
        return await db.Orders.Include(o => o.Analysis)
            .OrderBy(o => Guid.NewGuid())
            .FirstOrDefaultAsync();
    }

    private static async Task<int> GetRandomOrderIdAsync()
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var cmd = new NpgsqlCommand("SELECT ord_id FROM orders", connection);
        var orderIds = new List<int>();

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            orderIds.Add(reader.GetInt32(0));

        if (orderIds.Count == 0)
            throw new Exception("У таблиці orders немає записів.");

        var rand = new Random();
        return orderIds[rand.Next(orderIds.Count)];
    }

    private static async Task<int> GetRandomAnalysisIdAsync()
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var cmd = new NpgsqlCommand("SELECT an_id FROM analysis", connection);
        var analysisIds = new List<int>();

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            analysisIds.Add(reader.GetInt32(0));

        if (analysisIds.Count == 0)
            throw new Exception("У таблиці analysis немає записів.");

        var rand = new Random();
        return analysisIds[rand.Next(analysisIds.Count)];
    }

    private static async Task<Dictionary<string, object>?> GetAnalysisByID(int id)
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        string query = @"SELECT o.ord_id AS ID, o.ord_datetime AS DATE, a.an_name AS NAME, o.ord_an AS AN_ID
                 FROM orders o
                 LEFT JOIN analysis a ON o.ord_an = a.an_id
                 WHERE o.ord_id = @param1";

        var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@param1", id);

        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var result = new Dictionary<string, object>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                result[reader.GetName(i)] = reader.GetValue(i);
            }

            foreach (var kvp in result)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
            }

            return result;
        }

        Console.WriteLine("Запис не знайдено.");
        return null;
    }
}