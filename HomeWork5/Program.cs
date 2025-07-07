using Npgsql;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ConsoleTables;

Console.OutputEncoding = Encoding.UTF8;

// Використовуючи SqlCommand і SqlDataReader, вивести на консоль усі замовлення (Orders) за останній рік.
await Tasks.FirstTaskAsync();

// Виконати умову завдання 1, але з використанням SqlDataAdapter і DataSet.
Tasks.SecondTask();

// Виконати умову завдання 1, але з використанням EF Core.
await Tasks.ThirdTaskAsync();

