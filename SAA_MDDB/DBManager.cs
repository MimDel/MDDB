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

    public MyList<int> Select(string name, MyList<string>? colNames, string? whereClause, bool distinct)
    {
        if (!_tableNames.Contains(name))
        {
            Console.WriteLine("The name of the table is not valid.");
            return new MyList<int>();
        }

        var table = new DataFileStreamArray(name);
        MyList<string> res = new MyList<string>();
        MyList<int> indexes = new MyList<int>();

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
                indexes.Add(i);
            }
            else if (Where(cells, whereClause))
            {
                res.Add(table[i]);
                indexes.Add(i);
            }
        }
        
        if(colNames == null)
        {
            table.Dispose();
            return indexes;
        }


        for(int i =0; i<res.Count; i++)
        {
            var s = "";
            var cols = StringHelper.MySplit(res[i], '\0');
            for (int j = 0; j < table._metaData.Count; j++)
            {
                if (colNames.Contains(table._metaData[j].Name))
                    s += cols[j] + ' ';
                else if (colNames == null)
                    s += cols[j] + ' ';
            }
            res[i] = s;
        }

        if (distinct)
        {
            for (int i = 0; i < res.Count; i++)
            {
                for (int j = i + 1; j < res.Count; j++)
                {
                    if (res[i] == res[j])
                    {
                        res.RemoveAt(j);
                    }
                }
            }
        }

        for (int i = 0; i < res.Count; i++)
            Console.WriteLine(res[i]);

        Console.WriteLine($"You have selected {res.Count} rows.");

        table.Dispose();
        return indexes;
    }

    public void Delete(string name, string? whereClause)
    {
        var indexes = Select(name, null, whereClause, false);
        File.Copy($"Meta_{name}", $"Meta_copy_{name}");

        var f = File.Create($"copy_{name}");
        var br = new BinaryWriter(f, Encoding.ASCII, true);
        br.Write(0);
        br.Close();
        f.Close();

        var table = new DataFileStreamArray(name);//Go - 0
        var copyTable = new DataFileStreamArray($"copy_{name}");
        int j = 0;
        int i = 0;
        for (; i < table._rowCount && j < indexes.Count; i++)
        {
            if (indexes[j] != i)
            {
                copyTable[copyTable._rowCount] = table[i];
            }
            else
                j++;
        }

        for (; i < table._rowCount; i++)
        {
            copyTable[copyTable._rowCount] = table[i];
        }

        table.Dispose();
        copyTable.Dispose();

        File.Delete($"Meta_{name}");
        File.Move($"Meta_copy_{name}", $"Meta_{name}");

        File.Delete(name);
        File.Move($"copy_{name}", name);

        Console.WriteLine($"You deleted {indexes.Count} rows from table - {name}");
    }

    private bool Where(MyList<Cell> row, string expression)
    {
        string exp = BooleanParser.ExpresionToTrueOrFalse(row, expression);
        return BooleanParser.EvaluateBooleanExpression(exp);
    }   
}

