using AutoMapper;
using PrivateCert.AzureDal.Model;

namespace PrivateCert.AzureDal.Infrastructure
{
    public class SqlMappingProfile : Profile
    {
        public SqlMappingProfile()
        {
            CreateMap<Log, LibCore.Model.Log>();
            CreateMap<Certificate, LibCore.Model.Certificate>();
            CreateMap<AuthorityData, LibCore.Model.AuthorityData>();

            CreateMap<LibCore.Model.Certificate, Certificate>();
            CreateMap<LibCore.Model.AuthorityData, AuthorityData>();

        }
    }
}
