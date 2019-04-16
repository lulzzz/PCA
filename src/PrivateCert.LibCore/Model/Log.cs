using System;

namespace PrivateCert.LibCore.Model
{
    public class Log
    {
        public Log(Exception ex)
        {
            Exception = ex;
        }

        public Exception Exception { get; set; }
    }
}
