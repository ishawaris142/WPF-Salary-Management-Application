using System;

namespace WPF_Salary_Management_Application.Models
{
    public class SalaryModel
    {
        public int Id { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public int EmpJoinMonth { get; set; }
        public int EmpJoinYear { get; set; }
        public decimal Salary { get; set; }
        public string PayMethod { get; set; }
        public DateTime PayDate { get; set; }
    }

}
