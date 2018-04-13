using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Topshelf.Logging;

namespace Topshelf.Owin
{
    public class WebAppConfigurator
    {
        protected IDisposable WebApplication;

        protected readonly LogWriter Log;

        public string Scheme { get; private set; }
        public string Domain { get; private set; }
        public int Port { get; private set; }

        protected IDependencyResolver DependencyResolver;
        protected Action<HttpConfiguration> HttpConfigurator;
        protected Action<StartOptions> StartOptions;
        protected Action<IAppBuilder> AppBuilderConfigurator;

        public WebAppConfigurator()
        {
            Scheme = "http";
            Domain = "*";
            Port = 8080;

            Log = HostLogger.Get(typeof(WebAppConfigurator));
            DependencyResolver = null;

            HttpConfigurator = httpConfiguration => httpConfiguration.MapHttpAttributeRoutes();
            StartOptions = options => options.Urls.Add(new UriBuilder(Scheme, Domain, Port).ToString());
            AppBuilderConfigurator = appBuilder => { };
        }

      
        public WebAppConfigurator ListenOn(string scheme,string domain,int port)
        {
            Scheme = scheme;
            Domain = domain;
            Port = port;
            return this;
        }

        public WebAppConfigurator UseDependencyResolver(IDependencyResolver dependencyResolver)
        {
            DependencyResolver = dependencyResolver;
            return this;
        }

        public WebAppConfigurator ConfigureStartOptions(Action<StartOptions> startOptions)
        {
            StartOptions = startOptions;
            return this;
        }

        public WebAppConfigurator ConfigureHttp(Action<HttpConfiguration> httpConfigurator)
        {
            HttpConfigurator = httpConfigurator;
            return this;
        }

        public WebAppConfigurator ConfigureAppBuilder(Action<IAppBuilder> appBuilderConfigurator) 
        {
            AppBuilderConfigurator = appBuilderConfigurator;
            return this;
        }

        public void Start()
        {
            var options = new StartOptions();
            (StartOptions??throw new NullReferenceException("Start Options Configuration Not Set"))(options);

            Log.InfoFormat("[Topshelf.Owin] Starting OWIN self-host, listening on: {0}", string.Join(", ", options.Urls));
            
            WebApplication = WebApp.Start(options, Startup);
        }

        public void Stop()
        {
            Log.Info("[Topshelf.Owin] Stopping OWIN self-host");
            WebApplication.Dispose();
        }

        private void Startup(IAppBuilder appBuilder)
        {
            (AppBuilderConfigurator??throw new NullReferenceException("App Builder Configuration Not Set"))(appBuilder);

            var httpConfiguration = new HttpConfiguration();
            (HttpConfigurator??throw new NullReferenceException("HTTP Configuration Not Set"))(httpConfiguration);

            if (DependencyResolver != null)
                httpConfiguration.DependencyResolver = DependencyResolver;

            appBuilder.UseWebApi(httpConfiguration);
        }


    }

}