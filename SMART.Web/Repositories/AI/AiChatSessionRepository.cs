using SMART.Web.Models;
using SMART.Web.Models.AI;

namespace SMART.Web.Repositories.AI
{
    public class AiChatSessionRepository : RepositoryBase<AiChatSession>, IAiChatSessionRepository
    {
        ApplicationDbContext _context;

        public AiChatSessionRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }
    }

    public interface IAiChatSessionRepository : IRepository<AiChatSession>
    {

    }
}