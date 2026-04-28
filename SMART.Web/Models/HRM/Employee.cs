using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.Web.Models.HRM
{
    [Table("Employees", Schema = "hrm")]
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }


        #region MyRegion
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        #endregion

        public virtual List<EmployeeComplain> EmployeeComplains { get; set; }
    }
}