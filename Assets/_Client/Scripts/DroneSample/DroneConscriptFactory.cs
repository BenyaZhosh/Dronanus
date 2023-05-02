using UnityEngine;

public class DroneConscriptFactory : MonoBehaviour, IConscriptFactory
{
    [SerializeField] private DroneConscript _droneConscriptPrefab;
    [SerializeField] private Transform _container;
    
    public IConscript CreateConscript()
    {
        IConscript droneConscript = Instantiate(_droneConscriptPrefab, _container);

        return droneConscript;
    }
}