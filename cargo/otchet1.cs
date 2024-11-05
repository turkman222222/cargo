using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace cargo
{
    public partial class otchet1 : Form
    {
        public otchet1()
        {
            InitializeComponent();
        }

        private void otchet1_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadTotalStockValue(); // Вызов нового метода для загрузки общей стоимости
        }

        private void LoadData()
        {
            string connectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False"; // Замените на свои данные
            string query = @"SELECT 
    k.name AS CategoryName,
    dt.name_2 AS ProductName,
    ISNULL(SUM(CASE 
                   WHEN wo.OperationType = 'Приход' THEN wo.Quantity 
                   WHEN wo.OperationType = 'Отгрузка' THEN -wo.Quantity 
                   ELSE 0 
               END), 0) AS RemainingQuantity,
    ISNULL(SUM(CASE 
                   WHEN wo.OperationType = 'Приход' THEN wo.Quantity * dt.cost 
                   WHEN wo.OperationType = 'Отгрузка' THEN -wo.Quantity * dt.cost 
                   ELSE 0 
               END), 0) AS RemainingValue
FROM 
    dbo.drop_table dt
JOIN 
    dbo.kat k ON dt.catecoria = k.id
LEFT JOIN 
    dbo.WarehouseOperations wo ON dt.id = wo.ProductID
GROUP BY 
    k.name, dt.name_2
ORDER BY 
    k.name, dt.name_2;";

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

        private void LoadTotalStockValue()
        {
            string connectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False"; // Замените на свои данные
            string query = @"SELECT 
    ISNULL(SUM(CASE 
                   WHEN wo.OperationType = 'Приход' THEN wo.Quantity * dt.cost 
                   WHEN wo.OperationType = 'Отгрузка' THEN -wo.Quantity * dt.cost 
                   ELSE 0 
               END), 0) AS TotalStockValue
FROM 
    dbo.drop_table dt
LEFT JOIN 
    dbo.WarehouseOperations wo ON dt.id = wo.ProductID;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    var result = command.ExecuteScalar(); // Выполняем запрос и получаем одно значение
                    if (result != null)
                    {
                        // Предположим, что у вас есть Label с именем labelTotalStockValue
                        label1.Text = "Общая стоимость товаров: " + result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
