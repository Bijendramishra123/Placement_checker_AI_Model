using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using PlacementAPI.Models;

namespace PlacementAPI.Services
{
    // This service handles loading the ONNX model and performing predictions
    public class OnnxService
    {
        // InferenceSession is used to load and run the ONNX model
        private readonly InferenceSession _session;

        // Constructor initializes the ONNX model
        public OnnxService()
        {
            // Load the ONNX model from the Models folder
            _session = new InferenceSession("Models/placement_model.onnx");

            // Logging available output names (useful for debugging)
            Console.WriteLine("ONNX Model Outputs:");
            foreach (var output in _session.OutputMetadata)
            {
                Console.WriteLine($"- {output.Key}");
            }
        }

        // This method performs prediction using the ONNX model
        public (long label, float[] probability) Predict(PlacementInput input)
        {
            // Create input tensor with CGPA and IQ
            // Shape [1, 2] means 1 row and 2 features
            var inputTensor = new DenseTensor<float>(
                new[] { input.CGPA, input.IQ },
                new[] { 1, 2 }
            );

            // Create input list for ONNX model
            // "input" must match the model's input name
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", inputTensor)
            };

            // Run the model with given inputs
            using var results = _session.Run(inputs);

            // Initialize default values
            long label = -1;
            float[] probability = new float[] { };

            // Extract label output
            var labelResult = results.FirstOrDefault(x => x.Name == "label");

            // Extract probability output
            var probResult = results.FirstOrDefault(x => x.Name == "probabilities");

            // Convert label tensor to long value
            if (labelResult?.Value is Tensor<long> labelTensor)
            {
                label = labelTensor.FirstOrDefault();
            }

            // Convert probability tensor (float)
            if (probResult?.Value is Tensor<float> probFloatTensor)
            {
                probability = probFloatTensor.ToArray();
            }
            // Handle case where model returns double instead of float
            else if (probResult?.Value is Tensor<double> probDoubleTensor)
            {
                probability = probDoubleTensor.Select(x => (float)x).ToArray();
            }

            // Return prediction result
            return (label, probability);
        }
    }
}