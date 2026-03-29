using Microsoft.AspNetCore.Mvc;
using PlacementAPI.Models;
using PlacementAPI.Services;

namespace PlacementAPI.Controllers
{
    // This attribute defines the base route for this controller
    // "api/Placement" will be the base URL
    [Route("api/[controller]")]
    
    // Enables automatic model validation and API-specific behaviors
    [ApiController]
    
    // ControllerBase is used for API controllers (no views)
    public class PlacementController : ControllerBase
    {
        // Service object used to perform ONNX model prediction
        private readonly OnnxService _onnxService;

        // Constructor where service is initialized
        // Currently using manual object creation (can be improved using Dependency Injection)
        public PlacementController()
        {
            _onnxService = new OnnxService();
        }

        // This method handles HTTP POST requests at route: api/Placement/predict
        [HttpPost("predict")]
        public IActionResult Predict([FromBody] PlacementInput input)
        {
            // Check if incoming data is valid according to model validation rules
            if (!ModelState.IsValid)
            {
                // If validation fails, return 400 Bad Request with error details
                return BadRequest(ModelState);
            }

            try
            {
                // Call the ONNX service to get prediction result
                // It returns label (0 or 1) and probability values
                var (label, probability) = _onnxService.Predict(input);

                // Return successful response with prediction result
                return Ok(new
                {
                    PredictedLabel = label,
                    Probabilities = probability
                });
            }
            catch (Exception ex)
            {
                // Handle any runtime errors and return 500 Internal Server Error
                return StatusCode(500, new
                {
                    Error = "Prediction failed.",
                    ExceptionMessage = ex.Message
                });
            }
        }
    }
}