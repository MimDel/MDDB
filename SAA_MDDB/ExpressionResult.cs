using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_MDDB
{
    struct ExpressionResult
    {
        public string ErrorMessage;
        public bool Result;
        public bool Success;

        public ExpressionResult(bool result, bool success, string ErrorMessage)
        {
            Result = result;
            Success = success;
            this.ErrorMessage = ErrorMessage;
        }

        public ExpressionResult(bool result)
        {
            Result = result;
            Success = true;
            ErrorMessage = "";
        }
    }
}
