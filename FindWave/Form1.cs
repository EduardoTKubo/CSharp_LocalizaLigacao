using FindWave.Classes;
using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindWave
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.Text = Application.ProductName.ToString() + " ".PadLeft(110) + Application.ProductVersion;
            
            cboBase.Items.Clear();
            listBase.Items.Clear();
            foreach (DataRow item in clsBanco.Consulta("select tabela ,descricao from Wave_Tabs where ativo = 1 order by id").Rows)
            {
                cboBase.Items.Add(item[0].ToString());
                listBase.Items.Add(item[1].ToString());
            }
            
            lblInfo.Text = "";
        }

        private void LimpaGrids()
        {
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.DataSource = "";
                dataGridView2.DataSource = "";
                txtWave.Text = "";
                lblOrigem.Text = "...";
                txtWaveDestino.Text = "";
            }
        }

        private void txtFone_KeyPress(object sender, KeyPressEventArgs e)
        {
            //e.Handled = Classes.clsFuncoes.IsNumeric(e);
        }


        private void cboBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            LimpaGrids();

            lblInfo.Text = "";
            listBase.SelectedIndex = cboBase.FindStringExact(cboBase.Text, listBase.SelectedIndex);
            lblInfo.Text = listBase.Text;
        }


        private async void btnFind_Click(object sender, EventArgs e)
        {
            LimpaGrids();

            if (txtFone.Text != "")
            {
                btnFind.Enabled = false;
                this.Cursor = Cursors.WaitCursor;

                // pesquisar
                clsVariaveis.GstrSQL = "select * from [" + cboBase.Text + "] where Fone = '" + txtFone.Text.Trim() + "' order by data,hora";

                dataGridView1.DataSource = await Classes.clsBanco.ConsultaAsync(clsVariaveis.GstrSQL);
                if (dataGridView1.Rows.Count > 1)
                {
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        column.SortMode = DataGridViewColumnSortMode.NotSortable;
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                    }
                }
                else
                {
                    MessageBox.Show("gravação não foi localizada ", "localizar .wave", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                btnFind.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }


        private async void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            dataGridView2.DataSource = "";
            lblOrigem.Text = "...";
            txtWaveDestino.Text = "";

            int nLin = dataGridView1.RowCount;
            if (nLin != 0)
            {
                txtWave.Text = Convert.ToString(dataGridView1[6, dataGridView1.CurrentRow.Index].Value);
                txtWaveDestino.Text = Convert.ToString(dataGridView1[6, dataGridView1.CurrentRow.Index].Value);
                lblOrigem.Text = "...";

                // pesquisar em wave
                clsVariaveis.GstrSQL = "select [path] as caminho_origem from [wave] where wave = '" + txtWave.Text + "' ";

                dataGridView2.DataSource = await Classes.clsBanco.ConsultaAsync(clsVariaveis.GstrSQL);
                foreach (DataGridViewColumn column in dataGridView2.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                }

                if (dataGridView2.Rows.Count == 2)
                { 
                    lblOrigem.Text = (Convert.ToString(dataGridView2[0, 0].Value)).Replace(txtWave.Text ,"");
                }

                btnCopyWave.Select();
            }
        }

        private void txtFone_TextChanged(object sender, EventArgs e)
        {
            LimpaGrids();
        }

        private void btnDestino_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = @"c:\\";
            if (folderBrowserDialog1.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            // mostra a pasta selecionada
            lblDestino.Text = folderBrowserDialog1.SelectedPath;
            this.Refresh();
        }
        
        private async void btnCopyWave_Click(object sender, EventArgs e)
        {
            if (lblOrigem.Text != "...")
            {
                if (lblDestino.Text != "...")
                {
                    btnCopyWave.Enabled = false;
                    this.Cursor = Cursors.WaitCursor;

                    try
                    {
                        await Task.Run(() =>
                        {
                            File.Copy(lblOrigem.Text + @"\" + txtWave.Text, lblDestino.Text + @"\" + txtWaveDestino.Text, false);

                            MessageBox.Show("Copiado com sucesso", "Copy .wave", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        });
                    }
                    catch (Exception ew)
                    {
                        MessageBox.Show(ew.Message, "Copy .wave", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    this.Cursor = Cursors.Default;
                    btnCopyWave.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Informar a pasta de destino", "Busca Ligação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Informar o arquivo desejado", "Busca Ligação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dataGridView2_DoubleClick(object sender, EventArgs e)
        {
            lblOrigem.Text = "...";

            if (dataGridView2.RowCount != 0)
            {                
                lblOrigem.Text = (Convert.ToString(dataGridView2[0, dataGridView2.CurrentRow.Index].Value)).Replace( txtWave.Text,"");
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == System.Windows.Forms.FormWindowState.Maximized)
            {
                this.WindowState = System.Windows.Forms.FormWindowState.Normal;
            }
        }

        private void txtFone_Leave(object sender, EventArgs e)
        {
            if (txtFone.Text != "")
            {
                txtFone.Text = clsFuncoes.RetornaNumero(txtFone.Text).ToString();

                //if (clsFuncoes.ValidaFone(txtFone.Text) == false)
                //{
                //    MessageBox.Show("Fone inválido", txtFone.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    txtFone.Select();
                //}
            }
        }
    }
}
