using AutoMapper;
using PrivateCert.Lib.Model;

namespace PrivateCert.Sql.Infrastructure
{
    public class SqlMappingProfile : Profile
    {
        public SqlMappingProfile()
        {
            CreateMap<Log, PrivateCert.Sql.Model.Log>();
            CreateMap<PrivateCert.Sql.Model.Certificate, Certificate>();
        }
    }
}
