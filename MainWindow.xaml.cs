using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WPF_Salary_Management_Application.Data;
using WPF_Salary_Management_Application.Models;

namespace WPF_Salary_Management_Application
{
    public partial class MainWindow : Window
    {
        private readonly DataService dataService = new DataService();
        internal static readonly object Instance;

        public MainWindow()
        {
            InitializeComponent();

            // Populate Month ComboBox
            for (int i = 1; i <= 12; i++)
            {
                cmbMonth.Items.Add(i);
            }

            // Populate Year ComboBox
            for (int year = 2000; year <= 2023; year++)
            {
                cmbYear.Items.Add(year);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            SalaryModel newSalary = new SalaryModel
            {
                EmpId = Convert.ToInt32(txtEmpId.Text),
                EmpName = txtEmpName.Text,
                EmpJoinMonth = Convert.ToInt32(txtJoinMonth.Text),
                EmpJoinYear = Convert.ToInt32(txtJoinYear.Text),
                Salary = Convert.ToDecimal(txtSalary.Text),
                PayMethod = txtPaymentMethod.Text,
                PayDate = dpPaymentDate.SelectedDate ?? DateTime.Now
            };

            if (dataService.AddSalary(newSalary))
            {
                MessageBox.Show("Salary added successfully!");
            }
            else
            {
                MessageBox.Show("Failed to add salary.");
            }
        }

       private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // Assuming txtSalaryId is a TextBox for the user to input the ID
            if (int.TryParse(txtSalaryId.Text, out int salaryIdToUpdate))
            {
                // Create a new SalaryModel with the updated data
                SalaryModel updatedSalary = new SalaryModel
                {
                    EmpId = Convert.ToInt32(txtEmpId.Text),
                    EmpName = txtEmpName.Text,
                    EmpJoinMonth = Convert.ToInt32(txtJoinMonth.Text),
                    EmpJoinYear = Convert.ToInt32(txtJoinYear.Text),
                    Salary = Convert.ToDecimal(txtSalary.Text),
                    PayMethod = txtPaymentMethod.Text,
                    PayDate = dpPaymentDate.SelectedDate ?? DateTime.Now,
                    Id = salaryIdToUpdate // Set the ID property
                };

                // Call the EditSalary method with the dynamic ID
                if (dataService.EditSalary(updatedSalary))
                {
                    MessageBox.Show("Salary Updated successfully!");
                }
                else
                {
                    MessageBox.Show("Failed to Edit salary");
                }
            }
            else
            {
                MessageBox.Show("Invalid Salary ID. Please enter a valid numeric ID.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string empId = txtEmpId.Text;

            // Assuming you have the employee ID of the salary you want to delete
            if (int.TryParse(empId, out int employeeIdToDelete))
            {
                // Now you can use employeeIdToDelete as an integer
                bool isDeleted = dataService.DeleteSalaryByEmployeeId(employeeIdToDelete);

                if (isDeleted)
                {
                    MessageBox.Show("Salary deleted successfully.");
                    // You might want to refresh your UI or perform other actions after deletion
                }
                else
                {
                    MessageBox.Show("Failed to delete salary.");
                }
            }
            else
            {
                MessageBox.Show("Invalid employee ID. Please enter a valid integer.");
            }
            // Clear other textboxes after deletion if needed
            txtEmpId.Text = "";
            txtEmpName.Text = "";
            txtJoinMonth.Text = "";
            txtJoinYear.Text = "";
            txtSalary.Text = "";
            txtPaymentMethod.Text = "";
            dpPaymentDate.Text = "";
        }


        private void ShowAllSalariesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Call the GetSalaryData method to retrieve the salary data
                DataTable salaryDataTable = dataService.GetSalaryData();

                // Check if the DataTable has any rows
                if (salaryDataTable.Rows.Count > 0)
                {
                    // Clear the existing items in the ListBox
                    listBox.Items.Clear();

                    // Loop through the rows and add them to the ListBox
                    foreach (DataRow row in salaryDataTable.Rows)
                    {
                        string salaryInfo = $"ID: {row["emp_Id"]}, Name: {row["emp_Name"]}, " +
                                                            $"Joining Month: {row["emp_Join_Month"]}, Joining Year: {row["emp_Join_Year"]}, " +
                                                            $"Salary: {row["Salary"]}, Payment Method: {row["pay_Method"]}, " +
                                                            $"Payment Date: {row["pay_Date"]}";
                        listBox.Items.Add(salaryInfo);
                    }
                }
                else
                {
                    MessageBox.Show("No salary records found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying salaries: {ex.Message}");
            }
        }


        private void cal_Salaries(int month, int year)
        {
            try
            {
                // Clear the RichTextBox content
                txtOutput.Document = new FlowDocument();

                // Get salaries from the data service
                var salaries = dataService.GetSalaries(month, year);

                if (salaries.Count > 0)
                {
                    // Arrange salaries in descending order
                    var orderedSalaries = salaries.OrderByDescending(s => s).ToList();

                    // Calculate total, average, and display the output
                    double totalSalary = salaries.Sum();
                    double averageSalary = totalSalary / salaries.Count;

                    // Append the output to the RichTextBox
                    txtOutput.AppendText($"1st Highest Salary: {orderedSalaries.ElementAtOrDefault(0)}\n" +
                                        $"2nd Highest Salary: {orderedSalaries.ElementAtOrDefault(1)}\n" +
                                        $"3rd Highest Salary: {orderedSalaries.ElementAtOrDefault(2)}\n" +
                                        $"Average Salary: {averageSalary}\n" +
                                        $"Total Salary: {totalSalary}\n" +
                                        $"Count of Salaries: {salaries.Count}");
                }
                else
                {
                    MessageBox.Show("No data found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }



        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Retrieve selected month from ComboBox
                int? selectedMonth = cmbMonth.SelectedValue as int?;

                // Retrieve selected year from ComboBox
                int? selectedYear = cmbYear.SelectedValue as int?;

                if (selectedMonth.HasValue && selectedYear.HasValue)
                {
                    // Call the cal_Salaries method with user-input parameters
                    cal_Salaries(selectedMonth.Value, selectedYear.Value);
                }
                else
                {
                    MessageBox.Show("Invalid month or year selected.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private void SalaryStatsButton_Click(object sender, RoutedEventArgs e)
        {
            dataService.InsertDatasetStatistics();
        }

       
    }




}
