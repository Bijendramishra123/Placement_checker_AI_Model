using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace YourProjectName.Services
{
    public class OnnxPredictionService
    {
        private readonly InferenceSession _session;

        public OnnxPredictionService()
        {
            // Models folder me rakha hua file load karenge
            _session = new InferenceSession("Models/placement_model.onnx");
        }

        public float Predict(float cgpa, float iq)
        {
            // Input ko tensor me convert karna
            var input = new DenseTensor<float>(new[] { cgpa, iq }, new[] { 1, 2 });
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", input)
            };

            // Prediction run karna
            using var results = _session.Run(inputs);
            var output = results.First().AsEnumerable<float>().First();

            return output; // 0 ya 1 return karega (placement ke liye)
        }
    }
}
