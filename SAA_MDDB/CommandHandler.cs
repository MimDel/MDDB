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
    }
}
