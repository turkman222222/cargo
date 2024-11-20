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
            string connectionString = @"Data Source=(localdb)\mssqllocaldb;Initial Catalog=""111111111111 (1)"";Integrated Security=True";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = $"SELECT zakaz.*, zakazchik.name AS ZakazchikName, drop_table.name_2 AS drop_id2, sbor.name_3 AS SborName FROM zakaz INNER JOIN zakazchik ON (zakaz.id_zak = zakazchik.id) INNER JOIN drop_table ON (zakaz.drop_id = drop_table.id) INNER JOIN sbor ON (zakaz.sbor_id = sbor.id) WHERE zakazchik.id_polz = {userId};";
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    DataTable ordersTable = new DataTable();
                    adapter.Fill(ordersTable);
                    dataGridView1.DataSource = ordersTable;
                    dataGridView1.Columns["id_zak"].Visible = false;
                    dataGridView1.Columns["sbor_id"].Visible = false;
                    dataGridView1.Columns["drop_id"].Visible = false;
                    dataGridView1.Columns["SborName"].Visible = false;
                    dataGridView1.Columns["id_zakaza"].Visible = false;
                    // Устанавливаем заголовки колонок на русском языке
                    dataGridView1.Columns["id_zakaza"].HeaderText = "ID заказа";
                    dataGridView1.Columns["drop_id2"].HeaderText = "Товар";
                    dataGridView1.Columns["cost"].HeaderText = "Цена";
                    dataGridView1.Columns["date"].HeaderText = "Дата";
                    dataGridView1.Columns["ZakazchikName"].HeaderText = "Заказчик";

                    dataGridView1.Columns["SborName"].HeaderText = "Сборщик";
                    dataGridView1.Columns["col"].HeaderText = "Количество";
                }
            }
        }

     

        private void button1_Click(object sender, EventArgs e)
        {
            sd_zakaz orderForm = new sd_zakaz(userId); // Передаем userId в конструктор
            orderForm.Show();
        }

        private void MENU2_Load(object sender, EventArgs e)
        {

        }
    }
}
