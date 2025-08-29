using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using PlacementAPI.Models;

namespace PlacementAPI.Services
{
    public class OnnxService
    {
        private readonly InferenceSession _session;

        public OnnxService()
        {
            _session = new InferenceSession("Models/placement_model.onnx");

            // Optional: Log available outputs for debugging
            Console.WriteLine("ONNX Model Outputs:");
            foreach (var output in _session.OutputMetadata)
            {
                Console.WriteLine($"- {output.Key}");
            }
        }

        public (long label, float[] probability) Predict(PlacementInput input)
        {
            var inputTensor = new DenseTensor<float>(
                new[] { input.CGPA, input.IQ },
                new[] { 1, 2 }
            );

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", inputTensor)
            };

            using var results = _session.Run(inputs);

            // Extract outputs safely
            long label = -1;
            float[] probability = new float[] { };
            var labelResult = results.FirstOrDefault(x => x.Name == "label");
            var probResult = results.FirstOrDefault(x => x.Name == "probabilities");

            if (labelResult?.Value is Tensor<long> labelTensor)
            {
                label = labelTensor.FirstOrDefault();
            }

            if (probResult?.Value is Tensor<float> probFloatTensor)
            {
                probability = probFloatTensor.ToArray();
            }
            else if (probResult?.Value is Tensor<double> probDoubleTensor)
            {
                probability = probDoubleTensor.Select(x => (float)x).ToArray();
            }

            return (label, probability);
        }
    }
}