using Microsoft.AspNetCore.Mvc;
using PlacementAPI.Models;
using PlacementAPI.Services;

namespace PlacementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlacementController : ControllerBase
    {
        private readonly OnnxService _onnxService;

        public PlacementController()
        {
            _onnxService = new OnnxService();
        }

        [HttpPost("predict")]
        public IActionResult Predict([FromBody] PlacementInput input)
        {
            try
            {
                var (label, probability) = _onnxService.Predict(input);

                return Ok(new
                {
                    PredictedLabel = label,
                    Probabilities = probability
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Prediction failed.",
                    ExceptionMessage = ex.Message
                });
            }
        }
    }
}