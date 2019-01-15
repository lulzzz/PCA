using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivateCert.Lib.Model
{
    public enum CertificateTypeEnum
    {
        Undefined = 0,
        Root = 1,
        Intermediate = 2,
        EndUser = 3
    }
}
