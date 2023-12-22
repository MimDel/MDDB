using SAA_MDDB;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

//var dbManager = new DBManager();

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

CommandHandler commandHandler = new CommandHandler();
//commandHandler.HandleCommand("DropTable Person");
//commandHandler.HandleCommand("ListTables");

//commandHandler.HandleCommand("CreateTable Person(Id    :  int, Name   : string default \"Ivan Ivanov\", BirthDate: date default \"01.02.2022\")");
//commandHandler.HandleCommand("TableInfo Person");

//Console.WriteLine();

//commandHandler.HandleCommand("ListTables");

//commandHandler.HandleCommand("Insert    INTO    Person   (Id, Name, BirthDate) VALUE (69, \"Helena H\", \"02.04.2020\") , (42, \"Lisa l\", \"01.09.1999\")");

