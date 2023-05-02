using UnityEngine;

public static class NeuralNetworkInitializers
{
    public static void InitializeRandomly<TNeuron>(this NeuralNetwork<TNeuron> network) where TNeuron : BaseNeuron
    {
        float[][][] weights = network.GetWeights();
        float[][] biases = network.GetBiases();
        
        for (int layerIndex = 1; layerIndex < weights.Length; layerIndex++) {
            for (int neuronIndex = 0; neuronIndex < weights[layerIndex].Length; neuronIndex++) {
                for (int weightIndex = 0; weightIndex < weights[layerIndex][neuronIndex].Length; weightIndex++) {
                    weights[layerIndex][neuronIndex][weightIndex] = Random.value * 2 - 1;
                }

                biases[layerIndex][neuronIndex] = Random.value * 2 - 1;
            }
        }
        
        network.SetWeights(weights, biases);
    }
}
