using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
namespace cargo
{
    public partial class admin : Form
    {
        string ConnectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False;";
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
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Проверка заполненности полей
            if (string.IsNullOrEmpty(txtIdZak.Text) || string.IsNullOrEmpty(txtCol.Text) || string.IsNullOrEmpty(txtCost.Text) || string.IsNullOrEmpty(txtDropId.Text) || string.IsNullOrEmpty(txtSborId.Text) || string.IsNullOrEmpty(txtDate.Text))
            {
                MessageBox.Show("Заполните все поля", "Ошибка");
                return;
            }
            // Проверка формата даты
            DateTime parsedDate;
            if (!DateTime.TryParseExact(txtDate.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                MessageBox.Show("Введите корректную дату в формате ДД.ММ.ГГГГ.", "Ошибка");
                return;
            }
            try
            {
                // Получаем максимальный id_zakaza из таблицы zakaz
                int maxIdZakaza = 0;
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT MAX(id_zakaza) FROM zakaz", connection);
                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        maxIdZakaza = Convert.ToInt32(result);
                    }
                }
                // Увеличиваем максимальный ID на 1
                int newIdZakaza = maxIdZakaza + 1;
                // Вставляем новую строку в таблицу zakaz
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO zakaz (id_zakaza, date, id_zak, drop_id, col, cost, sbor_id) VALUES (@id_zakaza, @date, @id_zak, @drop_id, @col, @cost, @sbor_id);", connection);
                    command.Parameters.AddWithValue("@id_zakaza", newIdZakaza);
                    command.Parameters.AddWithValue("@date", parsedDate);
                    // Используем TryParse для проверки ввода
                    int idZak, dropId, col, sborId;
                    decimal cost;
                    if (!int.TryParse(txtIdZak.Text, out idZak) ||
                        !int.TryParse(txtDropId.Text, out dropId) ||
                        !int.TryParse(txtCol.Text, out col) ||
                        !decimal.TryParse(txtCost.Text, out cost) ||
                        !int.TryParse(txtSborId.Text, out sborId))
                    {
                        MessageBox.Show("Проверьте корректность введённых данных.", "Ошибка");
                        return;
                    }
                    command.Parameters.AddWithValue("@id_zak", idZak);
                    command.Parameters.AddWithValue("@drop_id", dropId);
                    command.Parameters.AddWithValue("@col", col);
                    command.Parameters.AddWithValue("@cost", cost);
                    command.Parameters.AddWithValue("@sbor_id", sborId);
                    // Выполнение запроса
                    command.ExecuteNonQuery();
                }
                // Обновляем DataSet и DataGridView
                loadData();
                MessageBox.Show("Данные успешно добавлены.", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message, "Ошибка");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                int rowIndex = dataGridView2.SelectedRows[0].Index;
                // Проверяем, что все поля заполнены
                if (string.IsNullOrEmpty(txtIdZak.Text) || string.IsNullOrEmpty(txtCol.Text) || string.IsNullOrEmpty(txtCost.Text) || string.IsNullOrEmpty(txtDropId.Text) || string.IsNullOrEmpty(txtSborId.Text) || string.IsNullOrEmpty(txtDate.Text))
                {
                    MessageBox.Show("Заполните все поля.", "Ошибка");
                    return;
                }
                DateTime parsedDate;
                if (!DateTime.TryParseExact(txtDate.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                {
                    MessageBox.Show("Введите корректную дату.", "Ошибка");
                    return;
                }
                // Обновляем данные в выбранной строке
                DataRow row = _dataSet.Tables[0].Rows[rowIndex];
                row["date"] = parsedDate;
                int idZak, dropId, col, sborId;
                decimal cost;
                if (!int.TryParse(txtIdZak.Text, out idZak) ||
                    !int.TryParse(txtDropId.Text, out dropId) ||
                    !int.TryParse(txtCol.Text, out col) ||
                    !decimal.TryParse(txtCost.Text, out cost) ||
                    !int.TryParse(txtSborId.Text, out sborId))
                {
                    MessageBox.Show("Проверьте корректность введённых данных.", "Ошибка");
                    return;
                }
                row["id_zak"] = idZak;
                row["drop_id"] = dropId;
                row["col"] = col;
                row["cost"] = cost;
                row["sbor_id"] = sborId;
                // Сохраняем изменения в базе данных
                SaveChanges();
                // Обновляем DataGridView после сохранения
                loadData();
                MessageBox.Show("Данные успешно обновлены.", "Успех");
            }
            else
            {
                MessageBox.Show("Выберите строку для редактирования.", "Ошибка");
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
            if (e.RowIndex >= 0)
            {
                // Заполняем поля формы данными из выбранной строки
                txtDate.Text = dataGridView2.Rows[e.RowIndex].Cells["date"].Value.ToString();
                txtIdZak.Text = dataGridView2.Rows[e.RowIndex].Cells["id_zak"].Value.ToString();
                txtDropId.Text = dataGridView2.Rows[e.RowIndex].Cells["drop_id"].Value.ToString();
                txtCol.Text = dataGridView2.Rows[e.RowIndex].Cells["col"].Value.ToString();
                txtCost.Text = dataGridView2.Rows[e.RowIndex].Cells["cost"].Value.ToString();
                txtSborId.Text = dataGridView2.Rows[e.RowIndex].Cells["sbor_id"].Value.ToString();
            }
        }
        private void admin_Load(object sender, EventArgs e)
        {
            // Здесь можно выполнить дополнительные действия при загрузке формы
        }
    }
}

