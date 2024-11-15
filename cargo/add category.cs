using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
namespace cargo

{
    public partial class edit_category : Form
    {
        private string connectionString = "Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False"; // Замените на вашу строку подключения
        private int selectedId; // Хранит ID выбранной категории

        public edit_category()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT id, name FROM dbo.kat;";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dt = new DataTable();

                connection.Open();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCategoryName.Text) && selectedId > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE dbo.kat SET name = @name WHERE id = @id;";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", txtCategoryName.Text.Trim());
                    command.Parameters.AddWithValue("@id", selectedId);

                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Категория успешно обновлена.", "Успех");
                    LoadCategories(); // Обновляем список категорий
                }
            }
            else
            {
                MessageBox.Show("Введите имя категории для редактирования.", "Ошибка");
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value);
                txtCategoryName.Text = dataGridView1.SelectedRows[0].Cells["name"].Value.ToString();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void edit_category_Load(object sender, EventArgs e)
        {

        }
    }
}
