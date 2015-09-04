using System;
using System.Web.Http;
using Ninject.Modules;
using Topshelf;
using Topshelf.Ninject;
using TopShelf.Owin;
using Topshelf.Owin.Ninject;

namespace Samples.Topshelf.Owin.Ninject
{
	internal class Program
	{
		private static void Main()
		{
			HostFactory.Run(c =>
			{
				c.UseNinject(new SampleModule()); //Initiates Ninject and consumes Modules

				c.Service<SampleService>(
					serviceConfigurator =>
					{
						serviceConfigurator.ConstructUsingNinject();
						serviceConfigurator.WhenStarted(p => p.Start());
						serviceConfigurator.WhenStopped(p => p.Stop());
						serviceConfigurator.OwinEndpoint(
							api =>
							{
								api.Domain = "localhost";
								api.Port = 8080;
								api.UseNinjectDependencyResolver();
								api.ConfigureHttp(http => Configure(http.Routes));
							});
					});
			});
		}

		private static void Configure(HttpRouteCollection routes)
		{
			routes.MapHttpRoute(
				"DefaultApiWithId",
				"Api/{controller}/{id}",
				new {id = RouteParameter.Optional},
				new {id = @"\d+"});
		}
	}

	public class SampleController : ApiController
	{
		private readonly ISampleDependency _dependency;

		public SampleController(ISampleDependency dependency)
		{
			_dependency = dependency;
		}

		public string Get(int id)
		{
			return string.Format("The id squared is: {0}", _dependency.Square(id));
		}
	}

	public class SampleModule : NinjectModule
	{
		public override void Load()
		{
			Bind<ISampleDependency>().To<SampleDependency>();
		}
	}

	public class SampleService
	{
		private readonly ISampleDependency _sample;

		public SampleService(ISampleDependency sample)
		{
			_sample = sample;
		}

		public bool Start()
		{
			Console.WriteLine("Sample Service Started.");
			Console.WriteLine("Sample Dependency: {0}", _sample);
			return _sample != null;
		}

		public bool Stop()
		{
			return _sample != null;
		}
	}

	public interface ISampleDependency
	{
		int Square(int id);
	}

	public class SampleDependency : ISampleDependency
	{
		public int Square(int id)
		{
			return id*id;
		}
	}
}