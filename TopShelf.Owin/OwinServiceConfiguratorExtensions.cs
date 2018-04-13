using System;
using Topshelf.ServiceConfigurators;

namespace Topshelf.Owin
{

    public static class OwinServiceConfiguratorExtensions
    {
        /// <summary>
        /// Configures the service to run an Owin Endpoint. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator">The service to attach this endpoint to.</param>
        /// <param name="appConfigurator">An action which configures the endpoint using the provided configurator.</param>
        /// <returns>The service configurator</returns>
        public static ServiceConfigurator<T> OwinEndpoint<T>(this ServiceConfigurator<T> configurator, Action<WebAppConfigurator> appConfigurator = null) where T : class
        {
            var config = new WebAppConfigurator();
            appConfigurator?.Invoke(config);

            configurator.BeforeStartingService(t => config.Start());
            configurator.AfterStoppingService(t => config.Stop());

            return configurator;
        }
    }
}
