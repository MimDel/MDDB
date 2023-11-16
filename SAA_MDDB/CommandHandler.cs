﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_MDDB
{
    internal class CommandHandler
    {
        private const string ListTable = "listtables";
        private const string TableInfo = "tableinfo";
        private const string DropTable = "droptable";
        private const string CreateTable = "createtable";
        private const string Int = "int";
        private const string String = "string";
        private const string Date = "date";

        private DBManager _dbManager = new DBManager();

        public void HandleCommand(string commandString)
        {
            commandString = StringHelper.RemoveExtraSpaces(commandString);
            commandString = commandString.ToLower();

            //todo for the ListTable command if sould not only RemoveExtraSpaces but RamoveSpaces

            if (commandString == ListTable)
            {
                HandleListTable();
                return;
            }

            int firstSpace = StringHelper.IndexOf(commandString, ' ');
            string command = StringHelper.Substring(commandString, 0, firstSpace);
            string param = StringHelper.Substring(commandString, firstSpace+1, commandString.Length);

            switch (command)
            {
                case TableInfo:  HandleTableInfo(param);
                    break;
                case DropTable: HandleDropTable(param);
                    break;
                case CreateTable: HandleCreateTable(param);
                    break;
            }
        }

        private void HandleListTable()
        {
            _dbManager.ListTables();
        }

        private void HandleTableInfo(string name)
        {
            _dbManager.TableInfo(name);
        }

        private void HandleDropTable(string name)
        {
            _dbManager.DropTable(name);
        }

        private void HandleCreateTable(string param)
        {
            var listCols = new MyList<Column>();
            int startOfInfo = StringHelper.IndexOf(param, '(');
            int endOfInfo = StringHelper.IndexOf(param, ')');
            string tableName = StringHelper.Substring(param, 0 , startOfInfo);
            string tableInfo = StringHelper.Substring(param, startOfInfo+1, endOfInfo);
            var tableCols = StringHelper.MySplit(tableInfo, ",");
            for (int i = 0; i < tableCols.Length; i++)
            {
                tableCols[i] = StringHelper.Trim(tableCols[i]);
                var infoCols  = StringHelper.MySplit(tableCols[i], ':');
                if (infoCols.Length != 2)
                {
                    Console.WriteLine("Invalid input.");
                    return;
                }
                infoCols[0] = StringHelper.Trim(infoCols[0]);
                infoCols[1] = StringHelper.Trim(infoCols[1]);
                if (!Validator.IsNameValid(infoCols[0]))
                {
                    Console.WriteLine($"The name of the {i} column is not valid.");
                    return;
                }

                var colAttributeInfo = StringHelper.MySplit(infoCols[1], ' ');
                var colTypeInfo = Validator.StringToMDDBType(colAttributeInfo[0]);
                bool autoIncrement = false;
                string defaultVal = "NULL";

                if (colAttributeInfo.Length == 4)
                {
                    Console.WriteLine($"The column can`t be auto_increment and default at the same time.");
                    return;
                }

                if (colAttributeInfo.Length > 3)
                {
                    Console.WriteLine($"The number of attributes is not right.");
                    return;
                }
                if (colAttributeInfo.Length > 1 && !(colAttributeInfo[1] == "auto_increment" || colAttributeInfo[1] == "default"))
                {
                    Console.WriteLine($"The attributes are invalid.");
                    return;
                }
                else
                {
                    if (colAttributeInfo.Length == 2 && colAttributeInfo[1] == "auto_increment")
                    {
                        if (colTypeInfo != MDDBType.Int)
                        {
                            Console.WriteLine($"A column that isn`t int can`t be auto_increment");
                            return;
                        }

                        autoIncrement = true;
                    }

                    if (colAttributeInfo.Length == 3 && colAttributeInfo[1] == "default")
                    {
                        if (!Validator.IsDefaultValid(colTypeInfo.Value, colAttributeInfo[2]))
                        {
                            Console.WriteLine($"Default value does not match the type of the column or is not volid.");
                            return;
                        }

                        defaultVal = colAttributeInfo[2];
                    }
                }
                if (colTypeInfo == null)
                {
                    Console.WriteLine($"The type of the {i} does not exist.");
                    return;
                } else
                {
                    var col = new Column(infoCols[0], colTypeInfo.Value);
                    col.DefaultValue = defaultVal;
                    col.IsAutoIncrement = autoIncrement;
                    listCols.Add(col);
                }
            }
            _dbManager.CreateTable(tableName,listCols);
        }
    }
}
