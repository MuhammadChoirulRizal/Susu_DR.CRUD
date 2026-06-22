using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    internal class DAL
    {
        public static string GetLocalIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        return ip.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting local IP: " + ex.Message);
            }
            return "127.0.0.1";
        }

        public static string GetConnectionString()
        {
            return $"Data Source={GetLocalIPAddress()}\\CHOY;Initial Catalog=DBAkademikADO;User ID=sa;Password=123;";
        }

        public int CountMhs()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("sp_CountMahasiswa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter output = new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(output);
                conn.Open();
                cmd.ExecuteNonQuery();
                return (output.Value != DBNull.Value) ? Convert.ToInt32(output.Value) : 0;
            }
        }

        
    }
}