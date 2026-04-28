using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.Web.Models.AI
{
    [Table("AiChatSessions")]
    public class AiChatSession
    {
        public AiChatSession()
        {
            Messages = new HashSet<AiChatMessage>();
        }

        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [StringLength(250)]
        public string Title { get; set; }

        [Required]
        [StringLength(50)]
        public string Mode { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<AiChatMessage> Messages { get; set; }
    }
}