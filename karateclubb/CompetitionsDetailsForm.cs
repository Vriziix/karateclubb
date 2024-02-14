using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace karateclubb
{
    public partial class CompetitionsDetailsForm : Form
    {
        private ComboBox competitionsComboBox = new ComboBox();
        private DataGridView membresDataGridView = new DataGridView();
        private Bdd bdd = new Bdd();

        public CompetitionsDetailsForm()
        {
            InitializeComponent();
            InitializeForm();
            ConfigurerComboBox();
            ConfigurerDataGridView();
            LoadCompetitions();
        }

        private void InitializeForm()
        {
            this.Size = new Size(800, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Détails des compétitions";
            LoadCompetitions();
            competitionsComboBox.SelectedIndexChanged += CompetitionsComboBox_SelectedIndexChanged;
        }

        private void ConfigurerComboBox()
        {
            competitionsComboBox.Location = new Point(10, 10);
            competitionsComboBox.Size = new Size(300, 30);
            competitionsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            competitionsComboBox.SelectedIndexChanged += CompetitionsComboBox_SelectedIndexChanged;
            this.Controls.Add(competitionsComboBox);
        }

        private void ConfigurerDataGridView()
        {
            membresDataGridView.Location = new Point(10, 50);
            membresDataGridView.Size = new Size(780, 300);
            membresDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.Controls.Add(membresDataGridView);
        }

        private void LoadCompetitions()
        {
            DataTable competitions = bdd.GetCompetitionsDataTable();
            competitionsComboBox.DataSource = competitions;
            competitionsComboBox.DisplayMember = "nom_club";
            competitionsComboBox.ValueMember = "num_competition";
            competitionsComboBox.SelectedIndex = -1;
        }

        private void CompetitionsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (competitionsComboBox.SelectedValue != null)
            {
                int numCompetition = Convert.ToInt32(competitionsComboBox.SelectedValue);
                bdd.UpdateNoteGlobale(numCompetition); 
                membresDataGridView.DataSource = bdd.LoadMembresEtNotesParCompetition(numCompetition);
            }
        }


    }
}



