using System;
using System.Linq;
using UnityEngine;


public class DroneBrain : MonoBehaviour
{
    public NeuralNetwork<SigmoidNeuron> NeuralNetwork => _neuralNetwork;
    
    [SerializeField] private int[] _hiddenLayers;
    
    private NeuralNetwork<SigmoidNeuron> _neuralNetwork;
    
    private readonly int InputSize = 16;
    private readonly int OutputSize = 4;
    
    
    
    public float[] PredictRotorPowers(Vector3[] rotorVelocities, Vector3 coreRotation, float height)
    {
        float[] inputs = new float[InputSize];

        for (int i = 0; i < rotorVelocities.Length; i++) {
            inputs[i * 3] = rotorVelocities[i].x;
            inputs[i * 3 + 1] = rotorVelocities[i].y;
            inputs[i * 3 + 2] = rotorVelocities[i].z;
        }

        inputs[12] = coreRotation.x;
        inputs[13] = coreRotation.y;
        inputs[14] = coreRotation.z;

        inputs[15] = height;

        return _neuralNetwork.Predict(inputs);
    }

    public void SetNetwork(NeuralNetwork<SigmoidNeuron> network)
    {
        _neuralNetwork = network;
    }


    private void Awake()
    {
        if (_neuralNetwork == null) {
            int[] layersSizes = new[] { InputSize }.Concat(_hiddenLayers).Concat(new[] { OutputSize }).ToArray();
            SigmoidNeuron NeuronConstructor(SigmoidNeuron[] inputs) => new SigmoidNeuron(inputs);
            _neuralNetwork = new NeuralNetwork<SigmoidNeuron>(layersSizes, NeuronConstructor);
        }
    }
}
