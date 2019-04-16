using System;
using FluentValidation;
using MediatR;
using MediatR.Pipeline;
using PrivateCert.LibCore.Interfaces;
using PrivateCert.AzureDal;
using PrivateCert.AzureDal.Repositories;
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
                cfg =>
                {
                    cfg.Scan(
                        scanner =>
                        {
                            // Carrega assembly Lib (que contém as interfaces)
                            scanner.Assembly("PrivateCert.Lib");
                            scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                            scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                        });

                    // Mediator
                    cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(RequestPreProcessorBehavior<,>));
                    cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(RequestPostProcessorBehavior<,>));
                    //cfg.For(typeof(IPipelineBehavior<,>)).Add(typeof(GenericPipelineBehavior<,>));
                    //cfg.For(typeof(IRequestPreProcessor<>)).Add(typeof(GenericRequestPreProcessor<>));
                    //cfg.For(typeof(IRequestPostProcessor<,>)).Add(typeof(GenericRequestPostProcessor<,>));
                    cfg.ForConcreteType<Mediator>().Configure.ContainerScoped();
                    cfg.For<IMediator>().Use(f=>f.GetInstance<Mediator>());
                    cfg.For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);

                    // Validator
                    cfg.ForSingletonOf(typeof(AbstractValidator<>));

                    // Lib -> Dal
                    cfg.ForConcreteType<PrivateCertContext>().Configure.ContainerScoped();
                    cfg.For<IPrivateCertContext>().Use(f => f.GetInstance<PrivateCertContext>());
                    cfg.For<IUnitOfWork>().Use(f => f.GetInstance<PrivateCertContext>());
                    cfg.For<IPrivateCertRepository>().Use(f => f.GetInstance<PrivateCertRepository>());
                });
        }

    }
}
