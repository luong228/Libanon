using System.Web.Http;
using AutoMapper;
using Libanon_API.Repository;
using Libanon_API.Repository.IRepository;
using Libanon_API.Utility;
using Unity;
using Unity.WebApi;

namespace Libanon_API
{
	public static class UnityConfig
	{
		public static void RegisterComponents()
		{
			var container = new UnityContainer();
			
			// register all your components with the container here
			// it is NOT necessary to register your controllers
			
			// e.g. container.RegisterType<ITestService, TestService>();
			container.RegisterType<IUnitOfWork, UnitOfWork>();
			container.RegisterType<IEmailSender, EmailSender>();
			container.RegisterType<IJwtHandler, JwtHandler>();
			container.RegisterInstance<IMapper>(new Mapper(new MapperConfiguration(cfg =>
			{
				cfg.AddProfile<MapperConfig>();
			})));
			
			GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
		}
	}
}