using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SAA_MDDB;

class DBManager
{
    private const string Tables = "Tables"; 

    private MyList<string> _tableNames = new MyList<string>();

    public DBManager()
    {
        if (!File.Exists(Tables))
        {
            var file = new FileStream(Tables, FileMode.Create);
            var bw = new BinaryWriter(file);
            bw.Write(0);
            bw.Close();
            file.Dispose();
        }
        else
        {
            using (var tf = new FileStream(Tables, FileMode.Open))
            {
                var br = new BinaryReader(tf);
                int count = br.ReadInt32();

                for (int i = 0; i < count; i++)
                {
                    _tableNames.Add(br.ReadString());
                }
            }
        }
    }

    public void CreateTable(string name, MyList<Column> cols)
    {
        if (File.Exists(name))
        {
            Console.WriteLine("The name of the table already exists.");
            return;
        }

        using (var metaFile = new FileStream($"Meta_{name}", FileMode.Create))
        {

            var bw = new BinaryWriter(metaFile);
            bw.Write(cols.Count);

            foreach (var col in cols)
            {
                bw.Write(col.Name);
                bw.Write((byte)col.Type);
                bw.Write(col.DefaultValue);
                bw.Write(col.IsAutoIncrement);
            }
        }

        using (var file = new FileStream(name,FileMode.Create)) 
        {
            var bw = new BinaryWriter(file);
            bw.Write(0);
        }

        _tableNames.Add(name);
        using(var tableFile = new FileStream(Tables,FileMode.Create))
        {
            var bw = new BinaryWriter(tableFile);
            bw.Write(_tableNames.Count);
            for (int i = 0; i < _tableNames.Count; i++)
            {
                bw.Write(_tableNames[i]);
            }
        }
    }

    public void DropTable(string name)
    {
        File.Delete($"Meta_{name}");
        File.Delete(name);

        _tableNames.Remove(name);
        using (var tableFile = new FileStream(Tables, FileMode.Create))
        {
            var bw = new BinaryWriter(tableFile);
            bw.Write(_tableNames.Count);
            for (int i = 0; i < _tableNames.Count; i++)
            {
                bw.Write(_tableNames[i]);
            }
        }
    }

    public void ListTables()
    {
        for (int i = 0; i < _tableNames.Count; i++)
        {
            Console.WriteLine($"Table {i}: {_tableNames[i]}");
        }
    }

    public void TableInfo(string name)
    {
        int fileSize = 0;

        var table = new DataFileStreamArray(name);
        Console.WriteLine(name);
        Console.WriteLine(new string('-',4*21));
        Console.Write(StringHelper.AddPadding(21,"|Field"));
        Console.Write(StringHelper.AddPadding(20,"Type"));
        Console.Write(StringHelper.AddPadding(20,"Default"));
        Console.Write(StringHelper.AddPadding(20,"Auto Incrament"));
        Console.WriteLine();
        Console.WriteLine(new string('-', 4 * 21));

        for (int i = 0; i < table._metaData.Count; i++)
        {
            Console.WriteLine($"{table._metaData[i].ToString()}");
        }
        Console.WriteLine(new string('-', 4 * 21));

        fileSize = table._rowCount * table.DataSize;

        Console.WriteLine($"There are {table._rowCount} entries in the table.");
        Console.WriteLine($"The occupied space: {fileSize/1024} KB");

        table.Dispose();
    }

    public void Insert(string name, MyList<string> data)
    {
        var row = new DataFileStreamArray(name);

        foreach (var item in data)
        {
            row[row._rowCount] = item;
        }

        row.Dispose();
    }

    private ExpressionResult Compare(MyList<Cell> row, string expression)
    {
        var exp = StringHelper.MySplit(expression, ' ');
        Cell? cell = null;
        var left = exp[0];
        var opr = exp[1];
        var right = exp[2];

        for (int i = 0; i < row.Count; i++) 
        {
            if (left == row[i].ColName)
            {
                cell = row[i];
                break;
            }
        }

        if (cell == null)
            return new ExpressionResult(false, false, "Not valid colnm name.");

        int result = -2;
        switch(cell.Type)
        {
            case MDDBType.Int:
                 result = int.Parse(cell.Value).CompareTo(int.Parse(right));
                break;
            case MDDBType.String:
                result = StringHelper.CompareStrings(cell.Value, right); 
                break;
            case MDDBType.Date:
                result = DateTime.Compare(DateTime.Parse(cell.Value), DateTime.Parse(right));
                break;
        }

        if (result == -2)
            return new ExpressionResult(false, false, "Invalid type.");

        switch (opr)
        {
            case "<>": return new ExpressionResult(result != 0); 
            case "=": return new ExpressionResult(result == 0);
            case ">": return new ExpressionResult(result == 1);
            case "<": return new ExpressionResult(result == -1);
            case "<=": return new ExpressionResult(result == -1 || result == 0);
            case ">=": return new ExpressionResult(result == 1 || result == 0);
            default: return new ExpressionResult(false, false, "Invalid operator.");
        }
    }
}

