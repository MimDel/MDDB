using SAA_MDDB;

var dbManager = new DBManager();

var col1 = new Column("a", MDDBType.Int);
var col2 = new Column("b", MDDBType.Date);
var col3 = new Column("c",MDDBType.String);
col2.DefaultValue = "29.05.2001";
col1.IsAutoIncrement = true;
col2.IsAutoIncrement = true;

MyList<Column> colomns = new MyList<Column>();
colomns.Add(col1);
colomns.Add(col2);
colomns.Add(col3);

dbManager.CreateTable("Test", colomns);
dbManager.CreateTable("Test1", colomns);
dbManager.CreateTable("Test2", colomns);
dbManager.CreateTable("Test3", colomns);
dbManager.CreateTable("Test4", colomns);
dbManager.CreateTable("Test5", colomns);



var data = new DataFileStreamArray("Test2");
data[0] = "13\016.09.2022\0jrkdfjsklsmp";
data[1] = "13\016.03.2022\0p";

var data2 = new DataFileStreamArray("Test3");
data2[0] = "13\016.09.2022\0jrkdfmp";
data2[1] = "13\016.03.2022\0jmp";

for (int i = 0; i < 2; i++)
{
    Console.WriteLine(data2[i]); 
}
Console.WriteLine();
for (int i = 0; i < 2; i++)
{
    Console.WriteLine(data[i]);
}

dbManager.ListTables();
