using SMART.Web.Models;
using SMART.Web.Models.AI;

namespace SMART.Web.Repositories.AI
{
    public class AiChatMessageRepository : RepositoryBase<AiChatMessage>, IAiChatMessageRepository
    {
        ApplicationDbContext _context;

        public AiChatMessageRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }
    }

    public interface IAiChatMessageRepository : IRepository<AiChatMessage>
    {

    }
}