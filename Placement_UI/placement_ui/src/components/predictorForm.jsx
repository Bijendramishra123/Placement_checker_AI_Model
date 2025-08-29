import { useState } from 'react';
import './prediction.css';

function PredictorForm() {
  const [cgpa, setCgpa] = useState('');
  const [iq, setIq] = useState('');
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setResult(null);

    try {
      const response = await fetch('https://localhost:7111/api/Placement/predict', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          cgpa: parseFloat(cgpa),
          iq: parseFloat(iq)
        })
      });

      const data = await response.json();
      setResult(data);
    } catch (err) {
      setResult({ error: 'Prediction failed. Check backend.' });
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