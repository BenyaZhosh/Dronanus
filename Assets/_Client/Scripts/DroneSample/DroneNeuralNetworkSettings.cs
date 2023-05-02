using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DroneNNSettings", menuName = "Custom/NeuralNetworkSettings/Drone")]
public class DroneNeuralNetworkSettings : ScriptableObject
{
    public readonly int InputSize = 16;
    public readonly int OutputSize = 4;
        
        
    public float MutationStrength => _mutationStrength;
    public float MutationChance => _mutationChance;
    public int[] HiddenLayers => _hiddenLayers;

    public int[] Layers {
        get {
            if (_layers == null || _layers.Length < 2) {
                _layers = new[] { InputSize }.Concat(_hiddenLayers).Concat(new[] { OutputSize }).ToArray();
            }
            return _layers;
        }
    }
        

    [SerializeField] private float _mutationStrength;
    [SerializeField] private float _mutationChance;
    [SerializeField] private int[] _hiddenLayers;

    private int[] _layers;
        
        
    public SigmoidNeuron NeuronConstructor(SigmoidNeuron[] inputs) => new SigmoidNeuron(inputs);
}