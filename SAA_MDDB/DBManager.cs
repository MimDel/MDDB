using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

    public MyList<int> Select(string name, MyList<string>? colNames, string? whereClause, bool distinct, MyList<string> orderClause)
    {
        if (!_tableNames.Contains(name))
        {
            Console.WriteLine("The name of the table is not valid.");
            return new MyList<int>();
        }

        var table = new DataFileStreamArray(name);
        MyList<MyList<Cell>> res = new MyList<MyList<Cell>>();
        MyList<int> indexes = new MyList<int>();

        LoadCells(name, whereClause, ref res, ref indexes, table);

        if (colNames == null)
        {
            return indexes;
        }

        if (orderClause.Count != 0)
        {
            OrderBy(ref res, orderClause);
        }

        var stringRes = FromCellsToRow(ref res);

        TrimCols(colNames, ref stringRes, table);

        if (distinct)
        {
            Distinct(stringRes);
        }

        for (int i = 0; i < stringRes.Count; i++)
            Console.WriteLine(stringRes[i]);

        Console.WriteLine($"You have selected {stringRes.Count} rows.");

        table.Dispose();
        return indexes;
    }

    private void LoadCells(string name, string? whereClause, ref MyList<MyList<Cell>> res, ref MyList<int> indexes, DataFileStreamArray table)
    {
        for (int i = 0; i < table._rowCount; i++)
        {
            var cells = new MyList<Cell>();
            var cols = StringHelper.MySplit(table[i], '\0');

            for (int j = 0; j < cols.Length; j++)
            {
                var c = new Cell(table._metaData[j].Name, table._metaData[j].Type, StringHelper.Trim(cols[j]));
                cells.Add(c);
            }

            if (whereClause == null)
            {
                res.Add(cells);
                indexes.Add(i);
            }
            else if (Where(cells, whereClause))
            {
                res.Add(cells);
                indexes.Add(i);
            }
        }
    }

    private void TrimCols(MyList<string>? colNames, ref MyList<string> res, DataFileStreamArray table)
    {
        for (int i = 0; i < res.Count; i++)
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
    }

    private void Distinct(MyList<string> res)
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

    private void OrderBy(ref MyList<MyList<Cell>> res, MyList<string>? orderClause)
    {
        for (int i = 0; i < res.Count - 1; i++)
        {
            for (int j = 0; j < res.Count - i - 1; j++)
            {
                if (CompareRows(res[j], res[j + 1], orderClause) > 0)
                {
                    MyList<Cell> tmp = res[j];
                    res[j] = res[j + 1];
                    res[j + 1] = tmp;
                }
            }
        }
    }

    private void QuickSort(ref MyList<MyList<Cell>> res, MyList<string>? orderClause, int l, int r)
    {
        if (l < r)
        {
            int pivot = Partition(ref res, orderClause, l, r);

            if (pivot > 1)
                QuickSort(ref res,orderClause, 1, pivot - 1);

            if(pivot+1<r)
                QuickSort(ref res,orderClause, pivot+1, r);
        }
    }

    private int Partition(ref MyList<MyList<Cell>> res, MyList<string>? orderClause, int l, int r)
    {
        MyList<Cell> temp;
        int pI = l++;

        while (true)
        {
            while (l <= r && CompareRows(res[l], res[pI], orderClause) <= 0)
                l++; 
            while(l<=r && CompareRows(res[r], res[pI], orderClause)>=0)
                r--;

            if (l >= r)
                break;
            else 
            {
                temp = res[l];
                res[l] = res[r];
                res[r] = temp;
            }
        }
        temp = res[r];
        res[r] = res[pI];
        res[pI] = temp;

        return r;
    }
    
    private int CompareRows(MyList<Cell> row1, MyList<Cell> row2, MyList<string>? orderClause)
    {
        int lastColInOrderClause = 0;
        int compareRes = -2;
        do
        {
            int col = 0;
            for (int i = 0; i < row1.Count; i++)
            {
                if (row1[i].ColName == orderClause[lastColInOrderClause])
                {
                    col = i;
                    break;
                }
            }
            compareRes = CompareCell(row1[col], row2[col]);
            lastColInOrderClause++;
        }
        while (compareRes == 0 && lastColInOrderClause < orderClause.Count);

        return compareRes;
    }

    private int CompareCell(Cell left, Cell right)
    {
        int result = -2;
        switch (left.Type)
        {
            case MDDBType.Int:
                result = int.Parse(left.Value).CompareTo(int.Parse(right.Value));
                break;
            case MDDBType.String:
                result = StringHelper.CompareStrings(left.Value, right.Value);
                break;
            case MDDBType.Date:
                result = DateTime.Compare(DateTime.Parse(left.Value), DateTime.Parse(right.Value));
                break;
        }

        return result;
    }
    private MyList<string> FromCellsToRow(ref MyList<MyList<Cell>> res)
    {
        var rows = new MyList<string>();
        for (int i = 0; i < res.Count; i++)
        {
            string s = "";
            for (int j = 0; j < res[i].Count; j++)
            {
                 s += res[i][j].Value + "\0";
            }
            rows.Add(s);
        }

        return rows;
    }

    public void Delete(string name, string? whereClause)
    {
        var indexes = Select(name, null, whereClause, false, null);
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

