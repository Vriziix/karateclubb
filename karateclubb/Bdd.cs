using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using karateclubb;
using MySql.Data.MySqlClient;

public class Bdd
{
    private string connectionString = "Server=localhost;Database=karate;Uid=root;Pwd=;";

    public MySqlConnection OpenConnection()
    {
        try
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            return connection;
        }
        catch (MySqlException ex)
        {
            MessageBox.Show($"Failed to connect to the database: {ex.Message}", "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return null;
        }
    }

    public void CloseConnection(MySqlConnection connection)
    {
        try
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }
        catch (MySqlException ex)
        {
            MessageBox.Show($"Failed to close the database connection: {ex.Message}", "Database Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public void FillComboBoxWithClubs(ComboBox comboBox)
    {
        using (MySqlConnection connection = OpenConnection())
        {
            if (connection == null) return;

            string query = "SELECT num_club, nom_club FROM club";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int numClub = reader.GetInt32("num_club");
                        string nomClub = reader.GetString("nom_club");
                        comboBox.Items.Add(new Club(numClub, nomClub));
                    }
                }
            }
            CloseConnection(connection);
        }
    }

    public bool AddMember(string numLicense, Club club, string nom, string prenom, DateTime dateNaissance, string rue, string codePostal, string villeNaissance)
    {
        using (MySqlConnection connection = OpenConnection())
        {
            if (connection == null) return false;

            string query = @"INSERT INTO membre (num_licence, num_club, nom_membre, prenom_membre, date_naissance, adr_rue_membre, code_post_membre, adr_ville_membre)
                             VALUES (@NumLicense, @NumClub, @Nom, @Prenom, @DateNaissance, @Rue, @CodePostal, @VilleNaissance)";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NumLicense", numLicense);
                command.Parameters.AddWithValue("@NumClub", club.NumClub);
                command.Parameters.AddWithValue("@Nom", nom);
                command.Parameters.AddWithValue("@Prenom", prenom);
                command.Parameters.AddWithValue("@DateNaissance", dateNaissance);
                command.Parameters.AddWithValue("@Rue", rue);
                command.Parameters.AddWithValue("@CodePostal", codePostal);
                command.Parameters.AddWithValue("@VilleNaissance", villeNaissance);

                try
                {
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
    }

    public DataTable GetMembresDataTable()
    {
        using (var connection = OpenConnection())
        {
            if (connection != null)
            {
                var command = new MySqlCommand("SELECT num_licence, nom_membre, prenom_membre FROM membre", connection);
                var adapter = new MySqlDataAdapter(command);
                var dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
            return null;
        }
    }

    public void FillDataGridViewWithMembres(DataGridView dataGridView)
    {
        using (var connection = OpenConnection())
        {
            if (connection != null)
            {
                var command = new MySqlCommand("SELECT num_licence, nom_membre, prenom_membre FROM membre", connection);
                var adapter = new MySqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView.DataSource = dataTable;

                dataGridView.Columns["num_licence"].HeaderText = "Num Licence";
                dataGridView.Columns["nom_membre"].HeaderText = "Nom";
                dataGridView.Columns["prenom_membre"].HeaderText = "Prénom";
            }
        }
    }

    public void FillDataGridViewWithCompetitions(DataGridView dataGridView)
    {
        using (var connection = OpenConnection())
        {
            if (connection != null)
            {
                var command = new MySqlCommand("SELECT num_competition, nom FROM competition", connection);
                var adapter = new MySqlDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView.DataSource = dataTable;

                dataGridView.Columns["num_competition"].HeaderText = "Numéro de la compétition";
                dataGridView.Columns["date_competition"].HeaderText = "Date de la compétition";
                dataGridView.Columns["nom_club"].HeaderText = "Nom du club organisateur";

            }
        }
    }

    public bool AddInscription(int numCompetition, int numLicence)
    {
        using (MySqlConnection connection = OpenConnection())
        {
            if (connection == null) return false;

            string query = @"INSERT INTO inscription (num_competition, num_licence) VALUES (@NumCompetition, @NumLicence)";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NumCompetition", numCompetition);
                command.Parameters.AddWithValue("@NumLicence", numLicence);

                try
                {
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Une erreur est survenue : {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
    }

    public bool UpdateMembre(int numLicence, string nomMembre, string prenomMembre)
    {
        using (var connection = OpenConnection())
        {
            if (connection == null) return false;

            string query = "UPDATE membre SET nom_membre = @NomMembre, prenom_membre = @PrenomMembre WHERE num_licence = @NumLicence";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NomMembre", nomMembre);
                command.Parameters.AddWithValue("@PrenomMembre", prenomMembre);
                command.Parameters.AddWithValue("@NumLicence", numLicence);

                try
                {
                    int result = command.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("La mise à jour du membre a échoué.", "Erreur de Mise à Jour", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Une erreur s'est produite lors de la mise à jour du membre: {ex.Message}", "Erreur de Mise à Jour", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
    }

    public bool DeleteInscriptionsByMembre(int numLicence)
    {
        using (var connection = OpenConnection())
        {
            if (connection == null) return false;

            string query = "DELETE FROM inscription WHERE num_licence = @NumLicence";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NumLicence", numLicence);

                try
                {
                    int result = command.ExecuteNonQuery();
                    return result >= 0;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Une erreur s'est produite lors de la suppression des inscriptions: {ex.Message}", "Erreur de Suppression", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
    }



    public bool DeleteMembre(int numLicence)
    {
        if (!DeleteInscriptionsByMembre(numLicence))
        {
            return false; 
        }

        using (var connection = OpenConnection())
        {
            if (connection == null) return false;

            string query = "DELETE FROM membre WHERE num_licence = @NumLicence";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NumLicence", numLicence);

                try
                {
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Une erreur s'est produite lors de la suppression du membre: {ex.Message}", "Erreur de Suppression", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
    }



    public bool UpdateCompetition(int numCompetition, string nomCompetition, DateTime dateCompetition, int numClub)
    {
        using (var connection = OpenConnection())
        {
            if (connection == null) return false;

            string query = "UPDATE competition SET nom_competition = @NomCompetition, date_competition = @DateCompetition WHERE num_competition = @NumCompetition";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NomCompetition", nomCompetition);
                command.Parameters.AddWithValue("@DateCompetition", dateCompetition);
                command.Parameters.AddWithValue("@NumCompetition", numCompetition);

                try
                {
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Une erreur s'est produite lors de la mise à jour de la compétition: {ex.Message}", "Erreur de Mise à Jour", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
    }

    public bool DeleteCompetition(int numCompetition)
    {
        using (var connection = OpenConnection())
        {
            if (connection == null) return false;

            string query = "DELETE FROM competition WHERE num_competition = @NumCompetition";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NumCompetition", numCompetition);

                try
                {
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show($"Une erreur s'est produite lors de la suppression de la compétition: {ex.Message}", "Erreur de Suppression", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
    }

    public DataTable GetCompetitionsDataTable()
    {
        DataTable competitions = new DataTable();
        using (var connection = OpenConnection())
        {
            string query = @"
            SELECT c.num_competition, cl.nom_club 
            FROM competition c
            JOIN club cl ON c.num_club = cl.num_club";
            using (var adapter = new MySqlDataAdapter(query, connection))
            {
                adapter.Fill(competitions);
            }
        }
        return competitions;
    }

    public DataTable LoadMembresEtNotesParCompetition(int numCompetition)
    {
        DataTable membresNotes = new DataTable();
        using (var connection = OpenConnection())
        {
            string query = @"
            SELECT i.num_licence, m.nom_membre, m.prenom_membre, 
            (SELECT GROUP_CONCAT(n.note ORDER BY n.note SEPARATOR ',') FROM note n WHERE n.num_licence = i.num_licence AND n.num_competition = i.num_competition) AS notes
            FROM inscription i
            JOIN membre m ON i.num_licence = m.num_licence
            WHERE i.num_competition = @NumCompetition";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NumCompetition", numCompetition);
                using (var adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(membresNotes);
                }
            }
        }

        if (!membresNotes.Columns.Contains("note_globale"))
        {
            membresNotes.Columns.Add("note_globale", typeof(double));
        }

        foreach (DataRow row in membresNotes.Rows)
        {

            var notesStringArray = row["notes"].ToString().Split(',');
            var notes = new List<float>();

            foreach (var noteString in notesStringArray)
            {
                if (int.TryParse(noteString, out int note))
                {
                    notes.Add(note);
                }
            }

            notes.Sort();
            if (notes.Count > 2)
            {
                notes.RemoveAt(0);
                notes.RemoveAt(notes.Count - 1);
            }

            double average = notes.Any() ? notes.Average() : 0;

            Console.WriteLine($"Nombre de notes après traitement : {notes.Count}");
            if (notes.Any()) Console.WriteLine($"Moyenne calculée : {notes.Average()}");

            row["note_globale"] = average;

            int numLicence = Convert.ToInt32(row["num_licence"]);
            UpdateNoteGlobaleInDb(numLicence, numCompetition, average);
        }

        return membresNotes;
    }

    public DataTable LoadMembresParCompetition(int numCompetition)
    {
        DataTable membres = new DataTable();
        using (var connection = OpenConnection())
        {
            string query = @"
            SELECT m.nom_membre, m.prenom_membre, c.nom_club, n.note
            FROM inscription i
            JOIN membre m ON i.num_licence = m.num_licence
            JOIN club c ON m.num_club = c.num_club
            JOIN note n ON m.num_licence = n.num_licence
            WHERE i.num_competition = @NumCompetition";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NumCompetition", numCompetition);
                using (var adapter = new MySqlDataAdapter(command))
                {
                    adapter.Fill(membres);
                }
            }
        }
        return membres;
    }

    public void UpdateNoteGlobale(int numCompetition)
    {
        DataTable membresNotes = LoadMembresEtNotesParCompetition(numCompetition);

        foreach (DataRow row in membresNotes.Rows)
        {
            List<double> notes = new List<double>();

            for (int i = 1; i <= 5; i++)
            {
                string noteColumnName = $"note{i}";
                if (membresNotes.Columns.Contains(noteColumnName) && row[noteColumnName] != DBNull.Value)
                {
                    notes.Add(Convert.ToDouble(row[noteColumnName]));
                }
            }

            notes.Sort();
            if (notes.Count > 2)
            {
                notes.RemoveAt(0);
                notes.RemoveAt(notes.Count - 1); 
            }

            double average = notes.Any() ? notes.Average() : 0;

            int numLicence = Convert.ToInt32(row["num_licence"]);
            UpdateNoteGlobaleInDb(numLicence, numCompetition, average);
        }
    }

  


    private void UpdateNoteGlobaleInDb(int numLicence, int numCompetition, double noteGlobale)
    {
        using (var connection = OpenConnection())
        {
            string query = @"
            UPDATE inscription
            SET note_globale = @NoteGlobale
            WHERE num_licence = @NumLicence AND num_competition = @NumCompetition";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NoteGlobale", noteGlobale);
                command.Parameters.AddWithValue("@NumLicence", numLicence);
                command.Parameters.AddWithValue("@NumCompetition", numCompetition);
                command.ExecuteNonQuery();
            }
        }
    }

}
