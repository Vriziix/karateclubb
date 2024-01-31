using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace karateclubb
{
    public partial class InscriptionForm : Form
    {
        private ComboBox clubComboBox = new ComboBox();
        private PlaceholderTextBox nomTextBox = new PlaceholderTextBox();
        private PlaceholderTextBox prenomTextBox = new PlaceholderTextBox();
        private PlaceholderTextBox rueTextBox = new PlaceholderTextBox();
        private PlaceholderTextBox codePostalTextBox = new PlaceholderTextBox();
        private DateTimePicker dateNaissancePicker = new DateTimePicker();
        private PlaceholderTextBox villeNaissanceTextBox = new PlaceholderTextBox();
        private PlaceholderTextBox numLicenseTextBox = new PlaceholderTextBox();
        private Button ajouterButton = new Button();
        private Button fermerButton = new Button();

        private Bdd bdd = new Bdd();

        public InscriptionForm()
        {
            InitializeComponent();

            this.BackColor = Color.MintCream;
            this.Size = new Size(400, 350);
            this.Text = "GESTION DES MEMBRES [AJOUTER]";

            ConfigurerComboBox(clubComboBox, "Club", 50, 40);
            ConfigurerTextBox(nomTextBox, "Nom", 50, 70);
            ConfigurerTextBox(prenomTextBox, "Prenom", 50, 100);
            ConfigurerTextBox(rueTextBox, "Rue", 50, 130);
            ConfigurerTextBox(codePostalTextBox, "Code Postal", 50, 160);

            dateNaissancePicker.Format = DateTimePickerFormat.Short;
            dateNaissancePicker.Location = new Point(50, 190);
            this.Controls.Add(dateNaissancePicker);

            ConfigurerTextBox(villeNaissanceTextBox, "Ville Naissance", 50, 220);
            ConfigurerTextBox(numLicenseTextBox, "Numéro de licence", 50, 250);

            ConfigurerButton(ajouterButton, "AJOUTER", 50, 290);
            ajouterButton.Click += AjouterButton_Click;

            ConfigurerButton(fermerButton, "FERMER", 150, 290);
            fermerButton.Click += (sender, e) => this.Close();

            this.Controls.Add(clubComboBox);
            this.Controls.Add(nomTextBox);
            this.Controls.Add(prenomTextBox);
            this.Controls.Add(rueTextBox);
            this.Controls.Add(codePostalTextBox);
            this.Controls.Add(villeNaissanceTextBox);
            this.Controls.Add(numLicenseTextBox);
            this.Controls.Add(ajouterButton);
            this.Controls.Add(fermerButton);

            bdd.FillComboBoxWithClubs(clubComboBox);
        }

        private void ConfigurerComboBox(ComboBox comboBox, string placeholder, int x, int y)
        {
            comboBox.Size = new Size(200, 20);
            comboBox.Location = new Point(x, y);
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.DisplayMember = "NomClub";
        }

        private void ConfigurerTextBox(PlaceholderTextBox textBox, string placeholder, int x, int y)
        {
            textBox.Size = new Size(200, 20);
            textBox.Location = new Point(x, y);
            textBox.PlaceholderText = placeholder;
        }

        private void ConfigurerButton(Button button, string texte, int x, int y)
        {
            button.Text = texte;
            button.Size = new Size(80, 30);
            button.Location = new Point(x, y);
            button.BackColor = Color.LightSkyBlue;
            button.FlatStyle = FlatStyle.Flat;
        }

        private void AjouterButton_Click(object sender, EventArgs e)
        {
            if (clubComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(nomTextBox.Text) ||
                string.IsNullOrWhiteSpace(prenomTextBox.Text) ||
                string.IsNullOrWhiteSpace(rueTextBox.Text) ||
                string.IsNullOrWhiteSpace(codePostalTextBox.Text) ||
                string.IsNullOrWhiteSpace(villeNaissanceTextBox.Text) ||
                string.IsNullOrWhiteSpace(numLicenseTextBox.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs et sélectionner un club.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Club clubSelectionne = clubComboBox.SelectedItem as Club;
            if (clubSelectionne != null)
            {
                bool success = bdd.AddMember(
                    numLicenseTextBox.Text,
                    clubSelectionne,
                    nomTextBox.Text,
                    prenomTextBox.Text,
                    dateNaissancePicker.Value,
                    rueTextBox.Text,
                    codePostalTextBox.Text,
                    villeNaissanceTextBox.Text
                );

                if (success)
                {
                    MessageBox.Show("Membre ajouté avec succès.");
                }
                else
                {
                    MessageBox.Show("L'ajout du membre a échoué.");
                }
            }
        }
    }
}
