using System;

namespace WPF_Salary_Management_Application.Models
{
    public class DatasetStatisticsModel
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal HighestSalary1 { get; set; }
        public decimal HighestSalary2 { get; set; }
        public decimal HighestSalary3 { get; set; }
        public decimal AverageSalary { get; set; }
        public decimal TotalSalary { get; set; }
        public int CountOfSalary { get; set; }
    }

}
