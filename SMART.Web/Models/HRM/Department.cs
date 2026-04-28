using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.Web.Models.HRM
{
    [Table("Departments", Schema = "hrm")]
    public class Department
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }


        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
    }
}