using Npgsql;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ConsoleTables;

Console.OutputEncoding = Encoding.UTF8;

// Використовуючи SqlCommand і SqlDataReader, вивести на консоль усі замовлення (Orders) за останній рік.
string query = @"SELECT ord_id AS ID, ord_datetime AS TIMESTAMP, ord_an AS ANALYSIS 
    FROM orders WHERE EXTRACT(YEAR FROM ord_datetime) >= 2021 ORDER BY ord_datetime";
//await Tasks.FirstTaskAsync(query);

// Виконати умову завдання 1, але з використанням SqlDataAdapter і DataSet.
//Tasks.SecondTask(query);

// Виконати умову завдання 1, але з використанням EF Core.
//await Tasks.ThirdTaskAsync();

// Використовуючи SqlCommand, створити новий запис у таблиці Orders.
//await Tasks.FourthTaskAsync();

// Використовуючи SqlCommand, оновити один із записів у таблиці Orders.
await Tasks.FifthTaskAsync();