using SMART.Web.Models;
using SMART.Web.Models.INV;

namespace SMART.Web.Repositories.INV
{
    public class ItemUnitRepository : RepositoryBase<ItemUnit>, IItemUnitRepository
    {
        ApplicationDbContext _context;
        public ItemUnitRepository(ApplicationDbContext context)
        : base(context)
        {
            _context = (ApplicationDbContext)context;
        }

    }
    public interface IItemUnitRepository : IRepository<ItemUnit>
    {

    }
}
