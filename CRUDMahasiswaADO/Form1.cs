using ExcelDataReader;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class Form1 : Form
    {
        DAL dbLogic = new DAL();
        private BindingSource bindingSource = new BindingSource();
        private DataTable dtMahasiswa = new DataTable();

        public Form1()
        {
            InitializeComponent();
        }

        public void SimpanLog(string message)
        {
            dbLogic.InsertLog(message);
        }

        private void ConnectDatabase()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DAL.GetConnectionString())) // ✅ static call
                {
                    conn.Open();
                    MessageBox.Show("Koneksi berhasil!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi gagal: " + ex.Message);
            }
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DAL.GetConnectionString())) // ✅ static call
                {
                    conn.Open();
                    MessageBox.Show("Koneksi Berhasil");
                }
            }
            catch (SqlException ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("SQL Error :" + ex.Message);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("General Error :" + ex.Message);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void BtnInsert_Click(object sender, EventArgs e)
        {
            try
            {
                if (fotoMhs.Image == null)
                {
                    MessageBox.Show("Silakan upload foto terlebih dahulu!");
                    return;
                }

                byte[] imgBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    fotoMhs.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imgBytes = ms.ToArray();
                }

                dbLogic.InsertMhs(txtNIM.Text, txtNama.Text, txtAlamat.Text, cmbJK.Text,
                    dtpTanggalLahir.Value.Date, txtKodeProdi.Text, imgBytes);

                MessageBox.Show("Data berhasil disimpan!");
                LoadData();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("SQL Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("General Error: " + ex.Message);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] imgBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    fotoMhs.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imgBytes = ms.ToArray();
                }

                dbLogic.UpdateMhs(txtNIM.Text, txtNama.Text, txtAlamat.Text, cmbJK.Text,
                    dtpTanggalLahir.Value.Date, txtKodeProdi.Text, imgBytes);

                MessageBox.Show("Data mahasiswa berhasil diubah");
                ClearForm();
                btnLoad.PerformClick();
            }
            catch (SqlException ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("SQL Error :" + ex.Message);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("General Error :" + ex.Message);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dg = MessageBox.Show(
                    "Yakin ingin menghapus data?",
                    "Konfirmasi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (dg == DialogResult.Yes)
                {
                    dbLogic.DeleteMhs(txtNIM.Text);
                    MessageBox.Show("Data mahasiswa berhasil dihapus");
                    ClearForm();
                    btnLoad.PerformClick();
                }
            }
            catch (SqlException ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("SQL Error :" + ex.Message);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("General Error :" + ex.Message);
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataRow row = ((DataRowView)bindingSource[e.RowIndex]).Row;

                txtNIM.Text = row[0].ToString();
                txtNama.Text = row[1].ToString();
                cmbJK.Text = row[2].ToString();
                dtpTanggalLahir.Value = Convert.ToDateTime(row[3]);
                txtAlamat.Text = row[4].ToString();
                txtKodeProdi.Text = row[6].ToString();

                if (row[5] != DBNull.Value)
                {
                    byte[] imgBytes = (byte[])row[5];
                    using (MemoryStream ms = new MemoryStream(imgBytes))
                    {
                        fotoMhs.Image = Image.FromStream(ms);
                        fotoMhs.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
                else
                {
                    fotoMhs.Image = null;
                }

                txtNIM.Enabled = true;
            }
        }

        private void ClearForm()
        {
            txtNIM.Enabled = true;
            txtNIM.Clear();
            txtNama.Clear();
            cmbJK.SelectedIndex = -1;
            txtAlamat.Clear();
            txtKodeProdi.Clear();
            dtpTanggalLahir.Value = DateTime.Now;
            fotoMhs.Image = null;
            txtNIM.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.mahasiswaTableAdapter.Fill(this.dBAkademikADODataSet.Mahasiswa);

            cmbJK.DataSource = new string[] { "L", "P" };

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView1.CellClick += DataGridView1_CellClick;

            bindingNavigator1.BindingSource = bindingSource;

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                DataTable dt = dbLogic.GetMhs();
                MessageBox.Show("Jumlah baris: " + dt.Rows.Count); // ← tambah di sini

                bindingSource.DataSource = dt;
                dataGridView1.DataSource = bindingSource;

                if (dataGridView1.Columns["Foto"] is DataGridViewImageColumn fotoColumn)
                {
                    fotoColumn.ImageLayout = DataGridViewImageCellLayout.Stretch;
                }

                HitungTotal();

                dataGridView1.Enabled = true;
                btnImpDb.Enabled = true;
                btnInsert.Enabled = true;
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
                btnCari.Enabled = true;
                btnLoad.Enabled = true;
                btnReset.Enabled = true;
                btnTestInjection.Enabled = true;
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("Error detail: " + ex.Message + "\n\nStack: " + ex.StackTrace);
            }
        }
        private void HitungTotal()
        {
            try
            {
                int total = dbLogic.CountMhs();
                lblCountMhs.Text = "Total Mahasiswa : " + total;
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void BtnTestInjection_Click(object sender, EventArgs e)
        {
            try
            {
                dbLogic.TestInject(txtNIM.Text); // ✅ PascalCase
                LoadData();
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("safe"))
                {
                    SimpanLog(ex.Message);
                    MessageBox.Show("SQL Error : Unsafe UPDATE operation not allowed");
                }
                else
                {
                    SimpanLog(ex.Message);
                    MessageBox.Show("SQL Error :" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("General Error :" + ex.Message);
            }
        }

        private void BtnRekapData_Click(object sender, EventArgs e)
        {
            Form2 fm2 = new Form2();
            fm2.Show();
            this.Hide();
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp" // ✅ simplified init
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                fotoMhs.Image = Image.FromFile(ofd.FileName);
                fotoMhs.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void BtnImpExcel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "Excel Workbook|*.xlsx" })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                                {
                                    UseHeaderRow = true
                                }
                            });
                            DataTable dt = result.Tables[0];
                            dataGridView1.DataSource = dt;
                            dataGridView1.Enabled = false;

                            btnImpDb.Enabled = true;
                            btnInsert.Enabled = true;
                            btnUpdate.Enabled = true;
                            btnDelete.Enabled = true;
                            btnCari.Enabled = true;
                            btnLoad.Enabled = true;
                            btnReset.Enabled = false;
                            btnTestInjection.Enabled = false;
                        }
                    }
                }
            }
        }

        private void BtnImpDb_Click(object sender, EventArgs e)
        {
            DataTable dt = (dataGridView1.DataSource is BindingSource bs)
                           ? (DataTable)bs.DataSource
                           : (DataTable)dataGridView1.DataSource;

            if (dt == null) return;

            int sukses = 0;
            int duplikat = 0;

            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    string nim = row["NIM"].ToString().Trim();
                    string nama = row["Nama"].ToString().Trim();
                    string jk = row["JenisKelamin"].ToString().Trim();
                    string alamat = row["Alamat"].ToString().Trim();
                    string namaProdi = row.Table.Columns.Contains("NamaProdi")
                        ? row["NamaProdi"].ToString().Trim() : "TI01";

                    // ✅ Parsing DateTime lebih aman
                    DateTime tgl;
                    string tglStr = row["TanggalLahir"].ToString().Trim();
                    if (!DateTime.TryParse(tglStr, out tgl))
                    {
                        // Coba format dd/MM/yyyy
                        if (!DateTime.TryParseExact(tglStr,
                            new string[] {
        "dd/MM/yyyy",
        "d/M/yyyy",
        "yyyy-MM-dd",
        "MM/dd/yyyy",
        "dd-MM-yyyy",  // ✅ tambah ini
        "d-M-yyyy"     // ✅ tambah ini
                            },
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out tgl))
                        {
                            MessageBox.Show("Format tanggal tidak valid untuk NIM: " + nim + "\nNilai: " + tglStr);
                            continue;
                        }
                    }

                    string kodeProdi = namaProdi.ToLower().Contains("informatika") ? "TI01" : "SI01";
                    dbLogic.InsertMhs(nim, nama, alamat, jk, tgl, kodeProdi, null);
                    sukses++;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627 || ex.Number == 2601)
                        duplikat++;
                    else
                        System.Diagnostics.Debug.WriteLine("Error SQL: " + ex.Message);
                }
            }

            MessageBox.Show("Proses Selesai.\nBerhasil: " + sukses + "\nDuplikat (diabaikan): " + duplikat);
            LoadData();
        }
        private void BindingNavigator1_RefreshItems(object sender, EventArgs e) { }

        private void BtnCari_Click(object sender, EventArgs e) { }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            try
            {
                dbLogic.resetData();
                MessageBox.Show("Data berhasil direset");
                LoadData();
            }
            catch (SqlException ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("SQL Error :" + ex.Message);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("General Error :" + ex.Message);
            }
        }

        private void Label5_Click(object sender, EventArgs e) { }
    }
}