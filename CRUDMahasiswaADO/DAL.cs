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

        public DataTable GetMhs()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("sp_GetMahasiswa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public void InsertMhs(string nim, string nama, string alamat, string jk, DateTime tgl, string prodi, byte[] foto)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertMahasiswa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pNIM", nim);
                cmd.Parameters.AddWithValue("@pNama", nama);
                cmd.Parameters.AddWithValue("@pAlamat", alamat);
                cmd.Parameters.AddWithValue("@pJenisKelamin", jk);
                cmd.Parameters.AddWithValue("@pTanggalLahir", tgl);
                cmd.Parameters.AddWithValue("@pKodeProdi", prodi);
                cmd.Parameters.Add("@pFoto", SqlDbType.VarBinary).Value = (object)foto ?? DBNull.Value;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateMhs(string nim, string nama, string alamat, string jk, DateTime tgl, string prodi, byte[] foto)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateMahasiswa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pNIM", nim);
                cmd.Parameters.AddWithValue("@pNama", nama);
                cmd.Parameters.AddWithValue("@pAlamat", alamat);
                cmd.Parameters.AddWithValue("@pJenisKelamin", jk);
                cmd.Parameters.AddWithValue("@pTanggalLahir", tgl);
                cmd.Parameters.AddWithValue("@pKodeProdi", prodi);
                cmd.Parameters.AddWithValue("@pFoto", (object)foto ?? DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteMhs(string nim)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteMahasiswa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NIM", nim);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void resetData()
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                new SqlCommand("DELETE FROM mahasiswa", conn).ExecuteNonQuery();
                new SqlCommand("INSERT INTO mahasiswa SELECT * FROM mahasiswa_backup", conn).ExecuteNonQuery();
            }
        }

        

    }
}