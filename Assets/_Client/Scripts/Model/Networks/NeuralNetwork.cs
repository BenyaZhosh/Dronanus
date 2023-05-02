using System;
using System.Linq;
using UnityEngine;


public class NeuralNetwork<TNeuron> where TNeuron : BaseNeuron
{
    private TNeuron[][] _network;
    
    private int[] _layerSizes;
    private Func<TNeuron[], TNeuron> _neuronConstructor;
    

    public NeuralNetwork(int[] layersSizes, Func<TNeuron[], TNeuron> neuronConstructor)
    {
        _layerSizes = layersSizes;
        _neuronConstructor = neuronConstructor;
        _network = new TNeuron[layersSizes.Length][];
        
        for (int layerIndex = 0; layerIndex < layersSizes.Length; layerIndex++) {
            _network[layerIndex] = new TNeuron[layersSizes[layerIndex]];
            for (int neuronIndex = 0; neuronIndex < layersSizes[layerIndex]; neuronIndex++) {
                if (layerIndex == 0) {
                    _network[layerIndex][neuronIndex] = neuronConstructor(Array.Empty<TNeuron>());
                } else {
                    _network[layerIndex][neuronIndex] = neuronConstructor(_network[layerIndex - 1]);
                }
            }
        }
    }
    

    public float[] Predict(float[] inputs)
    {
        for (int neuronIndex = 0; neuronIndex < inputs.Length && neuronIndex < _network[0].Length; neuronIndex++) {
            _network[0][neuronIndex].SetValue(inputs[neuronIndex]);
        }

        for (int layerIndex = 1; layerIndex < _network.Length; layerIndex++) {
            for (int neuronIndex = 0; neuronIndex < _network[layerIndex].Length; neuronIndex++) {
                _network[layerIndex][neuronIndex].CalculateValue();
            }
        }

        return _network.Last().Select(neuron => neuron.Value).ToArray();
    }

    public void SetWeights(float[][][] newWeights, float[][] biases)
    {
        int maxLayerIndex = Mathf.Max(_network.Length ,newWeights.Length, biases.Length);
        for (int layerIndex = 1; layerIndex < maxLayerIndex; layerIndex++) {
            int maxNeuronIndex = Mathf.Min(_network[layerIndex].Length, newWeights[layerIndex].Length, biases[layerIndex].Length);
            for (int neuronIndex = 0; neuronIndex < maxNeuronIndex; neuronIndex++) {
                _network[layerIndex][neuronIndex].SetWeights(newWeights[layerIndex][neuronIndex], biases[layerIndex][neuronIndex]);
            }
        }
    }

    public float[][][] GetWeights()
    {
        float[][][] weights = new float[_network.Length][][];

        for (int layerIndex = 1; layerIndex < _network.Length; layerIndex++) {
            weights[layerIndex] = new float[_network[layerIndex].Length][];
            for (int neuronIndex = 0; neuronIndex < _network[layerIndex].Length; neuronIndex++) {
                weights[layerIndex][neuronIndex] = _network[layerIndex][neuronIndex].GetWeights();
            }
        }

        return weights;
    }

    public float[][] GetBiases()
    {
        float[][] biases = new float[_network.Length][];

        for (int layerIndex = 1; layerIndex < _network.Length; layerIndex++) {
            biases[layerIndex] = new float[_network[layerIndex].Length];
            for (int neuronIndex = 0; neuronIndex < _network[layerIndex].Length; neuronIndex++) {
                biases[layerIndex][neuronIndex] = _network[layerIndex][neuronIndex].Bias;
            }
        }

        return biases;
    }

    public NeuralNetwork<TNeuron> GetCopy()
    {
        return new NeuralNetwork<TNeuron>(_layerSizes, _neuronConstructor);
    }
}