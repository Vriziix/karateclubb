using System;
using System.Drawing;
using System.Windows.Forms;

namespace karateclubb
{
    public partial class InscriptionCompetitionForm : Form
    {
        private DataGridView membresDataGridView = new DataGridView();
        private DataGridView competitionsDataGridView = new DataGridView();
        private Button inscrireButton = new Button();
        private Button nouveauMembreButton = new Button();
        private Button annulerButton = new Button();
        Button refreshButton = new Button();

        private Bdd bdd = new Bdd();

        public InscriptionCompetitionForm()
        {
            InitializeComponent();

            membresDataGridView.CellClick += DataGridView_CellClick;
            competitionsDataGridView.CellClick += DataGridView_CellClick;
            membresDataGridView.CellValueChanged += DataGridView_CellValueChanged;
            competitionsDataGridView.CellValueChanged += DataGridView_CellValueChanged;


            refreshButton.Text = "Rafraîchir";
            refreshButton.Size = new Size(100, 30);
            refreshButton.Location = new Point(650, 500);
            refreshButton.BackColor = Color.LightSkyBlue;
            refreshButton.Click += RefreshButton_Click;
            this.Controls.Add(refreshButton);

            this.BackColor = Color.MintCream;
            this.Size = new Size(800, 600);
            this.Text = "INSCRIPTION À UNE COMPÉTITION";

            ConfigurerDataGridView(membresDataGridView, 50, 40, 300, 200);
            ConfigurerDataGridView(competitionsDataGridView, 400, 40, 300, 200);

            ConfigurerButton(inscrireButton, "INSCRIRE", 50, 250);
            inscrireButton.Click += InscrireButton_Click;

            ConfigurerButton(nouveauMembreButton, "NOUVEAU MEMBRE", 150, 250);
            nouveauMembreButton.Click += NouveauMembreButton_Click;

            ConfigurerButton(annulerButton, "ANNULER", 250, 250);
            annulerButton.Click += (sender, e) => this.Close();

            this.Controls.Add(membresDataGridView);
            this.Controls.Add(competitionsDataGridView);
            this.Controls.Add(inscrireButton);
            this.Controls.Add(nouveauMembreButton);
            this.Controls.Add(annulerButton);

            LoadMembres();
            LoadCompetitions();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadMembres();
            LoadCompetitions();
            MessageBox.Show("Les données ont été rafraîchies.");
        }

        private void LoadMembres()
        {
            membresDataGridView.DataSource = null;
            membresDataGridView.DataSource = bdd.GetMembresDataTable();
        }

        private void LoadCompetitions()
        {
            competitionsDataGridView.DataSource = null;
            competitionsDataGridView.DataSource = bdd.GetCompetitionsDataTable();
        }

        private void ConfigurerDataGridView(DataGridView dataGridView, int x, int y, int width, int height)
        {
            dataGridView.Location = new Point(x, y);
            dataGridView.Size = new Size(width, height);
            dataGridView.ReadOnly = false;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            if (dataGridView.Columns["deleteColumn"] == null)
            {
                var deleteButtonColumn = new DataGridViewButtonColumn { Name = "deleteColumn", Text = "Supprimer", UseColumnTextForButtonValue = true, HeaderText = "Actions" };
                dataGridView.Columns.Add(deleteButtonColumn);

            }
        }

        private void ConfigurerButton(Button button, string texte, int x, int y)
        {
            button.Text = texte;
            button.Size = new Size(80, 30);
            button.Location = new Point(x, y);
            button.BackColor = Color.LightSkyBlue;
            button.FlatStyle = FlatStyle.Flat;
        }

        private void InscrireButton_Click(object sender, EventArgs e)
        {
            if (membresDataGridView.SelectedRows.Count > 0 && competitionsDataGridView.SelectedRows.Count > 0)
            {
                var selectedMembreRow = membresDataGridView.SelectedRows[0];
                int numLicence = Convert.ToInt32(selectedMembreRow.Cells["num_licence"].Value);

                var selectedCompetitionRow = competitionsDataGridView.SelectedRows[0];
                int numCompetition = Convert.ToInt32(selectedCompetitionRow.Cells["num_competition"].Value);

                bool success = bdd.AddInscription(numCompetition, numLicence);

                if (success)
                {
                    MessageBox.Show("Inscription réussie.");
                }
                else
                {
                    MessageBox.Show("L'inscription a échoué.");
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner un membre et une compétition.");
            }
        }



        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            membresDataGridView.CellValueChanged += DataGridView_CellValueChanged;
            competitionsDataGridView.CellValueChanged += DataGridView_CellValueChanged;
            membresDataGridView.CellClick += DataGridView_CellClick;

            if (e.RowIndex >= 0)
            {
                DataGridView dataGridView = sender as DataGridView;
                if (dataGridView != null)
                {
                    int id = Convert.ToInt32(dataGridView.Rows[e.RowIndex].Cells["num_licence"].Value);
                    string columnName = dataGridView.Columns[e.ColumnIndex].Name;
                    object newValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                    if (dataGridView == membresDataGridView && (columnName == "nom_membre" || columnName == "prenom_membre"))
                    {
                        string nomMembre = dataGridView.Rows[e.RowIndex].Cells["nom_membre"].Value.ToString();
                        string prenomMembre = dataGridView.Rows[e.RowIndex].Cells["prenom_membre"].Value.ToString();
                        bdd.UpdateMembre(id, nomMembre, prenomMembre);
                    }
                    else if (dataGridView == competitionsDataGridView && (columnName == "nom_competition" || columnName == "date_competition"))
                    {
                        string nomCompetition = dataGridView["nom_competition", e.RowIndex].Value.ToString();
                        DateTime dateCompetition = Convert.ToDateTime(dataGridView["date_competition", e.RowIndex].Value);
                        int numClub = Convert.ToInt32(dataGridView["num_club", e.RowIndex].Value);
                        bdd.UpdateCompetition(id, nomCompetition, dateCompetition, numClub);
                    }
                }
            }
        }




        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == membresDataGridView.Columns["deleteColumn"].Index)
            {
                if (MessageBox.Show("Voulez-vous vraiment supprimer cet élément ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DataGridView dataGridView = sender as DataGridView;
                    int id;
                    bool success;

                    if (dataGridView == membresDataGridView)
                    {
                        id = Convert.ToInt32(dataGridView["num_licence", e.RowIndex].Value);
                        success = bdd.DeleteMembre(id);
                    }
                    else if (dataGridView == competitionsDataGridView)
                    {
                        id = Convert.ToInt32(dataGridView["num_competition", e.RowIndex].Value);
                        success = bdd.DeleteCompetition(id);
                    }
                    else
                    {
                        return;
                    }

                    if (success)
                    {
                        dataGridView.Rows.RemoveAt(e.RowIndex);
                    }
                    else
                    {
                        MessageBox.Show("La suppression a échoué.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }




        private void NouveauMembreButton_Click(object sender, EventArgs e)
        {
            InscriptionForm inscriptionForm = new InscriptionForm();
            inscriptionForm.ShowDialog();
        }
    }
}
