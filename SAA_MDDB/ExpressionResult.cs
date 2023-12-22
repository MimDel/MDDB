using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_MDDB
{
    struct ExpressionResult
    {
        public string ErrorMasssage;
        public bool Result;
        public bool Success;

        public ExpressionResult(bool result, bool success, string ErrorMassage)
        {
            Result = result;
            Success = success;
            ErrorMasssage = ErrorMassage;
        }

        public ExpressionResult(bool result)
        {
            Result = result;
            Success = true;
            ErrorMasssage = "";
        }
    }
}
