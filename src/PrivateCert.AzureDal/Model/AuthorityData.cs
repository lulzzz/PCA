using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivateCert.AzureDal.Model
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
