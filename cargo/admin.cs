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
    public partial class admin : Form
    {
        string connectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=бавза;Integrated Security=False;Encrypt=False;";
        private SqlDataAdapter _adapter;
        private DataSet _dataSet;


        public admin()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
                SqlConnection connection = new SqlConnection(connectionString);
            
                _adapter = new SqlDataAdapter("SELECT * FROM zakaz", connection);
                _dataSet = new DataSet();
                _adapter.Fill(_dataSet);
                dataGridView2.DataSource = _dataSet.Tables[0];
            connection.Open();
        }

        private void btnAdd_Click(object sender, EventArgs e)
{
    if (string.IsNullOrEmpty(txtIdZak.Text) ||
        string.IsNullOrEmpty(txtCol.Text) || string.IsNullOrEmpty(txtCost.Text) ||
        string.IsNullOrEmpty(txtDropId.Text) || string.IsNullOrEmpty(txtSborId.Text) ||
        string.IsNullOrEmpty(txtDate.Text))
    {
        MessageBox.Show("Заполните все поля.", "Ошибка");
        return;
    }

    DateTime parsedDate;
    if (!DateTime.TryParse(txtDate.Text, out parsedDate))
    {
        MessageBox.Show("Введите корректную дату.", "Ошибка");
        return;
    }

    try
    {
        // Получите максимальный ID_ZAKAZA из таблицы zakaz
        int maxIdZakaza = 0;
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand("SELECT MAX(id_zakaza) FROM zakaz", connection);
            object result = command.ExecuteScalar();
            if (result != DBNull.Value)
            {
                maxIdZakaza = Convert.ToInt32(result);
            }
        }

        // Увеличьте максимальный ID на 1
        int newIdZakaza = maxIdZakaza + 1;

        // Создайте новую строку в DataSet
        DataRow newRow = _dataSet.Tables[0].NewRow();
        newRow["id_zakaza"] = newIdZakaza;
        newRow["date"] = parsedDate;
        newRow["id_zak"] = int.Parse(txtIdZak.Text);
        newRow["drop_id"] = int.Parse(txtDropId.Text);
        newRow["col"] = int.Parse(txtCol.Text);
        newRow["cost"] = decimal.Parse(txtCost.Text);
        newRow["sbor_id"] = int.Parse(txtSborId.Text);

        // Добавьте новую строку в DataTable
        _dataSet.Tables[0].Rows.Add(newRow);

        // Сохраните изменения в базе данных
        SaveChanges();

        // Обновите DataGridView после сохранения
        LoadData();

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
                if (string.IsNullOrEmpty(txtIdZak.Text) ||
                    string.IsNullOrEmpty(txtCol.Text) || string.IsNullOrEmpty(txtCost.Text) ||
                    string.IsNullOrEmpty(txtDropId.Text) || string.IsNullOrEmpty(txtSborId.Text) ||
                    string.IsNullOrEmpty(txtDate.Text))
                {
                    MessageBox.Show("Заполните все поля.", "Ошибка");
                    return;
                }

                DateTime parsedDate;
                if (!DateTime.TryParse(txtDate.Text, out parsedDate))
                {
                    MessageBox.Show("Введите корректную дату.", "Ошибка");
                    return;
                }

                // Обновляем данные в выбранной строке
                DataRow row = _dataSet.Tables[0].Rows[rowIndex];
                row["date"] = parsedDate;
                row["id_zak"] = int.Parse(txtIdZak.Text);
                row["drop_id"] = int.Parse(txtDropId.Text);
                row["col"] = int.Parse(txtCol.Text);
                row["cost"] = decimal.Parse(txtCost.Text);
                row["sbor_id"] = int.Parse(txtSborId.Text);

                // Сохраняем изменения в базе данных
                SaveChanges();

                // Обновляем DataGridView после сохранения
                LoadData();

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
                dataGridView2.Rows.RemoveAt(rowIndex); // Удалите строку из DataGridView

                SaveChanges(); // Сохраните изменения в базе данных
                LoadData(); // Перезагрузите данные в DataGridView
            }
        }




        private void SaveChanges()
        {
            //try
            //{
            //    if (_adapter == null || _dataSet == null)
            //    {
            //        MessageBox.Show("Ошибка: адаптер или набор данных не инициализированы.", "Ошибка");
            //        return;
            //    }

                SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(_adapter);
                // Обновляем только таблицу "zakaz" в DataSet
                _adapter.Update(_dataSet);
            //    MessageBox.Show("Изменения успешно сохранены.", "Успех");
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Ошибка при сохранении данных: " + ex.Message, "Ошибка");
            //}
        }




        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Заполните поля формы данными из выбранной строки
                txtIdZakaza.Text = dataGridView2.Rows[e.RowIndex].Cells["id_zakaza"].Value.ToString();
                txtDate.Text = dataGridView2.Rows[e.RowIndex].Cells["date"].Value.ToString();
                txtIdZak.Text = dataGridView2.Rows[e.RowIndex].Cells["id_zak"].Value.ToString();
                txtDropId.Text = dataGridView2.Rows[e.RowIndex].Cells["drop_id"].Value.ToString();
                txtCol.Text = dataGridView2.Rows[e.RowIndex].Cells["col"].Value.ToString();
                txtCost.Text = dataGridView2.Rows[e.RowIndex].Cells["cost"].Value.ToString();
                txtSborId.Text = dataGridView2.Rows[e.RowIndex].Cells["sbor_id"].Value.ToString();
            }
        }
    }
}