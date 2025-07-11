using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Npgsql;
using ConsoleTables;
using System.Reflection;

public static class PrintUtil
{
    public static async Task<int> PrintAsync(NpgsqlDataReader reader)
    {
        var table = new ConsoleTable();
        int rowCount = 0;

        // Add columns
        for (int i = 0; i < reader.FieldCount; i++)
            table.Columns.Add(reader.GetName(i));

        // Add rows
        while (await reader.ReadAsync())
        {
            var row = new object[reader.FieldCount];
            reader.GetValues(row);
            table.AddRow(row);
            rowCount++;
        }

        table.Write(Format.Minimal); // or Format.Default

        return rowCount;
    }

    public static void Print(DataTable dataTable)
    {
        var table = new ConsoleTable();

        // Add columns
        foreach (DataColumn col in dataTable.Columns)
            table.Columns.Add(col.ColumnName);

        // Add rows
        foreach (DataRow dr in dataTable.Rows)
            table.AddRow(dr.ItemArray);

        table.Write(Format.Minimal); // or Format.Default
    }

    public static void PrintOrder(Order order)
    {
        Console.WriteLine($"ID: {order.Id}");
        Console.WriteLine($"Datetime: {order.Datetime}");
        Console.WriteLine($"Analysis ID: {order.AnalysisId}");
        Console.WriteLine($"Analysis Name: {order.Analysis?.Name}");
    }
}
