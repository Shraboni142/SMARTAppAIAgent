using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.Web.Models.MST
{
    [Table("Companies", Schema = "mst")]
    public class Company
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Slogan { get; set; }

        public string Contact { get; set; }
        public string Address { get; set; }
    }
}