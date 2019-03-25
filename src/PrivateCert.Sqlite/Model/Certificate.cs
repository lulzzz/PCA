using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivateCert.Sqlite.Model
{
    [Table("Certificates")]
    public partial class Certificate
    {
        [Key]
        public int CertificateId { get; set; }

        public DateTime ExpirationDate { get; set; }

        public byte CertificateTypeId { get;set; }

        [Required]
        [StringLength(100)]
        public string SerialNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(250)]
        public string Thumbprint { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime? RevocationDate { get; set; }

        [Required]
        public byte[] PfxData { get; set; }

        public int? AuthorityId { get; set; }

        [ForeignKey("AuthorityId")]
        public virtual Certificate Authority { get; set; }
        
        [ForeignKey("CertificateId")]
        public virtual AuthorityData AuthorityData { get; set; }
    }
}
