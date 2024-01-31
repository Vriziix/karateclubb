using System;
using System.Windows.Forms;
using karateclubb;
using MySql.Data.MySqlClient;

public class Bdd
{
    private string connectionString = "Server=localhost;Database=karate;Uid=root;Pwd=;";

    public Bdd()
    {
        
    }

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
}
