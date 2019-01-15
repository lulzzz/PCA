using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using PrivateCert.Lib.Interfaces;
using PrivateCert.Sql;
using PrivateCert.Sql.Repositories;
using StructureMap;

namespace PrivateCert.CompositionRoot
{
    public class SqlIoC
    {
        private static IContainer containerBase;

        public static IContainer GetNestedContainer()
        {
            return containerBase.GetNestedContainer();
        }

        public static void Dispose()
        {
            if (containerBase != null)
            {
                containerBase.Dispose();
            }
        }

        public static void InitializeBaseContainer()
        {
            if (containerBase != null)
            {
                throw new InvalidOperationException("Base Container already initialized.");
            }

            containerBase = new Container(
                c =>
                {
                    c.ForSingletonOf(typeof(AbstractValidator<>));

                    //c.For<ILogRepositorio>().Use<LogRepositorio<PortalContext>>();
                    ////c.For<IArquivoRepositorio>().Use<ArquivoRepositorio>();
                    //c.For<IAppDbSettingsRepositorio>().Use<AppDbSettingsRepositorio>();
                    ////c.For<ICertidaoNegativaRepositorio>().Use<CertidaoNegativaRepositorio>();
                    ////c.For<IContainerRepositorio>().Use<ContainerRepositorio>();
                    ////c.For<IDecisaoRepositorio>().Use<DecisaoRepositorio>();
                    ////c.For<IInteiroTeorRepositorio>().Use<InteiroTeorRepositorio>();
                    ////c.For<ISpocRepositorio>().Use<SpocRepositorio>();
                    ////c.For<IOrgaoRepositorio>().Use<OrgaoRepositorio>();                    
                    //c.For<IUsuarioRepositorio>().Use<UsuarioRepositorio>();
                    //c.For<IContrachequeRepositorio>().Use<ContrachequeRepositorio>();
                    c.ForConcreteType<PrivateCertContext>().Configure.ContainerScoped();
                    //c.ForConcreteType<UserManagerContext>().Configure.ContainerScoped();

                    c.For<IPrivateCertContext>().Use(f => f.GetInstance<PrivateCertContext>());
                    c.For<IUnitOfWork>().Use(f => f.GetInstance<PrivateCertContext>());
                    c.For<IPrivateCertRepository>().Use(f => f.GetInstance<PrivateCertRepository>());
                    ////c.For<ILogContext>().Use(f => f.GetInstance<PortalContext>());
                    ////c.For<IProcessoContext>().Use(f => f.GetInstance<PortalContext>());
                    //c.For<IPortalContext>().Use(f => f.GetInstance<PortalContext>());
                    ////c.For<IOrgaoContext>().Use(f => f.GetInstance<PortalContext>());
                    ////c.For<IArquivoContext>().Use(f => f.GetInstance<PortalContext>());
                    //c.For<IAppSettingsContext>().Use(f => f.GetInstance<PortalContext>());
                    //c.For<IUsuarioContext>().Use(f => f.GetInstance<PortalContext>());                   
                });
        }

    }
}
