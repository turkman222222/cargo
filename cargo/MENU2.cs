using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace cargo
{
    public partial class MENU2 : Form
    {
        private int userId; // Идентификатор текущего пользователя

        public MENU2(int userId)
        {
            InitializeComponent();
            this.userId = userId; // Сохраняем идентификатор пользователя
            LoadUserOrders(); // Загружаем заказы сразу при открытии формы
        }

        // Метод для загрузки заказов пользователя
        private void LoadUserOrders()
        {
            string connectionString = @"Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM zakaz WHERE id_zak = @userId";
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@userId", userId); // Используйте правильное имя параметра
                    DataTable ordersTable = new DataTable();
                    adapter.Fill(ordersTable);
                    dataGridView1.DataSource = ordersTable;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // ... (Код для обработки клика по ячейке)
        }
    }
}