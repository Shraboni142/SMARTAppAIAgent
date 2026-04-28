using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace SMART.Web.Models.HRM
{
    [Table("EmployeeComplains", Schema = "hrm")]
    public class EmployeeComplain
    {
        [Key]
        public int Id { get; set; }

        #region Employee
        public int? EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        #endregion

        public string ReviewStatus { get; set; }

        public string OffenceType { get; set; }
        public string OffenceDetails { get; set; }

        public string ComplainActionType { get; set; }
        public string ComplainActionDetails { get; set; }

        public DateTime DateOfNotice { get; set; } = DateTime.Now;
        public DateTime? EarlyWithdrawalDate { get; set; }

        public string AttachmentFileName { get; set; }
        public string AttachmentFilePath { get; set; }

        [NotMapped]
        public HttpPostedFileBase AttachmentFile { get; set; }

        #region Audit
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        #endregion
    }
}