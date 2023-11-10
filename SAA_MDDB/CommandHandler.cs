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

            //todo for the ListTable command if sould not only RemoveExtraSpaces but RamoveSpaces

            if (commandString == "ListTables")
            {
                HandleListTable();
                return;
            }

            int firstSpace = StringHelper.IndexOf(commandString, ' ');
            string command = StringHelper.Substring(commandString, 0, firstSpace);
            string param = StringHelper.Substring(commandString, firstSpace+1, commandString.Length);

            switch (command)
            {
                case "TableInfo":  HandleTableInfo(param);
                    break;
                case "DropTable": HandleDropTable(param);
                    break;
                case "CreateTable": HandleCreateTable(param);
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
            int startOfInfo = StringHelper.IndexOf(param, '(');
            int endOfInfo = StringHelper.IndexOf(param, ')');
            string tableName = StringHelper.Substring(param, 0 , startOfInfo);
            string tableInfo = StringHelper.Substring(param, startOfInfo+1, endOfInfo);
            Console.WriteLine(tableName);
            Console.WriteLine(tableInfo);
            //var tableRow = StringHelper.MySplit(tableInfo, ", ");

        }
    }
}
