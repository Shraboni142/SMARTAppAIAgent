using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.Web.Models.AI
{
    [Table("AiChatMessages")]
    public class AiChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Column("SessionId")]
        public int SessionId { get; set; }

        [ForeignKey("SessionId")]
        public virtual AiChatSession Session { get; set; }

        public string UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Mode { get; set; }

        [Required]
        public string UserMessage { get; set; }

        public string AiReply { get; set; }

        public DateTime CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}