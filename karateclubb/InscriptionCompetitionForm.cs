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

        private Bdd bdd = new Bdd();

        public InscriptionCompetitionForm()
        {
            InitializeComponent();

            this.BackColor = Color.MintCream;
            this.Size = new Size(800, 600);
            this.Text = "INSCRIPTION À UNE COMPÉTITION";

            ConfigurerDataGridView(membresDataGridView, 50, 40);
            ConfigurerDataGridView(competitionsDataGridView, 450, 40);

            ConfigurerButton(inscrireButton, "INSCRIRE", 50, 520);
            inscrireButton.Click += InscrireButton_Click;

            ConfigurerButton(nouveauMembreButton, "NOUVEAU MEMBRE", 250, 520);
            nouveauMembreButton.Click += NouveauMembreButton_Click;

            ConfigurerButton(annulerButton, "ANNULER", 450, 520);
            annulerButton.Click += (sender, e) => this.Close();

            this.Controls.Add(membresDataGridView);
            this.Controls.Add(competitionsDataGridView);
            this.Controls.Add(inscrireButton);
            this.Controls.Add(nouveauMembreButton);
            this.Controls.Add(annulerButton);

            bdd.FillDataGridViewWithMembres(membresDataGridView);
            bdd.FillDataGridViewWithCompetitions(competitionsDataGridView);
        }

        private void ConfigurerDataGridView(DataGridView dataGridView, int x, int y)
        {
            dataGridView.Location = new Point(x, y);
            dataGridView.Size = new Size(300, 400);
        }

        private void ConfigurerButton(Button button, string texte, int x, int y)
        {
            button.Text = texte;
            button.Size = new Size(150, 40);
            button.Location = new Point(x, y);
            button.BackColor = Color.LightSkyBlue;
            button.FlatStyle = FlatStyle.Flat;
        }

        private void InscrireButton_Click(object sender, EventArgs e)
        {
            
        }

        private void NouveauMembreButton_Click(object sender, EventArgs e)
        {
            InscriptionForm inscriptionForm = new InscriptionForm();
            inscriptionForm.ShowDialog();
        }
    }
}
