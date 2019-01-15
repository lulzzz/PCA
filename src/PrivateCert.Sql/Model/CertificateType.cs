using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivateCert.Sql.Model
{
    [Table("CertificateTypes")]
    public class CertificateType
    {
        public CertificateType()
        {
        }

        public CertificateType(byte certificateTypeId, string description)
        {
            CertificateTypeId = certificateTypeId;
            Description = description;
        }

        [Key]
        public byte CertificateTypeId { get;set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }
    }
}
