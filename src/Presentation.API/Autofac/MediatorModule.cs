using Autofac;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using AF = Autofac;

namespace RU.Challenge.Presentation.API.Autofac
{
    public class MediatorModule : AF.Module
    {
        private readonly IEnumerable<Assembly> _assemblies;

        public MediatorModule(IEnumerable<Assembly> assemblies)
            => _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly).AsImplementedInterfaces();
            builder.Register<SingleInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.Register<MultiInstanceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => (IEnumerable<object>)c.Resolve(typeof(IEnumerable<>).MakeGenericType(t));
            });

            foreach (var assembly in _assemblies)
            {
                RegisterRequestHandlersFromAssembly(builder, assembly);
                RegisterNotificationHandlersFromAssembly(builder, assembly);
            }
        }

        private void RegisterRequestHandlersFromAssembly(AF.ContainerBuilder builder, Assembly assembly)
        {
            builder
                .RegisterAssemblyTypes(assembly)
                .As(t => t.GetInterfaces().Where(i => i.IsClosedTypeOf(typeof(IRequestHandler<>))));

            builder
                .RegisterAssemblyTypes(assembly)
                .As(t => t.GetInterfaces().Where(i => i.IsClosedTypeOf(typeof(IRequestHandler<,>))));
        }


        private void RegisterNotificationHandlersFromAssembly(AF.ContainerBuilder builder, Assembly assembly)
        {
            builder
                .RegisterAssemblyTypes(assembly)
                .As(t => t.GetInterfaces().Where(i => i.IsClosedTypeOf(typeof(INotificationHandler<>))));
        }
    }
}