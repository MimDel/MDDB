using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SAA_MDDB
{
    internal class CommandHandler
    {
        private const string ListTable = "listtables";
        private const string TableInfo = "tableinfo";
        private const string DropTable = "droptable";
        private const string CreateTable = "createtable";
        private const string Insert = "insert";
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
                case TableInfo:
                    HandleTableInfo(param);
                    break;
                case DropTable:
                    HandleDropTable(param);
                    break;
                case CreateTable:
                    HandleCreateTable(param);
                    break;
                case Insert:
                    HandleInsert(param);
                    break;
                default:
                    Console.WriteLine("There is no command that matches your input.");
                    break;
            }
        }

        private void HandleListTable()
        {
            _dbManager.ListTables();
        }

        private void HandleTableInfo(string name)
        {
            if (!File.Exists(name))
            {
                Console.WriteLine("You can not get the info for a table that do not exist.");
                return;
            }

            _dbManager.TableInfo(name);
        }

        private void HandleDropTable(string name)
        {
            if (!File.Exists(name))
            {
                Console.WriteLine("You can not drop a table that do not exist.");
                return;
            }

            _dbManager.DropTable(name);
        }

        private void HandleCreateTable(string param)
        {
            var listCols = new MyList<Column>();
            int startOfInfo = StringHelper.IndexOf(param, '(');
            int endOfInfo = StringHelper.IndexOf(param, ')');
            string tableName = StringHelper.Substring(param, 0 , startOfInfo);
            if (tableName == "")
            {
                Console.WriteLine("You haven't specified the name of the table that you want to create.");
                return;
            }
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

                var colAttributeInfo = StringHelper.SplitAttributes(infoCols[1], '"', ' ');
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
                        if (!Validator.IsTypeValid(colTypeInfo.Value, colAttributeInfo[2]))
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

        private void HandleInsert(string param)
        {
            var startOfColData = StringHelper.IndexOf(param, '(');
            var endOfColData = StringHelper.IndexOf(param, ')');

            var tableInfo = StringHelper.MySplit(
                StringHelper.Trim(StringHelper.Substring(param, 0, startOfColData)),' ');

            if (StringHelper.Trim(tableInfo[0]) != "into")
            {
                Console.WriteLine("Missing the key word into.");
                return;
            }
            if (tableInfo.Length == 1)
            {
                Console.WriteLine("No table name specified.");
                return;
            }

            var tableName = StringHelper.Trim(tableInfo[1]);

            if (!File.Exists(tableName))
            {
                Console.WriteLine("You can not insert row in table that do not exist.");
                return;
            }

            var colNames = StringHelper.MySplit(
                StringHelper.Substring(param, startOfColData + 1, endOfColData),',');

            for (int i = 0; i < colNames.Length; i++)
                colNames[i] = StringHelper.Trim(colNames[i]);


            param = StringHelper.Trim(
                StringHelper.Substring(param, endOfColData + 1, param.Length));

            var startOfData = StringHelper.IndexOf(param, '(');
            
            if (StringHelper.Trim(StringHelper.Substring(param, 0, startOfData)) != "value")
            {
                Console.WriteLine("You are missing the key word VALUE.");
                return;
            }

            param = StringHelper.Substring(param, startOfData, param.Length);

            var result = ToBeInserted(colNames, param, tableName);
            if (result != null) 
            _dbManager.Insert(tableName, result);
        }

        //todo work no ToBeInserted
        private MyList<string> ToBeInserted(string[] colNames, string param, string tableName)
        {
            var rows = new MyList<string>();
            var sb = new StringBuilder();
            var df = new DataFileStreamArray(tableName);
            var output = new MyStringBuilder();
            var message = "";

            bool isInBracket = false;
            bool isInQuotation = false;

            foreach (var c in param)
            {
                if (c == '(' && !isInQuotation)
                {
                   isInBracket = true;
                   continue; 
                }

                if (c == '"')
                   isInQuotation = !isInQuotation;

                if (c == ')' && !isInQuotation)
                {
                    rows.Add(sb.ToString());
                    sb.Clear();
                    isInBracket = false;
                    continue;
                }
                
                if(isInBracket)
                sb.Append(c);
            }

            for (int i = 0; i < rows.Count; i++)
            {
                output.Clear();
                var data = StringHelper.SplitAttributes(rows[i], '"', ','); //todo remove the space
                if (data.Length > colNames.Length)
                {
                    Console.WriteLine("Invalid number of values.");
                    return null;
                }
                if (!FormatedRow(data, df._metaData, colNames, ref output, ref message))
                {
                    Console.WriteLine(message);
                    return null;
                }
                rows[i] = output.ToString();
            }
            df.Dispose();
            return rows;
        }

        private bool FormatedRow(string[] data, MyList<Column> metaCol, string[] col, ref MyStringBuilder output, ref string message )
        {
            bool isColSkipped = true;
            var dataIndex = 0;
            for (int i = 0; i < metaCol.Count; i++)
            {
                for (int j = 0; j < col.Length; j++)
                {
                    if (metaCol[i].Name == col[j])
                    { 
                        isColSkipped = false;
                        break;
                    }
                }

                if (isColSkipped)
                {
                    if (i == metaCol.Count - 1)
                    {
                        output.Append(metaCol[i].DefaultValue);
                    }
                    else
                    {
                        output.Append(metaCol[i].DefaultValue + '\0');
                    }
                }
                else
                {
                    if (!Validator.IsTypeValid(metaCol[i].Type, data[dataIndex]))
                    {
                        message = "Invalid data.";
                        return false;
                    }
                    if (i == metaCol.Count - 1)
                    {
                        output.Append(data[dataIndex++]);
                    }
                    else
                    { 
                        output.Append(data[dataIndex++] + '\0');
                    }
                }

                isColSkipped = true;
            }
            return true;
        }
    }
}
