//using Microsoft.AspNetCore.Mvc;

//namespace agency.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ToursApiController : ControllerBase
//    {
//        [HttpGet("Details/{id}")]
//        public IActionResult GetTourDetails(int id)
//        {
//            var tour = tours.FirstOrDefault(t => t.Id == id);
//            if (tour == null)
//            {
//                return NotFound(new { message = $"Тур с ID {id} не найден." });
//            }
//            return Ok(tour);
//        }
//    }
//}
