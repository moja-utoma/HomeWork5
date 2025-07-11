using Npgsql;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ConsoleTables;

Console.OutputEncoding = Encoding.UTF8;

string query = @"SELECT ord_id AS ID, ord_datetime AS TIMESTAMP, ord_an AS ANALYSIS 
    FROM orders WHERE EXTRACT(YEAR FROM ord_datetime) >= 2021 ORDER BY ord_datetime";
string? input;
do
{
    Console.Clear();
    Console.WriteLine("=== МЕНЮ ===");
    Console.WriteLine("1. Виконати запит (SqlCommand)");
    Console.WriteLine("2. Виконати запит (SqlDataAdapter + DataSet)");
    Console.WriteLine("3. Виконати запит (EF Core)");
    Console.WriteLine("4. Створити нове замовлення");
    Console.WriteLine("5. Оновити замовлення");
    Console.WriteLine("6. Видалити замовлення");
    Console.WriteLine("7. 4-6 завдання (EF Core)");
    Console.WriteLine("0. Вихід");
    Console.Write("\nВаш вибір: ");
    input = Console.ReadLine();

    switch (input)
    {
        case "1":
            await Tasks.FirstTaskAsync(query);
            break;
        case "2":
            Tasks.SecondTask(query);
            break;
        case "3":
            await Tasks.ThirdTaskAsync();
            break;
        case "4":
            await Tasks.FourthTaskAsync();
            break;
        case "5":
            await Tasks.FifthTaskAsync();
            break;
        case "6":
            await Tasks.SixthTaskAsync();
            break;
        case "7":
            await Tasks.SeventhTaskAsync();
            break;
        case "0":
            Console.WriteLine("Вихід...");
            break;
        default:
            Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
            break;
    }

    if (input != "0")
    {
        Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
        Console.ReadKey();
    }

} while (input != "0");