using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PrivateCert.Lib.Features;
using PrivateCert.Lib.Model;

namespace PrivateCert.Lib.Infrastructure
{
    public class LibMappingProfile : Profile
    {
        public LibMappingProfile ()
        {
            CreateMap<Certificate, ListCertificates.CertificateVM>();
        }
    }
}
