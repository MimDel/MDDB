using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

    public void Select(string name, MyList<string> colNames, string? whereClause)
    {
        if (!_tableNames.Contains(name))
        {
            Console.WriteLine("The name of the table is not valid.");
            return;
        }

        var table = new DataFileStreamArray(name);
        MyList<string> res = new MyList<string>();
        for (int i = 0; i < table._rowCount; i++)
        {
            var cells = new MyList<Cell>();
            var cols = StringHelper.MySplit(table[i],'\0');
            for (int j = 0; j < cols.Length; j++)
            {
                var c = new Cell(table._metaData[j].Name, table._metaData[j].Type, StringHelper.Trim(cols[j]));
                cells.Add(c);
            }

            if (whereClause == null)
            {
                res.Add(table[i]);
            }
            else if (Where(cells, whereClause))
            {
                res.Add(table[i]);
            }
        }
        
        for(int i =0; i<res.Count; i++)
        {
            var s = "";
            var cols = StringHelper.MySplit(res[i], '\0');
            for (int j = 0; j < table._metaData.Count; j++)
            {
                if (colNames.Contains(table._metaData[j].Name))
                    s += cols[j] + ' ';
            }
            Console.WriteLine(s);
        }
        table.Dispose();
    }

    private bool Where(MyList<Cell> row, string expression)
    {
        string exp = BooleanParser.ExpresionToTrueOrFalse(row, expression);
        return BooleanParser.EvaluateBooleanExpression(exp);
    }   
}

