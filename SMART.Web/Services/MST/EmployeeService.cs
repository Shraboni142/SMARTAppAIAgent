using PagedList;
using SMART.Web.Models.HRM;
using SMART.Web.Repositories;
using SMART.Web.Repositories.MST;
using System;
using System.Collections.Generic;

namespace SMART.Web.Services.MST
{
    public interface IEmployeeService
    {
        IPagedList<Employee> GetModelPageList(string searchStr, string sCode, int page, int size, string flag);
        IEnumerable<Employee> GetAllEmployees();
        Employee GetEmployeeDetails(int EmployeeID);

        bool CheckIfExist(int id, string Code);

        bool AddEmployee(Employee Employee);
        bool UpdateEmployee(Employee Employee);
        bool DeleteEmployee(int EmployeeId);

        void SaveEmployee();
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _EmployeeReporitory;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IEmployeeRepository EmployeeReporitory, IUnitOfWork unitOfWork)
        {
            _EmployeeReporitory = EmployeeReporitory;

            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            var getAllEmployees = _EmployeeReporitory.GetMany(p => p.IsDeleted != true);
            return getAllEmployees;
        }


        public Employee GetEmployeeDetails(int ID)
        {
            var getEmployeeDetails = _EmployeeReporitory.Get(u => u.Id == ID);
            return getEmployeeDetails;
        }

        public bool AddEmployee(Employee model)
        {
            try
            {
                model.CreatedOn = DateTime.Now;
                _EmployeeReporitory.Add(model);
                SaveEmployee();
                return true;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                return false;
            }

        }

        public bool CheckIfExist(int id, string Code)
        {
            Code = !string.IsNullOrEmpty(Code) ? Code.Trim().ToLower() : "";

            int countUnit = _EmployeeReporitory.GetCount(r => r.Id != id && r.Code.Trim().ToLower() == Code);

            if (countUnit > 0)
                return true;
            return false;
        }

        public bool UpdateEmployee(Employee model)
        {
            try
            {
                model.UpdatedOn = DateTime.Now;
                _EmployeeReporitory.Update(model);
                SaveEmployee();
                return true;
            }

            catch (Exception ex)
            {
                ex.Message.ToString();
                return false;
            }
        }

        public void SaveEmployee()
        {
            _unitOfWork.Commit();
        }

        public bool DeleteEmployee(int UnitId)
        {
            var UnitDetails = _EmployeeReporitory.Get(u => u.Id == UnitId);
            try
            {
                _EmployeeReporitory.Delete(UnitDetails);
                SaveEmployee();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public IPagedList<Employee> GetModelPageList(string searchStr, string sCode, int page, int size, string flag)
        {
            return null;
        }
    }
}