using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.Web.Models.AI
{
    [Table("AiAgentSettings")]
    public class AiAgentSetting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ProviderName { get; set; }

        [StringLength(500)]
        public string ApiBaseUrl { get; set; }

        public string ApiKey { get; set; }

        [StringLength(200)]
        public string ModelName { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}