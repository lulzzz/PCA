using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivateCert.Sql.Model
{
    [Table("Certificates")]
    public partial class Certificate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CertificateId { get; set; }

        public DateTime ExpirationDate { get; set; }

        public byte CertificateTypeId { get;set; }

        [ForeignKey("CertificateTypeId")]
        public CertificateType CertificateType { get;set; }

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

        public string PfxPassword { get; set; }
    }
}
