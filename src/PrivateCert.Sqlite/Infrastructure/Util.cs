using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PrivateCert.Sqlite.Infrastructure
{
    public static class Util
    {
        public static string ToSqliteDateTime(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
