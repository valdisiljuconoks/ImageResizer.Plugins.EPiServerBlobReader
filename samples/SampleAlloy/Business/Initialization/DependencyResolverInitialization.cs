using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;
using SampleAlloy.Business.Rendering;
using InitializationModule = EPiServer.Web.InitializationModule;

namespace SampleAlloy.Business.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(InitializationModule))]
    public class DependencyResolverInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<IContentRenderer, ErrorHandlingContentRenderer>();
            context.Services.AddTransient<ContentAreaRenderer, AlloyContentAreaRenderer>();

            DependencyResolver.SetResolver(new StructureMapDependencyResolver(context.StructureMap()));
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}