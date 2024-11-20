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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace cargo
{
    public partial class reg : Form
    {
        private DataSet _userSet = new DataSet();
        private SqlDataAdapter _adapter3;

        
        public reg()
        {
            InitializeComponent();
            string connectionString = @"Data Source=(localdb)\mssqllocaldb;Initial Catalog=""111111111111 (1)"";Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                string selectQuery = "SELECT * FROM polzov;";
                _adapter3 = new SqlDataAdapter(selectQuery, connection);
                _adapter3.Fill(_userSet);
            }
        }
        private void SaveData()
        {
            SqlCommandBuilder sqlCommandBuilder = new SqlCommandBuilder(_adapter3);
            _adapter3.Update(_userSet);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataRow newRow = _userSet.Tables[0].NewRow();
            newRow["name"] = textBox1.Text;
            newRow["log"] = textBox2.Text;
            newRow["password"] = textBox3.Text;
            newRow["mail"] = textBox4.Text;
            
            _userSet.Tables[0].Rows.Add(newRow);
            SaveData();
            MessageBox.Show("user added");
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void reg_Load(object sender, EventArgs e)
        {

        }
    }
}
