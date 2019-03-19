using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivateCert.Sqlite.Model
{
    [Table("Logs")]
    public class Log
    {
        [Required]
        public DateTime Date { get; set; }

        [Key]
        [Column("LogID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogId { get; set; }

        [Required(ErrorMessage = "A mensagem do log é obrigatória")]
        public string Message { get; set; }
    }
}