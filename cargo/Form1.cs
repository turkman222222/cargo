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
    public partial class sign_in : Form

    {
        private DataSet _userSet = new DataSet();
        private SqlDataAdapter _adapter;
        public sign_in()
        {
            InitializeComponent();
            this.AcceptButton = button1;

            string connectionString = @"Data Source=NEGGER;Initial Catalog=10241367;Integrated Security=True;Encrypt=False";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                string selectQuery = "SELECT * FROM polzov;";
                _adapter = new SqlDataAdapter(selectQuery, connection);
                _adapter.Fill(_userSet);
            }
        }

        private void sign_in_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in _userSet.Tables[0].Rows)
            {
                if (row["log"].ToString() == textBox1.Text && row["password"].ToString() == textBox2.Text)
                {
                    MENU p = new MENU();
                    p.Show();
                    return;


                }
            }
            reg r = new reg();
            r.Show();
        }
    }
}
