using System;

using Topshelf.Logging;
using Topshelf.Ninject;
using TopShelf.Owin;

namespace Topshelf.Owin.Ninject
{
	public static class NinjectOwinServiceConfiguratorExtensions
	{
		public static WebAppConfigurator UseNinjectDependencyResolver(this WebAppConfigurator configurator)
		{
			var log = HostLogger.Get(typeof(NinjectOwinServiceConfiguratorExtensions));
			var kernel = NinjectBuilderConfigurator.Kernel;

			if (kernel == null)
			{
				throw new Exception("You must call UseNinject() to use the Owin Topshelf Ninject integration.");
			}

			configurator.UseDependencyResolver(new NinjectDependencyResolver(kernel));

			log.Info("[Topshelf.Owin.Ninject] Owin Dependency Resolver configured to use Ninject.");

			return configurator;
		}
	}
}
