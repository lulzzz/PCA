using System;

namespace PrivateCert.AzureDal.Infrastructure
{
    public static class Util
    {
        public static string ToSqliteDateTime(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
