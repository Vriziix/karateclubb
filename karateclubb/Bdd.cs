using System;
using System.Data;
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

    public DataTable GetCompetitionsDataTable()
    {
        using (var connection = OpenConnection())
        {
            if (connection != null)
            {
                var command = new MySqlCommand(
                    "SELECT c.num_competition, c.date_competition, cl.nom_club " +
                    "FROM competition c " +
                    "JOIN club cl ON c.num_club = cl.num_club", connection);
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


    public bool DeleteMembre(int numLicence)
    {
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
                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("La suppression du membre a échoué.", "Erreur de Suppression", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
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

}
