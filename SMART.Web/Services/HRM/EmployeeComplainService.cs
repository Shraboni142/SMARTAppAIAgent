using SMART.Web.Models.HRM;
using SMART.Web.Repositories;
using SMART.Web.Repositories.HRM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SMART.Web.Services.HRM
{
    public interface IEmployeeComplainService
    {
        IEnumerable<EmployeeComplain> GetAllEmployeeComplains();
        EmployeeComplain GetEmployeeComplainDetails(int id);
        bool AddEmployeeComplain(EmployeeComplain model, HttpServerUtilityBase server);
        bool UpdateEmployeeComplain(EmployeeComplain model, HttpServerUtilityBase server);
        bool DeleteEmployeeComplain(int id);
        bool UpdateReviewStatus(int id, string reviewStatus);
        void SaveEmployeeComplain();
    }

    public class EmployeeComplainService : IEmployeeComplainService
    {
        private readonly IEmployeeComplainRepository _employeeComplainRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeComplainService(
            IEmployeeComplainRepository employeeComplainRepository,
            IUnitOfWork unitOfWork)
        {
            _employeeComplainRepository = employeeComplainRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<EmployeeComplain> GetAllEmployeeComplains()
        {
            return _employeeComplainRepository.GetLatestComplainsByEmployee();
        }

        public EmployeeComplain GetEmployeeComplainDetails(int id)
        {
            return _employeeComplainRepository.Get(u => u.Id == id && u.IsDeleted != true);
        }

        public bool AddEmployeeComplain(EmployeeComplain model, HttpServerUtilityBase server)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.ReviewStatus))
                {
                    model.ReviewStatus = "Pending";
                }

                SaveAttachmentFile(model, server);

                model.CreatedOn = DateTime.Now;
                model.IsActive = true;
                model.IsDeleted = false;

                _employeeComplainRepository.Add(model);
                SaveEmployeeComplain();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateEmployeeComplain(EmployeeComplain model, HttpServerUtilityBase server)
        {
            try
            {
                var existing = _employeeComplainRepository.Get(u => u.Id == model.Id && u.IsDeleted != true);

                if (existing == null)
                {
                    return false;
                }

                existing.EmployeeId = model.EmployeeId;
                existing.EmployeeCode = model.EmployeeCode;
                existing.EmployeeName = model.EmployeeName;
                existing.OffenceType = model.OffenceType;
                existing.OffenceDetails = model.OffenceDetails;
                existing.ComplainActionType = model.ComplainActionType;
                existing.ComplainActionDetails = model.ComplainActionDetails;
                existing.DateOfNotice = model.DateOfNotice;
                existing.EarlyWithdrawalDate = model.EarlyWithdrawalDate;

                if (string.IsNullOrWhiteSpace(model.ReviewStatus))
                {
                    existing.ReviewStatus = string.IsNullOrWhiteSpace(existing.ReviewStatus) ? "Pending" : existing.ReviewStatus;
                }
                else
                {
                    existing.ReviewStatus = model.ReviewStatus;
                }

                if (model.AttachmentFile != null && model.AttachmentFile.ContentLength > 0)
                {
                    SaveAttachmentFile(model, server);
                    existing.AttachmentFileName = model.AttachmentFileName;
                    existing.AttachmentFilePath = model.AttachmentFilePath;
                }

                existing.UpdatedOn = DateTime.Now;

                _employeeComplainRepository.Update(existing);
                SaveEmployeeComplain();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteEmployeeComplain(int id)
        {
            var details = _employeeComplainRepository.Get(u => u.Id == id && u.IsDeleted != true);

            if (details == null)
            {
                return false;
            }

            try
            {
                _employeeComplainRepository.Delete(details);
                SaveEmployeeComplain();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateReviewStatus(int id, string reviewStatus)
        {
            try
            {
                var existing = _employeeComplainRepository.Get(u => u.Id == id && u.IsDeleted != true);

                if (existing == null)
                {
                    return false;
                }

                existing.ReviewStatus = reviewStatus;
                existing.UpdatedOn = DateTime.Now;

                _employeeComplainRepository.Update(existing);
                SaveEmployeeComplain();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SaveEmployeeComplain()
        {
            _unitOfWork.Commit();
        }

        private void SaveAttachmentFile(EmployeeComplain model, HttpServerUtilityBase server)
        {
            if (model == null || model.AttachmentFile == null || model.AttachmentFile.ContentLength <= 0)
            {
                return;
            }

            int maxFileSize = 50 * 1024; // 50 KB
            if (model.AttachmentFile.ContentLength > maxFileSize)
            {
                throw new Exception("File size must be 50 KB or less.");
            }

            string folderPath = server.MapPath("~/Uploads/ComplainAction/");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string originalFileName = Path.GetFileName(model.AttachmentFile.FileName);
            string uniqueFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + originalFileName;
            string fullPath = Path.Combine(folderPath, uniqueFileName);

            model.AttachmentFile.SaveAs(fullPath);

            model.AttachmentFileName = originalFileName;
            model.AttachmentFilePath = "/Uploads/ComplainAction/" + uniqueFileName;
        }
    }
}