using UnityEngine;

public abstract class BaseNeuron
{
    public float Value { get; private set; }
    public float Bias { get; private set; }

    
    private float[] _weights;
    private BaseNeuron[] _inputs;

    
    public BaseNeuron(BaseNeuron[] inputs)
    {
        _inputs = inputs;
        _weights = new float[inputs.Length];
    }


    public float[] GetWeights()
    {
        return _weights;
    }
    
    public void SetWeights(float[] newWeights, float bias)
    {
        for (int i = 0; i < newWeights.Length && i < _weights.Length; i++) {
            _weights[i] = newWeights[i];
        }

        Bias = bias;
    }

    public void SetValue(float newValue)
    {
        Value = newValue;
    }

    public void CalculateValue()
    {
        float weightedValue = 0;
        
        for (int i = 0; i < _weights.Length; i++) {
            weightedValue += _weights[i] * _inputs[i].Value;
        }
        
        Value = ActivationFunction(weightedValue + Bias);
    }

    protected abstract float ActivationFunction(float value);
}