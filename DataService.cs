using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Linq;
using Microsoft.Data.SqlClient;
using WPF_Salary_Management_Application.Models;
using System.Windows.Documents;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;

namespace WPF_Salary_Management_Application.Data
{
    public class DataService
    {
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SallariesDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public bool AddSalary(SalaryModel salary)
        {
            try
            {
                // Create a DataTable with the same structure as the database table
                DataTable dataTable = new DataTable("tblSalary");
                dataTable.Columns.Add("emp_Id", typeof(int));
                dataTable.Columns.Add("emp_Name", typeof(string));
                dataTable.Columns.Add("emp_Join_Month", typeof(int));
                dataTable.Columns.Add("emp_Join_Year", typeof(int));
                dataTable.Columns.Add("Salary", typeof(decimal));
                dataTable.Columns.Add("pay_Method", typeof(string));
                dataTable.Columns.Add("pay_Date", typeof(DateTime));

                // Create a new DataRow with the data from the SalaryModel
                DataRow newRow = dataTable.NewRow();
                newRow["emp_Id"] = salary.EmpId;
                newRow["emp_Name"] = salary.EmpName;
                newRow["emp_Join_Month"] = salary.EmpJoinMonth;
                newRow["emp_Join_Year"] = salary.EmpJoinYear;
                newRow["Salary"] = salary.Salary;
                newRow["pay_Method"] = salary.PayMethod;
                newRow["pay_Date"] = salary.PayDate;

                // Add the new DataRow to the DataTable
                dataTable.Rows.Add(newRow);

                // Use a DataAdapter to insert the data into the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = "INSERT INTO tblSalary (emp_Id, emp_Name, emp_Join_Month, emp_Join_Year, Salary, pay_Method, pay_Date) " +
                                        "VALUES (@empId, @empName, @empJoinMonth, @empJoinYear, @Salary, @paymentMethod, @paymentDate)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                    {
                        // Set up the parameters
                        cmd.Parameters.AddWithValue("@empId", salary.EmpId);
                        cmd.Parameters.AddWithValue("@empName", salary.EmpName);
                        cmd.Parameters.AddWithValue("@empJoinMonth", salary.EmpJoinMonth);
                        cmd.Parameters.AddWithValue("@empJoinYear", salary.EmpJoinYear);
                        cmd.Parameters.AddWithValue("@Salary", salary.Salary);
                        cmd.Parameters.AddWithValue("@paymentMethod", salary.PayMethod);
                        cmd.Parameters.AddWithValue("@paymentDate", salary.PayDate);

                        // Use a SqlDataAdapter to update the database
                        using (SqlDataAdapter adapter = new SqlDataAdapter())
                        {
                            adapter.InsertCommand = cmd;
                            int rowsAffected = adapter.Update(dataTable);

                            return rowsAffected > 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (log, display error message, etc.)
                Console.WriteLine($"Error adding salary: {ex.Message}");
                return false;
            }
        }
        public bool EditSalary(SalaryModel salary)
        {
            try
            {
                // Create a new connection with the connection string
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Open the connection
                    connection.Open();

                    // Check if the month is valid
                    if (salary.EmpJoinMonth < 1 || salary.EmpJoinMonth > 12)
                    {
                        Console.WriteLine("Invalid month. Month should be between 1 and 12.");
                        return false;
                    }

                    // Construct the update query
                    string query = "UPDATE tblSalary SET emp_Id=@empId," +
                                    "emp_Name=@empName," +
                                    "emp_Join_Month=@empJoinMonth," +
                                    "emp_Join_Year=@empJoinYear," +
                                    "Salary=@Salary," +
                                    "pay_Method=@paymentMethod," +
                                    "pay_Date=@paymentDate " +
                                    "WHERE Id=@id";

                    // Show the query in the console
                    Console.WriteLine($"Executing query: {query}");

                    // Create a new SqlCommand with the query and connection
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        // Set the parameters for the query
                        cmd.Parameters.AddWithValue("@empId", salary.EmpId);
                        cmd.Parameters.AddWithValue("@empName", salary.EmpName);
                        cmd.Parameters.AddWithValue("@empJoinMonth", salary.EmpJoinMonth);
                        cmd.Parameters.AddWithValue("@empJoinYear", salary.EmpJoinYear);
                        cmd.Parameters.AddWithValue("@Salary", salary.Salary);
                        cmd.Parameters.AddWithValue("@paymentMethod", salary.PayMethod);
                        cmd.Parameters.AddWithValue("@paymentDate", salary.PayDate);
                        cmd.Parameters.AddWithValue("@id", salary.Id); // Assuming Id is part of your SalaryModel

                        // Execute the query and get the number of rows affected
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Return true if one or more rows were affected, indicating success
                        return rowsAffected > 0;
                    } // The connection is automatically closed when leaving the using block
                } // The connection is automatically disposed of when leaving the using block
            }
            catch (Exception ex)
            {
                // Handle exceptions (log, display error message, etc.)
                Console.WriteLine($"Error Editing salary: {ex.Message}");
                return false;
            }
        }


        public bool DeleteSalaryByEmployeeId(int empId)
        {
            try
            {
                // Create a DataSet to hold the data
                DataSet dataSet = new DataSet();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Construct the delete query
                    string query = "SELECT * FROM tblSalary WHERE emp_Id=@empId";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        // Set the parameter for the query
                        adapter.SelectCommand.Parameters.AddWithValue("@empId", empId);

                        // Fill the DataSet with the data
                        adapter.Fill(dataSet, "tblSalary");

                        // Check if any rows were found
                        if (dataSet.Tables["tblSalary"].Rows.Count > 0)
                        {
                            // Mark the row for deletion
                            dataSet.Tables["tblSalary"].Rows[0].Delete();

                            // Update the database with the changes
                            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                            adapter.Update(dataSet, "tblSalary");

                            return true;
                        }
                        else
                        {
                            // No matching rows found
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (log, display error message, etc.)
                Console.WriteLine($"Error Deleting salary: {ex.Message}");
                return false;
            }
        }


        public DataTable GetSalaryData()
        {
            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM tblSalary";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    adapter.Fill(dataTable);
                }
            }

            return dataTable;
        }

        public List<double> GetSalaries(int month, int year)
        {
            List<double> salaries = new List<double>();

            try
            {
                // Create a DataTable to hold the data
                DataTable dataTable = new DataTable();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = "SELECT Salary FROM tblSalary WHERE emp_Join_Month=@month AND emp_Join_Year=@year";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@month", month);
                        adapter.SelectCommand.Parameters.AddWithValue("@year", year);

                        // Fill the DataTable with the data
                        adapter.Fill(dataTable);

                        // Iterate through the rows and add salaries to the list
                        foreach (DataRow row in dataTable.Rows)
                        {
                            salaries.Add(Convert.ToDouble(row["Salary"]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or log it as needed
                Console.WriteLine($"Error getting salaries: {ex.Message}");
            }

            return salaries;
        }

        public void InsertDatasetStatistics()
        {
            try
            {
                // Create a DataTable to hold the data
                DataTable dataTable = new DataTable();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Creating the HR_Department table if it does not exist
                    string createTableQuery = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'HR_Department')
                BEGIN
                    CREATE TABLE [sallariesDatabase].[dbo].[HR_Department] (
                        Id INT IDENTITY (1, 1) NOT NULL,
                        FileName NVARCHAR (50)  NULL,
                        Month INT NOT NULL,
                        Year INT NOT NULL,
                        Highest_Sal_1 DECIMAL (10, 2) NULL,
                        Highest_Sal_2 DECIMAL (10, 2) NULL,
                        Highest_Sal_3 DECIMAL (10, 2) NULL,
                        Total_Sal DECIMAL (10, 2) NULL,
                        Count_Of_Sal INT NULL,
                        avg_Sal NUMERIC (18) NULL,
                        PRIMARY KEY CLUSTERED (Id ASC)
                    )
                END";

                    using (SqlCommand createTableCommand = new SqlCommand(createTableQuery, connection))
                    {
                        createTableCommand.ExecuteNonQuery();
                    }

                    string sqlQuery = "SELECT * FROM tblSalary";

                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlQuery, connection))
                    {
                        adapter.Fill(dataTable);

                        if (dataTable.Rows.Count > 0)
                        {
                            var salaries = new List<double>();
                            string month = "";
                            int year = 0;

                            foreach (DataRow row in dataTable.Rows)
                            {
                                salaries.Add(Convert.ToDouble(row["Salary"]));
                                month = row["emp_Join_Month"].ToString();
                                year = Convert.ToInt32(row["emp_Join_Year"]);
                            }

                            // Arranging the salaries in order
                            var orderedSalaries = salaries.OrderByDescending(s => s).ToList();

                            double totalSalary = salaries.Sum();
                            double averageSalary = totalSalary / salaries.Count;

                            // Insert data into HR_Department table
                            string insertStatisticsQuery = "INSERT INTO [sallariesDatabase].[dbo].[HR_Department]" +
                                "(Month, Year, Highest_Sal_1, Highest_Sal_2, Highest_Sal_3, Total_Sal, avg_Sal, Count_Of_Sal)" +
                                "VALUES (@month, @year, @firstHighest, @secondHighest, @thirdHighest, @total, @average, @count)";

                            using (SqlDataAdapter insertAdapter = new SqlDataAdapter())
                            {
                                insertAdapter.InsertCommand = new SqlCommand(insertStatisticsQuery, connection);
                                insertAdapter.InsertCommand.Parameters.AddWithValue("@month", month);
                                insertAdapter.InsertCommand.Parameters.AddWithValue("@year", year);
                                insertAdapter.InsertCommand.Parameters.AddWithValue("@firstHighest", orderedSalaries.ElementAtOrDefault(0));
                                insertAdapter.InsertCommand.Parameters.AddWithValue("@secondHighest", orderedSalaries.ElementAtOrDefault(1));
                                insertAdapter.InsertCommand.Parameters.AddWithValue("@thirdHighest", orderedSalaries.ElementAtOrDefault(2));
                                insertAdapter.InsertCommand.Parameters.AddWithValue("@average", averageSalary);
                                insertAdapter.InsertCommand.Parameters.AddWithValue("@total", totalSalary);
                                insertAdapter.InsertCommand.Parameters.AddWithValue("@count", salaries.Count);

                                int rowsAffected = insertAdapter.InsertCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    MessageBox.Show("Statistics inserted successfully.");
                                }
                                else
                                {
                                    MessageBox.Show("Failed to insert statistics.");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("No data found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

    }
}
