using System;

namespace SMART.Web.TOs
{
    public class IdNameListTO
    {
        public int Id { set; get; }
        public string EmployeeId { set; get; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        // Farhad
        public DateTime? JoiningDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public int? GenderId { set; get; }
        public int? ReligionId { set; get; }
        public int? EmployeeTypeId { set; get; }
        public bool? IsChecked { set; get; }
        // Farhad
    }
}