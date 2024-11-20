using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace cargo
{
    public partial class drop : Form
    {
        private string connectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False";

        public drop()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                dt.id, 
                k.name AS Category, 
                dt.name_2 AS Название, 
                ei.ed_iz AS Unit, 
                dt.col_sk AS [Кол-во на складе], 
                dt.cost AS Цена, 
                p.name AS Post 
            FROM 
                drop_table dt 
            JOIN 
                kat k ON dt.catecoria = k.id 
            JOIN 
                ed_iz ei ON dt.ed_izm = ei.id 
            JOIN 
                post p ON dt.post_id = p.id";

                try
                {
                    connection.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dataGridView1.DataSource = dt;

                        // Установка заголовков столбцов (ИСПОЛЬЗУЕМ АЛИАСЫ ИЗ ЗАПРОСА)
                        dataGridView1.Columns["id"].HeaderText = "ID";
                        dataGridView1.Columns["Название"].HeaderText = "Название";
                        dataGridView1.Columns["Unit"].HeaderText = "Ед. измерения";
                        dataGridView1.Columns["Кол-во на складе"].HeaderText = "Кол-во на складе";
                        dataGridView1.Columns["Цена"].HeaderText = "Цена";
                        dataGridView1.Columns["Post"].HeaderText = "Поставщик";
                        dataGridView1.Columns["Category"].HeaderText = "Категория";


                        // Сделать столбец 'id' только для чтения
                        dataGridView1.Columns["id"].ReadOnly = true;
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Ошибка SQL: {ex.Message}", "Ошибка");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Непредвиденная ошибка: {ex.Message}", "Ошибка");
                }
            }
        }

        private void add_Click(object sender, EventArgs e)
        {
            ADD aDD = new ADD();
            aDD.Show();
        }

        private void edit_Click(object sender, EventArgs e)
        {
            Editdrop gg = new Editdrop();
            gg.Show();

            
            

            
        }

        private void delete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите товар для удаления.", "Ошибка");
                return;
            }

            if (MessageBox.Show("Вы действительно хотите удалить товар?", "Подтверждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                int id = (int)dataGridView1.SelectedRows[0].Cells["id"].Value;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM dbo.drop_table WHERE id = @id;";
                    try
                    {
                        connection.Open();
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@id", id);
                            command.ExecuteNonQuery();
                            MessageBox.Show("Товар удален.", "Успех");
                            LoadProducts();
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show($"Ошибка SQL: {ex.Message}", "Ошибка");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Непредвиденная ошибка: {ex.Message}", "Ошибка");
                    }
                }
            }
        }

        private void ClearTextBoxes()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
             // Очистка поля для ID
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Заполнить поля для редактирования
                
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["Категория"].Value.ToString();
                textBox2.Text = dataGridView1.SelectedRows[0].Cells["Название"].Value.ToString();
                textBox3.Text = dataGridView1.SelectedRows[0].Cells["Ед. измерения"].Value.ToString();
                textBox4.Text = dataGridView1.SelectedRows[0].Cells["Кол-во на складе"].Value.ToString();
                textBox5.Text = dataGridView1.SelectedRows[0].Cells["Цена"].Value.ToString();
                textBox6.Text = dataGridView1.SelectedRows[0].Cells["Поставщик"].Value.ToString();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void drop_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
