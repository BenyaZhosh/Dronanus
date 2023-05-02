using UnityEngine;

public static class NeuralNetworkExtensions
{
    public static NeuralNetwork<TNeuron> CreateMutatedChild<TNeuron>(this NeuralNetwork<TNeuron> parent, float mutationStrength, float mutationChance) where TNeuron : BaseNeuron
    {
        float[][][] weights = parent.GetWeights();
        float[][] biases = parent.GetBiases();
        
        for (int layerIndex = 1; layerIndex < weights.Length; layerIndex++) {
            for (int neuronIndex = 0; neuronIndex < weights[layerIndex].Length; neuronIndex++) {
                if (mutationChance >= Random.value) {
                    for (int weightIndex = 0; weightIndex < weights[layerIndex][neuronIndex].Length; weightIndex++) {
                        weights[layerIndex][neuronIndex][weightIndex] += Random.value * mutationStrength;
                    }
                    biases[layerIndex][neuronIndex] += Random.value * mutationStrength;
                }
            }
        }

        NeuralNetwork<TNeuron> child = parent.GetCopy();
        child.SetWeights(weights, biases);

        return child;
    }
}