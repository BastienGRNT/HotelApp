using Microsoft.AspNetCore.Mvc;
using WebAPI;

namespace WebAPI;

[ApiController]
[Route("api/[controller]")]
public class ChambresController : ControllerBase
{
    [HttpPost]
    public IActionResult AjouterChambre([FromBody] Chambres chambres)
    {
        if (chambres == null)
        {
            return BadRequest("Données de réservation manquantes.");
        }

        var ajouterData = new Backend.AjouterData();
        string result = ajouterData.AjouterChambre(chambres);

        if (result.StartsWith("Chambre ajoutée avec succès !"))
        {
            return Ok(result);
        }
        else
        {
            return BadRequest(result);
        }
    }
}