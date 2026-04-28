using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.Web.Models.INV
{
    [Table("ItemUnits", Schema = "inv")]
    public class ItemUnit
    {
        [Key]
        public int Id { set; get; }

        [Required(ErrorMessage = " Unit name is required field")]
        [StringLength(100, ErrorMessage = "Maximum length should be 100")]

        public string UnitName { set; get; }

        [StringLength(100, ErrorMessage = "Maximum length should be 150")]
        public string Remark { set; get; }

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