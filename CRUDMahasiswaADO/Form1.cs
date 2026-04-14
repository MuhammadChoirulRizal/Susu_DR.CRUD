using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class Form1 : Form

    {

        private readonly SqlConnection conn;
        private readonly string connectionString = "Data Source=LAPTOP-5LMNPAS3\\CHOY;Initial Catalog=DBAkademikADO;Integrated Security=True";
        public Form1()
        {
            InitializeComponent();
            conn = new SqlConnection(connectionString);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
        private void ConnectDatabase()
        {
            try
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                {
                    conn.Open();
                }
                MessageBox.Show("Konneksi Berhasil");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Konneksi Gagal:" + ex.Message);
            }
        }

       
}
