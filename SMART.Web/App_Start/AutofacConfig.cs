
using Autofac;
using Autofac.Integration.Mvc;
using SMART.Web.Models;
using SMART.Web.Repositories;
using SMART.Web.Repositories.AI;
using SMART.Web.Repositories.HRM;
using SMART.Web.Repositories.INV;
using SMART.Web.Repositories.MST;
using SMART.Web.Services.AI;
using SMART.Web.Services.HRM;
using SMART.Web.Services.INV;
using SMART.Web.Services.MST;
using System.Reflection;
using System.Web.Mvc;

namespace SMART.Web.App_Start
{
    public class AutofacConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            // Register all MVC controllers
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // repository
            builder.RegisterType<ItemUnitRepository>().As<IItemUnitRepository>().InstancePerRequest();
            builder.RegisterType<EmployeeRepository>().As<IEmployeeRepository>().InstancePerRequest();
            builder.RegisterType<EmployeeComplainRepository>().As<IEmployeeComplainRepository>().InstancePerRequest();
            builder.RegisterType<AiChatSessionRepository>().As<IAiChatSessionRepository>().InstancePerRequest();
            builder.RegisterType<AiChatMessageRepository>().As<IAiChatMessageRepository>().InstancePerRequest();
            builder.RegisterType<AiAgentSettingRepository>().As<IAiAgentSettingRepository>().InstancePerRequest();


            // service
            builder.RegisterType<ItemUnitService>().As<IItemUnitService>().InstancePerRequest();
            builder.RegisterType<EmployeeService>().As<IEmployeeService>().InstancePerRequest();
            builder.RegisterType<EmployeeComplainService>().As<IEmployeeComplainService>().InstancePerRequest();
            builder.RegisterType<AiAgentService>().As<IAiAgentService>().InstancePerRequest();
            builder.RegisterType<AiLlmService>().As<IAiLlmService>().InstancePerRequest();
            builder.RegisterType<ErpAiDataService>().As<IErpAiDataService>().InstancePerRequest();
            builder.RegisterType<ErpSchemaService>().As<IErpSchemaService>().InstancePerRequest();



            // Register DbContext
            builder.RegisterType<ApplicationDbContext>().AsSelf().InstancePerRequest();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}