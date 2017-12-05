using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleContactsManager
{
    public partial class DocumentListForm : Form
    {
        CancellationTokenSource cancellationTokenSource;

        public DocumentListForm()
        {
            InitializeComponent();

            Text = "Google Contacts Manager";
            Size = new System.Drawing.Size(780, 400);
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
            KeyDown += DocumentListForm_KeyDown;
            Shown += DocumentListForm_Shown;

            gridDocs.BackgroundColor = SystemColors.Window;
            gridDocs.AllowUserToOrderColumns = false;
            gridDocs.AllowUserToResizeRows = false;
            gridDocs.ColumnHeadersVisible = false;
            gridDocs.Dock = DockStyle.Fill;
            gridDocs.MultiSelect = true;
            gridDocs.ReadOnly = true;
            gridDocs.RowHeadersVisible = false;
            gridDocs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridDocs.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            gridDocs.RowTemplate.Height = 30;
            gridDocs.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", (float)9.75);
            gridDocs.BorderStyle = BorderStyle.None;
            gridDocs.CellBorderStyle = DataGridViewCellBorderStyle.None;

            gridDocs.DataError += gridDocs_DataError;
            gridDocs.SelectionChanged += gridDocs_SelectionChanged;
            gridDocs.CellDoubleClick += gridDocs_CellDoubleClick;
            gridDocs.KeyDown += gridDocs_KeyDown;

            googleContactsDocListBindingSource.DataSource = GoogleContactsDocList.DocumentList;

            toolStripProgressBar1.Visible = false;
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            toolStripProgressBar1.MarqueeAnimationSpeed = 50;

            prepareForTask();
        }

        #region Eventos
        private void DocumentListForm_Shown(object sender, EventArgs e)
        {
            taskIsCompleted();
        }
        private void DocumentListForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                Close();
            }
        }
        private void gridDocs_KeyDown(object sender, KeyEventArgs e)
        {
            GoogleContactsDoc dobj = getOneRowSelected();
            if (e.KeyCode == Keys.Enter && dobj != null)
            {
                showDocDetails(dobj);
                e.Handled = true;
            }
        }
        private void gridDocs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            GoogleContactsDoc dobj = getOneRowSelected();
            if (dobj != null)
            {
                showDocDetails(dobj);
            }
        }
        private void gridDocs_SelectionChanged(object sender, EventArgs e)
        {
            toolStripButtonSaveLocal.Enabled = (getOneRowSelected() != null);
            toolStripButtonSinc.Enabled = (getTwoRowsSelected() != null);
        }
        private void gridDocs_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }
        private void toolStripButtonCancelar_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
        private async void toolStripButtonPasta_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.ShowNewFolderButton = false;
                fbd.Description = "Escolher uma pasta onde se encontram os ficheiros de grupos e contactos";
                fbd.SelectedPath = GoogleContactsDocList.GetDefaultPathForFiles();
                if (fbd.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                prepareForTask();
                await Task.Run(() =>
                {
                    for (int i = 1; i < 4; i++)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        System.Threading.Thread.Sleep(100);
                    }
                    GoogleContactsDocList.AddPath(fbd.SelectedPath);
                    for (int i = 1; i < 4; i++)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        System.Threading.Thread.Sleep(100);
                    }
                });
            }
            catch (Exception ex)
            {
                handleTaskException(ex);
            }
            finally
            {
                taskIsCompleted();
            }
        }
        private async void toolStripButtonSaveLocal_Click(object sender, EventArgs e)
        {
            try
            {
                GoogleContactsDoc doc = getOneRowSelected();
                if (doc == null)
                {
                    return;
                }
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.Description = "Escolher uma pasta para guardar os ficheiros de grupos e contactos";
                fbd.SelectedPath = GoogleContactsDocList.GetDefaultPathForFiles();
                if (fbd.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                prepareForTask();
                await Task.Run(() =>
                {
                    for (int i = 1; i < 4; i++)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        System.Threading.Thread.Sleep(100);
                    }
                    doc.SaveToDisk(fbd.SelectedPath);
                    for (int i = 1; i < 4; i++)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        System.Threading.Thread.Sleep(100);
                    }
                });
            }
            catch (Exception ex)
            {
                handleTaskException(ex);
            }
            finally
            {
                taskIsCompleted();
            }
        }
        private async void toolStripButtonConta_Click(object sender, EventArgs e)
        {
            try
            {
                prepareForTask();
                //
                Task time = Task.Delay(60000, cancellationTokenSource.Token);
                Task main = GoogleContactsDocList.AddGoogle(cancellationTokenSource.Token);
                var r = await Task.WhenAny(main, time);                
                if (r.Status == TaskStatus.Canceled)
                {
                    throw new OperationCanceledException();
                }
                else if (time.Status == TaskStatus.RanToCompletion)
                {
                    throw new GoogleContactsException("Foi ultrapassado o tempo destinado a esta tarefa (60 segundos)");
                }
            }
            catch (Exception ex)
            {
                handleTaskException(ex);
            }
            finally
            {
                taskIsCompleted();
            }
        }
        private void toolStripButtonSinc_Click(object sender, EventArgs e)
        {
            GoogleContactsDoc[] docs = getTwoRowsSelected();
            if (docs != null)
            {
                MessageBox.Show(docs[0].ContaId + Environment.NewLine + docs[1].ContaId);
            }
        }
        #endregion

        #region Actions
        private GoogleContactsDoc getOneRowSelected()
        {
            if (gridDocs.SelectedRows.Count != 1 || gridDocs.CurrentRow == null)
            {
                return null;
            }
            return (GoogleContactsDoc) gridDocs.CurrentRow.DataBoundItem;
        }
        private GoogleContactsDoc[] getTwoRowsSelected()
        {
            if (gridDocs.SelectedRows.Count != 2)
            {
                return null;
            }
            return new GoogleContactsDoc[] { (GoogleContactsDoc)gridDocs.SelectedRows[0].DataBoundItem, (GoogleContactsDoc)gridDocs.SelectedRows[1].DataBoundItem };
        }
        private void prepareForTask()
        {
            googleContactsDocListBindingSource.RaiseListChangedEvents = false;
            cancellationTokenSource = new CancellationTokenSource();
            toolStripButtonPasta.Enabled = false;
            toolStripButtonConta.Enabled = false;
            toolStripButtonSave.Enabled = false;
            toolStripButtonSaveLocal.Enabled = false;
            toolStripButtonSinc.Enabled = false;
            toolStripStatusLabel1.Text = "Aguarde p.f.";
            toolStripProgressBar1.Visible = true;
            toolStripButtonCancelar.Enabled = true;
        }
        private void handleTaskException(Exception ex)
        {
            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel1.Text = "";
            if (ex is GoogleContactsException)
            {
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (ex is OperationCanceledException)
            {
                MessageBox.Show(this, "A operação foi cancelada.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void taskIsCompleted()
        {
            toolStripButtonCancelar.Enabled = false;
            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel1.Text = "";
            toolStripButtonPasta.Enabled = true;
            toolStripButtonConta.Enabled = true;
            toolStripButtonSave.Enabled = (getOneRowSelected() != null && getOneRowSelected().TemAlteracoes);
            toolStripButtonSaveLocal.Enabled = (getOneRowSelected() != null);
            toolStripButtonSinc.Enabled = (getTwoRowsSelected() != null);
            googleContactsDocListBindingSource.RaiseListChangedEvents = true;
            googleContactsDocListBindingSource.ResetBindings(false);
        }
        private void showDocDetails(GoogleContactsDoc doc)
        {
            try
            {
                Form1 f1 = new Form1(doc);
                f1.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
