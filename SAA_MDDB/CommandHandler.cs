using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_MDDB
{
    internal class CommandHandler
    {
        private DBManager _dbManager = new DBManager();

        public void HandleCommand(string commandString)
        {
            commandString = StringHelper.RemoveExtraSpaces(commandString);
            commandString = commandString.ToLower();

            //todo for the ListTable command if sould not only RemoveExtraSpaces but RamoveSpaces

            if (commandString == "listtables")
            {
                HandleListTable();
                return;
            }

            int firstSpace = StringHelper.IndexOf(commandString, ' ');
            string command = StringHelper.Substring(commandString, 0, firstSpace);
            string param = StringHelper.Substring(commandString, firstSpace+1, commandString.Length);

            switch (command)
            {
                case "tableinfo":  HandleTableInfo(param);
                    break;
                case "droptable": HandleDropTable(param);
                    break;
                case "createtable": HandleCreateTable(param);
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
                    break;
                }
                infoCols[0] = StringHelper.Trim(infoCols[0]);
                infoCols[1] = StringHelper.Trim(infoCols[1]);
                if (!Validator.IsNameValid(infoCols[0]))
                {
                    Console.WriteLine($"The name of the {i} column is not valid.");
                    break;
                }
                var colAttributeInfo = StringHelper.MySplit(infoCols[1], ' ');
                var colTypeInfo = Validator.StringToMDDBType(colAttributeInfo[0]);
                if (colTypeInfo == null)
                {
                    Console.WriteLine($"The type of the {i} does not exist.");
                    break;
                } else
                {
                    var col = new Column(infoCols[0], colTypeInfo.Value);
                    listCols.Add(col);
                }
            }
            _dbManager.CreateTable(tableName,listCols);

        }
    }
}
