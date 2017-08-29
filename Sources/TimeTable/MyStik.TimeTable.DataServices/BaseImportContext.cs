using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyStik.TimeTable.DataServices
{
    public class BaseImportContext
    {
        public BaseImportContext()
        {
            ErrorMessages = new Dictionary<string, List<ErrorMsg>>();
        }

        public Dictionary<string, List<ErrorMsg>> ErrorMessages { get; private set; }

        public void AddErrorMessage(string fileName, string msg, bool isError)
        {
            if (!ErrorMessages.ContainsKey(fileName))
            {
                ErrorMessages[fileName] = new List<ErrorMsg>();
            }

            ErrorMessages[fileName].Add(
                new ErrorMsg
                {
                    Message = msg,
                    IsError = isError
                });
        }

        public class ErrorMsg
        {
            public string Message { get; set; }

            public bool IsError { get; set; }
        }
    }
}
