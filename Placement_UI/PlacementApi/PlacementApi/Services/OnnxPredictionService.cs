using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace YourProjectName.Services
{
    // This service is responsible for running ONNX model prediction
    public class OnnxPredictionService
    {
        // InferenceSession is used to load and execute the ONNX model
        private readonly InferenceSession _session;

        // Constructor loads the ONNX model from the Models folder
        public OnnxPredictionService()
        {
            _session = new InferenceSession("Models/placement_model.onnx");
        }

        // This method takes CGPA and IQ as input and returns prediction result
        public float Predict(float cgpa, float iq)
        {
            // Convert input values into a tensor of shape [1, 2]
            var input = new DenseTensor<float>(new[] { cgpa, iq }, new[] { 1, 2 });

            // Create ONNX input with name "input"
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", input)
            };

            // Run the model with given input
            using var results = _session.Run(inputs);

            // Extract the first output value as float
            var output = results.First().AsEnumerable<float>().First();

            // Return prediction (0 or 1)
            return output;
        }
    }
}