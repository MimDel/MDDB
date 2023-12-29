using SAA_MDDB;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

//var dbManager = new DBManager();
//MyList<Cell> list = new MyList<Cell>();
//list.Add(new Cell("Name", MDDBType.String, "ebi se \"be\""));
//list.Add(new Cell("B", MDDBType.Int, "5"));
//list.Add(new Cell("C", MDDBType.Int, "8"));
//list.Add(new Cell("Date", MDDBType.Date, "01.02.2020"));
//list.Add(new Cell("V", MDDBType.Int, "8"));
//list.Add(new Cell("F", MDDBType.Int, "7"));
//var col = new MyList<string>() {"name"};
//dbManager.Select("person",col,null);

//var col1 = new Column("a", MDDBType.Int);
//var col2 = new Column("b", MDDBType.Date);
//var col3 = new Column("c",MDDBType.String);
//col2.DefaultValue = "29.05.2001";
//col1.IsAutoIncrement = true;
//col2.IsAutoIncrement = true;

//MyList<Column> colomns = new MyList<Column>();
//MyList<string> data = new MyList<string>();
//colomns.Add(col1);
//colomns.Add(col2);
//colomns.Add(col3);

//for (int i = 0; i < 3000; i++)
//{
//    data.Add($"{i}\015.03.2003\0kekdlfne");
//}

//dbManager.CreateTable("Test", colomns);
//dbManager.CreateTable("Test1", colomns);
//dbManager.CreateTable("Test2", colomns);
//dbManager.CreateTable("Test3", colomns);
//dbManager.CreateTable("Test4", colomns);
//dbManager.CreateTable("Test5", colomns);

//dbManager.Insert("Test2",data);

//commandHandler.HandleCommand("DropTable Person");
//commandHandler.HandleCommand("ListTables");

//commandHandler.HandleCommand("CreateTable Person(Id    :  int, Name   : string default \"Ivan Ivanov\", BirthDate: date default \"01.02.2022\")");
//commandHandler.HandleCommand("TableInfo Person");

//Console.WriteLine();

//commandHandler.HandleCommand("ListTables");

//commandHandler.HandleCommand("Insert    INTO    Person   (Id, Name, BirthDate) VALUE (69, \"Helena H\", \"02.04.2020\") , (42, \"Lisa l\", \"01.09.1999\")");
//commandHandler.HandleCommand("Select Name, Id From Person Where Name = \"Helena H\"");

CommandHandler commandHandler = new CommandHandler();

Console.WriteLine("Welcome to MDDB.");
Console.Write("MDDB: ");
string command = Console.ReadLine();
while (command != "exit")
{
    commandHandler.HandleCommand(command);
    Console.Write("MDDB: ");
    command = Console.ReadLine();
}


