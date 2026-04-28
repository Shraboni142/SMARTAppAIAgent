using SMART.Web.Models;
using SMART.Web.Models.HRM;

namespace SMART.Web.Repositories.MST
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        ApplicationDbContext _context;
        public EmployeeRepository(ApplicationDbContext context)
        : base(context)
        {
            _context = (ApplicationDbContext)context;
        }

    }
    public interface IEmployeeRepository : IRepository<Employee>
    {

    }
}