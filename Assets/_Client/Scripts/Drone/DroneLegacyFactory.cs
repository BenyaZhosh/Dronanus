using UnityEngine;

public class DroneLegacyFactory : MonoBehaviour
{
    [SerializeField] private DroneController _dronePrefab;
    [SerializeField] private Transform _container;
    [Header("Mutation Settings")]
    [SerializeField] private float _mutationStrength;
    [SerializeField] private float _mutationChance;
    

    public DroneController CreateDefaultDrone(Vector3 position)
    {
        var drone = Instantiate(_dronePrefab, position, Quaternion.identity, _container);
        drone.Brain.NeuralNetwork.InitializeRandomly();
        
        return drone;
    }

    public DroneController CreateDroneChild(Vector3 position, DroneController parent)
    {
        var droneChild = Instantiate(_dronePrefab, position, Quaternion.identity, _container);
        droneChild.Brain.SetNetwork(parent.Brain.NeuralNetwork.CreateMutatedChild(_mutationStrength, _mutationChance));
        
        return droneChild;
    }
    
    public DroneController CreateDroneClone(Vector3 position, DroneController parent)
    {
        var droneClone = Instantiate(_dronePrefab, position, Quaternion.identity, _container);
        droneClone.Brain.SetNetwork(parent.Brain.NeuralNetwork);
        
        return droneClone;
    }
}