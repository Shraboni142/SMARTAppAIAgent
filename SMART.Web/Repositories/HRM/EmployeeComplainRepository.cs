using SMART.Web.Models;
using SMART.Web.Models.HRM;
using System.Collections.Generic;
using System.Linq;

namespace SMART.Web.Repositories.HRM
{
    public class EmployeeComplainRepository : RepositoryBase<EmployeeComplain>, IEmployeeComplainRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeComplainRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public IEnumerable<EmployeeComplain> GetLatestComplainsByEmployee()
        {
            var latestIds = _context.EmployeeComplains
                .Where(x => x.IsDeleted != true)
                .GroupBy(x => x.EmployeeCode)
                .Select(g => g.Max(x => x.Id))
                .ToList();

            var data = _context.EmployeeComplains
                .Where(x => latestIds.Contains(x.Id))
                .OrderByDescending(x => x.Id)
                .ToList();

            return data;
        }
    }

    public interface IEmployeeComplainRepository : IRepository<EmployeeComplain>
    {
        IEnumerable<EmployeeComplain> GetLatestComplainsByEmployee();
    }
}