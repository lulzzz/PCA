using AutoMapper;
using PrivateCert.LibCore.Features;
using PrivateCert.LibCore.Model;

namespace PrivateCert.LibCore.Infrastructure
{
    public class LibMappingProfile : Profile
    {
        public LibMappingProfile ()
        {
            CreateMap<Certificate, ListCertificates.CertificateVM>();
        }
    }
}
