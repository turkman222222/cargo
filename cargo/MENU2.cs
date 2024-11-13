using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace cargo
{
    public partial class MENU2 : Form
    {
        private int userId; // Идентификатор текущего пользователя
        string ConnectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False;";
        private SqlDataAdapter _adapter;
        private DataSet _dataSet;

        public MENU2(int userId)
        {
            InitializeComponent();
            this.userId = userId; // Сохраняем идентификатор пользователя
            LoadUserOrders(); // Загружаем заказы сразу при открытии формы
        }

        // Метод для загрузки заказов пользователя
        private void LoadUserOrders()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                // Запрос для получения заказов пользователя
                string query = $"SELECT zakaz.*, zakazchik.id_polz, zakazchik.name AS ZakazchikName, sbor.name AS SborName, drop_table.name_2 AS drop_id2  FROM zakaz " +
                               "JOIN zakazchik ON zakaz.id_zak = zakazchik.id_zak " +
                               "JOIN sbor ON zakaz.sbor_id = sbor.id " +
                               "JOIN drop_table ON zakaz.drop_id = drop_table.id " +
                               $"WHERE zakazchik.id_polz = {userId}";

                // Создаем адаптер данных
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    // Заполняем DataTable данными
                    DataTable ordersTable = new DataTable();
                    adapter.Fill(ordersTable);

                    // Настройка DataGridView
                    dataGridView1.DataSource = ordersTable; // Устанавливаем источник данных для DataGridView
                    dataGridView1.Columns["id_zak"].Visible = false;
                    dataGridView1.Columns["sbor_id"].Visible = false;
                    dataGridView1.Columns["drop_id"].Visible = false;

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

        // ... (Остальной код формы)
    }
}