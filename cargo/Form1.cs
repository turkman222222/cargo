using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace cargo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.AcceptButton = button1;
        }

        private SqlConnection GetSqlConnection()
        {
            string connectionString = @"Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False";
            SqlConnection connection = new SqlConnection(connectionString);
            return connection;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = GetSqlConnection())
                {
                    connection.Open();
                    string query = "SELECT * FROM polzov WHERE log = @login AND password = @password";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", textBox1.Text);
                        command.Parameters.AddWithValue("@password", textBox2.Text);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int roleId = Convert.ToInt32(reader["rol_id"]);
                                int userId = Convert.ToInt32(reader["id_polzov"]);

                                if (roleId == 2)
                                {
                                    MENU g = new MENU();
                                    g.Show();

                                    return;
                                }
                                if (roleId == 1)
                                {
                                    MENU2 jj = new MENU2(userId);
                                    jj.Show();

                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
                return;
            }

            MessageBox.Show("Неверный логин или пароль.");
        }


        private void button2_Click(object sender, EventArgs e)
        {
            reg jj = new reg();
            jj.Show();

           
        }
    }
}