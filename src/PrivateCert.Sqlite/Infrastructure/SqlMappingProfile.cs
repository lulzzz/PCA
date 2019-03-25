using System.Collections.Generic;
using AutoMapper;
using PrivateCert.Sqlite.Model;

namespace PrivateCert.Sqlite.Infrastructure
{
    public class SqlMappingProfile : Profile
    {
        public SqlMappingProfile()
        {
            CreateMap<Log, Lib.Model.Log>();
            CreateMap<Certificate, Lib.Model.Certificate>();
            CreateMap<AuthorityData, Lib.Model.AuthorityData>();

            CreateMap<Lib.Model.Certificate, Certificate>();
            CreateMap<Lib.Model.AuthorityData, AuthorityData>();

        }
    }
}
