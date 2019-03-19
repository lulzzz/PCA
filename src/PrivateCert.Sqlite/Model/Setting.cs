using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivateCert.Sqlite.Model
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
