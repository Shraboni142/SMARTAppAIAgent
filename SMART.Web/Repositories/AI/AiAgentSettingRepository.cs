using SMART.Web.Models;
using SMART.Web.Models.AI;

namespace SMART.Web.Repositories.AI
{
    public class AiAgentSettingRepository : RepositoryBase<AiAgentSetting>, IAiAgentSettingRepository
    {
        ApplicationDbContext _context;

        public AiAgentSettingRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }
    }

    public interface IAiAgentSettingRepository : IRepository<AiAgentSetting>
    {

    }
}