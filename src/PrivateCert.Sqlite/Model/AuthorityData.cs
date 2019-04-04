using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace PrivateCert.Sqlite.Model
{
    [Table("AuthorityData")]
    public class AuthorityData
    {
        [Key]
        public int CertificateId { get; set; }

        [ForeignKey("CertificateID")]
        [Required]
        public Certificate Certificate { get; set; }

        public string FirstP7B { get; set; }

        public string SecondP7B { get; set; }
    }
}
