using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class Form3 : Form
    {
        string prodi { get; set; }
        DateTime tglmasuk { get; set; }

        public Form3(string Prodi, DateTime TglMasuk)
        {
            InitializeComponent();
            prodi = Prodi;
            tglmasuk = TglMasuk;

            try
            {
                DataTable dt = new DataTable();
                using (SqlConnection conn = new SqlConnection(DAL.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("sp_Report", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@inProdi", prodi);
                    cmd.Parameters.AddWithValue("@inTglMsuk", tglmasuk.Year.ToString());
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }

                ReportDocument report = new ReportDocument();
                report.Load(Application.StartupPath + "\\ListMahasiswa.rpt");
                report.SetDataSource(dt);

                crystalReportViewer1.ReportSource = report;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }
    }
}