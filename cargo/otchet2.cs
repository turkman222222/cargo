using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cargo
{
    public partial class otchet2 : Form
    {
        public otchet2()
        {
            InitializeComponent();
            LoadData();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }
        private void LoadData()
        {
            string connectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False;"; // Замените на свои данные
            string query = @"
                WITH SalesData AS (
                    SELECT 
                        z.date AS SaleDate,
                        zc.name AS CustomerName,
                        dt.name_2 AS ProductName,
                        SUM(z.cost) AS TotalCost,
                        COUNT(z.id_zakaza) AS TotalOrders
                    FROM 
                        zakaz z
                    JOIN 
                        zakazchik zc ON z.id_zak = zc.id
                    JOIN 
                        drop_table dt ON z.drop_id = dt.id
                    GROUP BY 
                        z.date, zc.name, dt.name_2
                )
                SELECT 
                    SaleDate,
                    CustomerName,
                    SUM(TotalCost) AS TotalCost,
                    SUM(TotalOrders) AS TotalOrders
                FROM 
                    SalesData
                GROUP BY 
                    SaleDate, CustomerName
                UNION ALL
                SELECT 
                    SaleDate,
                    'Total' AS CustomerName,
                    SUM(TotalCost) AS TotalCost,
                    SUM(TotalOrders) AS TotalOrders
                FROM 
                    SalesData
                GROUP BY 
                    SaleDate
                ORDER BY 
                    SaleDate, CustomerName;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable; // Привязываем DataTable к DataGridView
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }
    }
}
