using System.Web.Http;
using SimpleInjector;
using MediatR;
using AutoMapper;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using TechnosoftDay2.Context;
using SimpleInjector.Integration.WebApi;
using System;
using FluentValidation;
using System.Linq;
using MediatR.Pipeline;
using FluentValidation.WebApi;
using TechnosoftDay2.Validator;

namespace TechnosoftDay2
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Inisiasi Instance Container milik Simple Injector
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new SimpleInjector.Lifestyles.AsyncScopedLifestyle();

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            //Regist MediaTr
            RegisterMediatR(container);

            //Regist DB Context
            container.Register<CountryContext>(Lifestyle.Scoped);

            //Regist AutoMapper
            RegisterAutoMapper(container);

            //Regist FluentValidation
            //RegisterFluentValidation(container);
            RegisterValidators(container, Assembly.GetExecutingAssembly());

            //Regis Pipeline
            container.Collection.Register(typeof(INotificationHandler<>), Assembly.GetExecutingAssembly());
            container.Collection.Register(typeof(IRequestExceptionAction<,>), Assembly.GetExecutingAssembly());
            container.Collection.Register(typeof(IRequestExceptionHandler<,,>), Assembly.GetExecutingAssembly());
            container.Collection.Register(typeof(IPipelineBehavior<,>), new[]
            {
                typeof(RequestExceptionProcessorBehavior<,>),
                typeof(RequestExceptionActionProcessorBehavior<,>),
                typeof(RequestPreProcessorBehavior<,>),
                typeof(RequestPostProcessorBehavior<,>),
            });

            container.Collection.Register(typeof(IRequestPreProcessor<>));
            container.Collection.Register(typeof(IRequestPostProcessor<,>));

            //Regis ServiceProvider
            container.RegisterInstance<IServiceProvider>(container);

            container.Verify();
            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
            
        }


        private void RegisterMediatR(Container container)
        {
            container.Register(typeof(IRequestHandler<,>), Assembly.GetExecutingAssembly());
            container.RegisterSingleton<IMediator>(() =>
            {
                var mediator = new Mediator(container);
                return mediator;
            });
        }

        private void RegisterAutoMapper(Container container)
        {
            container.RegisterSingleton(() =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<AutoMapperProfiles>();
                });
                return config.CreateMapper();
            });
        }

        private static void RegisterValidators(Container container, Assembly assembly)
        {
            var validatorTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)));

            foreach (var validatorType in validatorTypes)
            {
                var validatorInterfaces = validatorType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));

                foreach (var validatorInterface in validatorInterfaces)
                {
                    container.Register(validatorInterface, validatorType);
                }
            }
        }
    }
}
