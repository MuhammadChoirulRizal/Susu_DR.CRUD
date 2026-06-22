using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDMahasiswaADO
{
  
        internal class CetakData
        {
            static string connectionString ="Data Source=LAPTOP-5LMNPAS3\\CHOY;Initial Catalog=DBAkademikADO;User ID=sa;Password=123";

            SqlConnection conn = new SqlConnection(connectionString);
            SqlDataAdapter da;
            DataTable dtMahasiswa;

            DataMahasiswa listMahasiswa = new DataMahasiswa();

            // 2 references
            string prodi { get; set; }
            // 2 references
            DateTime tglmasuk { get; set; }
            public string Nama { get; set; }

        public string JenisKelamin { get; set; }

        public string Alamat { get; set; }

        public string NamaProdi { get; set; }

        public DateTime TanggalDaftar { get; set; }

    }
}
