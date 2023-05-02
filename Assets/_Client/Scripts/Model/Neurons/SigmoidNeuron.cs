using UnityEngine;


public class SigmoidNeuron : BaseNeuron
{
    public SigmoidNeuron(BaseNeuron[] inputs) : base(inputs) { }
    
    
    protected override float ActivationFunction(float value)
    {
        return 1 / (1 + Mathf.Exp(value));
    }
}