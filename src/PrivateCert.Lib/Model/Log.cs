using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivateCert.Lib.Model
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
