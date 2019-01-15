using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivateCert.Sql.Model
{
    [Table("Settings")]
    public class Setting
    {
        public Setting()
        {
        }

        public Setting(string settingId, string value)
        {
            SettingId = settingId;
            Value = value;
        }

        [Key]
        public string SettingId { get; set; }

        public string Value { get; set; }
    }
}
