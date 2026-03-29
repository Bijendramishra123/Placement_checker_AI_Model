import { useState } from 'react';
import './prediction.css';

function PredictorForm() {
  const [cgpa, setCgpa] = useState('');
  const [iq, setIq] = useState('');
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);

  const BASE_URL = "http://localhost:5242";

  const handleSubmit = async (e) => {
    e.preventDefault();

    // ✅ Step 1: Input validation (IMPORTANT)
    const cgpaValue = parseFloat(cgpa);
    const iqValue = parseFloat(iq);

    if (!cgpa || !iq) {
      setResult({ error: "Please fill all fields" });
      return;
    }

    if (isNaN(cgpaValue) || isNaN(iqValue)) {
      setResult({ error: "Invalid number format" });
      return;
    }

    // Range validation (backend ke rules ke according)
    if (cgpaValue < 0 || cgpaValue > 10) {
      setResult({ error: "CGPA must be between 0 and 10" });
      return;
    }

    if (iqValue < 0 || iqValue > 200) {
      setResult({ error: "IQ must be between 0 and 200" });
      return;
    }

    setLoading(true);
    setResult(null);

    try {
      const response = await fetch(`${BASE_URL}/api/Placement/predict`, {
        method: 'POST',
        headers: { 
          'Content-Type': 'application/json' 
        },
        body: JSON.stringify({
          cgpa: cgpaValue,
          iq: iqValue
        })
      });

      // ✅ Step 2: Handle backend validation errors properly
      if (!response.ok) {
        const errorData = await response.json();
        console.error("Backend error:", errorData);

        setResult({
          error: errorData?.errors 
            ? Object.values(errorData.errors).flat().join(", ")
            : "Backend validation failed"
        });
        return;
      }

      const data = await response.json();
      setResult(data);

    } catch (err) {
      console.error(err);
      setResult({ error: 'Server connection error' });
    } finally {
      setLoading(false);
    }
  };

  const renderMessage = () => {
    if (!result || result.error) return null;

    const label = result.predictedLabel;
    const confidence = (result.probabilities?.[label] * 100).toFixed(2);

    return label === 1 ? (
      <p className="success">🎯 Placement hoga — {confidence}% chance!</p>
    ) : (
      <p className="failure">❌ Placement nahi hoga — {confidence}% chance.</p>
    );
  };

  return (
    <div className="form-container">
      <h2>🎓 Placement Predictor</h2>

      <form onSubmit={handleSubmit}>
        <label htmlFor="cgpa">CGPA</label>
        <input
          id="cgpa"
          type="number"
          step="0.1"
          value={cgpa}
          onChange={(e) => setCgpa(e.target.value)}
          required
        />

        <label htmlFor="iq">IQ</label>
        <input
          id="iq"
          type="number"
          value={iq}
          onChange={(e) => setIq(e.target.value)}
          required
        />

        <button type="submit" disabled={loading}>
          {loading ? 'Predicting...' : 'Predict'}
        </button>
      </form>

      {result && (
        <div className="result-box">
          {result.error ? (
            <p className="error">{result.error}</p>
          ) : (
            renderMessage()
          )}
        </div>
      )}
    </div>
  );
}

export default PredictorForm;