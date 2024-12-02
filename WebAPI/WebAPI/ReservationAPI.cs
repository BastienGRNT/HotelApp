using Microsoft.AspNetCore.Mvc;
using WebAPI;

namespace WebAPI;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    [HttpPost]
    public IActionResult AjouterReservation([FromBody] Reservation reservation)
    {
        if (reservation == null)
        {
            return BadRequest("Données de réservation manquantes.");
        }

        var ajouterData = new Backend.AjouterData();
        string result = ajouterData.AjouterReservation(reservation);

        if (result.StartsWith("Réservation ajoutée avec succès"))
        {
            return Ok(result);
        }
        else
        {
            return BadRequest(result);
        }
    }
}