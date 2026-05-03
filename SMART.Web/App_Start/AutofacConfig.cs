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

            // Controllers
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            // Repositories
            builder.RegisterType<ItemUnitRepository>().As<IItemUnitRepository>().InstancePerRequest();
            builder.RegisterType<EmployeeRepository>().As<IEmployeeRepository>().InstancePerRequest();
            builder.RegisterType<EmployeeComplainRepository>().As<IEmployeeComplainRepository>().InstancePerRequest();
            builder.RegisterType<AiChatSessionRepository>().As<IAiChatSessionRepository>().InstancePerRequest();
            builder.RegisterType<AiChatMessageRepository>().As<IAiChatMessageRepository>().InstancePerRequest();
            builder.RegisterType<AiAgentSettingRepository>().As<IAiAgentSettingRepository>().InstancePerRequest();

            // Services
            builder.RegisterType<ItemUnitService>().As<IItemUnitService>().InstancePerRequest();
            builder.RegisterType<EmployeeService>().As<IEmployeeService>().InstancePerRequest();
            builder.RegisterType<EmployeeComplainService>().As<IEmployeeComplainService>().InstancePerRequest();

            builder.RegisterType<AiAgentService>().As<IAiAgentService>().InstancePerRequest();
            builder.RegisterType<AiLlmService>().As<IAiLlmService>().InstancePerRequest();

            // ERP AI Services
            builder.RegisterType<ErpAiDataService>().As<IErpAiDataService>().InstancePerRequest();
            builder.RegisterType<ErpSchemaService>().As<IErpSchemaService>().InstancePerRequest();
            builder.RegisterType<ErpSqlSafetyService>().As<IErpSqlSafetyService>().InstancePerRequest();
            builder.RegisterType<ErpSqlGeneratorService>().As<IErpSqlGeneratorService>().InstancePerRequest();
            builder.RegisterType<ErpQueryExecutorService>().As<IErpQueryExecutorService>().InstancePerRequest();
            builder.RegisterType<ErpRelevantTableService>().As<IErpRelevantTableService>().InstancePerRequest();
            builder.RegisterType<ErpSchemaRegistryService>().As<IErpSchemaRegistryService>().InstancePerRequest();


            // DB
            builder.RegisterType<ApplicationDbContext>().AsSelf().InstancePerRequest();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}