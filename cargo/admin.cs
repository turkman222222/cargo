using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
namespace cargo
{
    public partial class admin : Form
    {
        string ConnectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False";
        private SqlDataAdapter _adapter;
        private DataSet _dataSet;
        public admin()
        {
            InitializeComponent();
            loadData();
        }
        private void loadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    _adapter = new SqlDataAdapter("SELECT zakaz.*, zakazchik.name AS ZakazchikName, drop_table.name_2 AS drop_id2, sbor.name_3 AS SborName FROM zakaz INNER JOIN zakazchik ON (zakaz.id_zak = zakazchik.id) INNER JOIN drop_table ON (zakaz.drop_id = drop_table.id) INNER JOIN sbor ON (zakaz.sbor_id = sbor.id);", connection);
                    _dataSet = new DataSet();
                    _adapter.Fill(_dataSet);
                    dataGridView2.DataSource = _dataSet.Tables[0];
                    // Скрываем ненужные колонки
                    dataGridView2.Columns["id_zak"].Visible = false;
                    dataGridView2.Columns["sbor_id"].Visible = false;
                    dataGridView2.Columns["drop_id"].Visible = false;
                    // Устанавливаем заголовки колонок на русском языке
                    dataGridView2.Columns["id_zakaza"].HeaderText = "ID заказа";
                    dataGridView2.Columns["drop_id2"].HeaderText = "Товар";
                    dataGridView2.Columns["cost"].HeaderText = "Цена";
                    dataGridView2.Columns["date"].HeaderText = "Дата";
                    dataGridView2.Columns["ZakazchikName"].HeaderText = "Заказчик";
                    
                    dataGridView2.Columns["SborName"].HeaderText = "Сборщик";
                    dataGridView2.Columns["col"].HeaderText = "Количество";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка");
            }
        }
       
      
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                int rowIndex = dataGridView2.SelectedRows[0].Index;
                dataGridView2.Rows.RemoveAt(rowIndex);
                // Сохраняем изменения в базе данных
                SaveChanges();
                // Перезагружаем данные в DataGridView
                loadData();
            }
        }
        private void SaveChanges()
        {
            try
            {
                if (_adapter == null || _dataSet == null)
                {
                    MessageBox.Show("Ошибка: адаптер или набор данных не инициализированы", "Ошибка");
                    return;
                }
                SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(_adapter);
                // Обновляем только таблицу «zakaz» в наборе данных
                _adapter.Update(_dataSet);
                MessageBox.Show("Изменения успешно сохранены", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных: " + ex.Message, "Ошибка");
            }
        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        private void admin_Load(object sender, EventArgs e)
        {
            // Здесь можно выполнить дополнительные действия при загрузке формы
        }

        private void txtSborId_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           category kk = new category();
            kk.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            drop gg = new drop();
            gg.Show();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            ed_izm hh = new ed_izm();
            hh.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            post post = new post();
            post.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pol pol = new pol();
            pol.Show();
        }

        private void admin_Load_1(object sender, EventArgs e)
        {

        }
    }
}

