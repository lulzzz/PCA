﻿using AutoMapper;
using PrivateCert.Lib.Infrastructure;
using PrivateCert.Sqlite.Infrastructure;

namespace PrivateCert.CompositionRoot
{
    public class AutoMapperInitializer
    {
        public static void Initialize()
        {
            // O comando abaixo serve para buscar todas as classes que herdam de Profile dentro do assembly que contem a classe AutoMapperInitializer.
            Mapper.Initialize(cfg => { cfg.AddProfiles(typeof(SqlMappingProfile), typeof(LibMappingProfile)); });
        }
    }
}
