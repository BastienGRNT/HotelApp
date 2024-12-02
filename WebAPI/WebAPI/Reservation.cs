namespace WebAPI;

public class Reservation
{
    public string nom { get; set; }
    public string prenom { get; set; }
    public DateTime date_arriver { get; set; }
    public DateTime date_depart { get; set; }
    public int chambre_client { get; set; }
}