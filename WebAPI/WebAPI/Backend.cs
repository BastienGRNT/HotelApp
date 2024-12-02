using System.Data.SQLite;

namespace WebAPI;

public class Backend
{
    public class Sql
    {
        public static void CreateSql()
        {
            string dbPath = @"C:\Users\greno\RiderProjects\WebAPI\File\data.db";

            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
                Console.WriteLine("Base de données SQLite créée avec succès !");
            }
            else
            {
                Console.WriteLine("La base de données existe déjà.");
            }
        }

        public static SQLiteConnection ConnectSql()
        {
            var connection =
                new SQLiteConnection(
                    "Data Source=C:\\Users\\greno\\RiderProjects\\WebAPI\\File\\data.db;Version=3;");
            connection.Open();
            return connection;
        }
    }
    
    public class SqlTable
    {
        public static void CreateSqlTableChambre()
        {
            using (var connection = Sql.ConnectSql())
            {
                string createSqlTableChambre =
                    @"CREATE TABLE IF NOT EXISTS Chambres (Numero INTEGER PRIMARY KEY, Type TEXT NOT NULL);";

                using (var command = new SQLiteCommand(createSqlTableChambre, connection))
                {
                    command.ExecuteNonQuery();
                }

                Console.WriteLine("Table Chambres créée avec succès !");
            }
        }

        public static void CreateSqlTableReservation()
        {
            using (var connection = Sql.ConnectSql())
            {
                string CreateSqlTableReservation =
                    @"CREATE TABLE IF NOT EXISTS Reservations (NumeroReservation INTEGER PRIMARY KEY AUTOINCREMENT, Nom TEXT NOT NULL, Prenom TEXT NOT NULL, DateArriver DATETIME NOT NULL, DateDepart DATETIME NOT NULL, ChambreClient INTEGER NOT NULL);";

                using (var command = new SQLiteCommand(CreateSqlTableReservation, connection))
                {
                    command.ExecuteNonQuery();
                }

                Console.WriteLine("Table Reservation créée avec succès !");
            }
        }
    }
    
    public class AjouterData
    {
        public void AjouterChambre(Chambres chambres)
    {
        using (var connection = Sql.ConnectSql())
        {
            SqlTable.CreateSqlTableChambre();

            string AjouterUneChambre = @"INSERT OR REPLACE INTO Chambres (Numero, Type) VALUES (@Numero, @Type);";
            
            using (var command = new SQLiteCommand(AjouterUneChambre, connection))
            {
                command.Parameters.AddWithValue("@Numero", chambres.numero_chambre);
                command.Parameters.AddWithValue("@Type", chambres.type_chambre);
                command.ExecuteNonQuery();
            }
            Console.WriteLine($"Chambre Numéro: {chambres.numero_chambre} et type: {chambres.type_chambre} ajoutée avec succès !!!");
        }
    }
    
        public string AjouterReservation(Reservation reservation)
        {
            try
            {
                // Vérifier si la réservation est valide
                string erreur = GestionReservation.ChambreCorrect(
                    reservation.date_arriver, 
                    reservation.date_depart, 
                    reservation.chambre_client);

                if (!string.IsNullOrEmpty(erreur))
                {
                    // Retourner l'erreur si la validation échoue
                    return erreur;
                }

                // Si tout est valide, ajouter la réservation dans la base de données
                SqlTable.CreateSqlTableReservation();
                using (var connection = Sql.ConnectSql())
                {
                    string AjouterReservationSql =
                        @"INSERT INTO Reservations (Nom, Prenom, DateArriver, DateDepart, ChambreClient) 
                  VALUES (@Nom, @Prenom, @DateArriver, @DateDepart, @ChambreClient);";

                    using (var command = new SQLiteCommand(AjouterReservationSql, connection))
                    {
                        command.Parameters.AddWithValue("@Nom", reservation.nom);
                        command.Parameters.AddWithValue("@Prenom", reservation.prenom);
                        command.Parameters.AddWithValue("@DateArriver", reservation.date_arriver);
                        command.Parameters.AddWithValue("@DateDepart", reservation.date_depart);
                        command.Parameters.AddWithValue("@ChambreClient", reservation.chambre_client);

                        command.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("Réservation ajoutée avec succès !");
                return "Réservation ajoutée avec succès !";
            }
            catch (Exception ex)
            {
                // Gérer les exceptions de manière appropriée et renvoyer un message explicite
                Console.WriteLine($"Erreur lors de l'ajout de la réservation : {ex.Message}");
                return $"Erreur interne : {ex.Message}";
            }
        }
    }
    
    public class GestionReservation
    {
        public static List<int> ChambresDispnibles(DateTime dateDebut, DateTime dateFin)
        {
            using (var connection = Sql.ConnectSql())
            {
                string query = @"SELECT Numero FROM Chambres WHERE Numero NOT IN (SELECT ChambreClient FROM Reservations WHERE (@DateFin >= DateDepart AND @DateDebut <= DateArriver));";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DateDebut", dateDebut);
                    command.Parameters.AddWithValue("@DateFin", dateFin);

                    using (var reader = command.ExecuteReader())
                    {
                        var chambresDisponibles = new List<int>();
                        while (reader.Read())
                        {
                            chambresDisponibles.Add(reader.GetInt32(0));
                        }
                        return chambresDisponibles;
                    }
                }
            }
        }

        public static string ChambreCorrect(DateTime DateArriver, DateTime DateDepart, int ChambreClient)
        {
            List<int> ChambreDisponnible = GestionReservation.ChambresDispnibles(DateArriver, DateDepart);

            if (ChambreDisponnible.Count == 0)
            {
                return "Il n'y à pas de chambres disponnible";
            }

            if (!ChambreDisponnible.Contains(ChambreClient))
            {
                return $"Sélectionnez une chambre disponible : {string.Join(", ", ChambreDisponnible)}";
            }

            if (DateArriver > DateDepart)
            {
                return "Merci de selectionner une date de départ > à la date d'arriver";
            }

            return null;
            
        } 
    }

}