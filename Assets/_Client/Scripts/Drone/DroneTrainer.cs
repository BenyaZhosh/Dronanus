using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Comparers;


public class DroneTrainer : MonoBehaviour
{
    [SerializeField] private DroneLegacyFactory _legacyFactory;
    [SerializeField] private GameObject _droneArenaPrefab;
    [SerializeField] private Transform _droneArenasContainer;
    [SerializeField] private float _spacing;
    [SerializeField] private float _height;
    [Header("Training settings")]
    [SerializeField] private float _sessionDuration;
    [SerializeField] private int _packSize;
    [SerializeField] private int _packCount;

    private DroneController[] _drones;

    private float _sessionCd;


    private void CreatePack(Vector3 pivot, int packIndex)
    {
        for (int i = 0; i < _packSize; i++) {
            _drones[packIndex * _packSize + i] = _legacyFactory.CreateDefaultDrone(pivot + Vector3.right * (_spacing * i) + Vector3.up * _height);
        }
    }
    
    private void CreateMutatedPack(DroneController parent, Vector3 pivot, int packIndex)
    {
        _drones[packIndex] = _legacyFactory.CreateDroneClone(pivot + Vector3.up * _height, parent);
        for (int i = 1; i < _packSize; i++) {
            _drones[packIndex * _packSize + i] = _legacyFactory.CreateDroneChild(pivot + Vector3.right * (_spacing * i) + Vector3.up * _height, parent);
        }
    }
    

    private void Start()
    {
        _drones = new DroneController[_packCount * _packSize];

        for (int y = 0; y < _packCount; y++) {
            for (int x = 0; x < _packSize; x++) {
                Vector3 arenaPos = new Vector3(_spacing * x, 0, _spacing * y);
                Instantiate(_droneArenaPrefab, arenaPos, Quaternion.identity, _droneArenasContainer);
            }
        }
        
        for (int i = 0; i < _packCount; i++) {
            CreatePack(Vector3.forward * (_spacing * i), i);
        }
    }

    private void Update()
    {
        _sessionCd += Time.deltaTime;

        if (_sessionCd > _sessionDuration) {
            Array.Sort(_drones, (firstDrone, secondDrone) => {
                if (firstDrone.LiveTime > secondDrone.LiveTime)
                    return -1;
                if (firstDrone.LiveTime < secondDrone.LiveTime)
                    return 1;
                return 0;
            });
            DroneController[] bests = _drones.Take(_packCount).ToArray();
            DroneController[] prevDrones = new DroneController[_packCount * _packSize];
            for (int i = 0; i < _drones.Length; i++) {
                prevDrones[i] = _drones[i];
            }

            for (int i = 0; i < _packCount; i++) {
                CreateMutatedPack(bests[i], Vector3.forward * (_spacing * i), i);
            }
            
            foreach (DroneController drone in prevDrones) {
                if (drone.gameObject)
                    Destroy(drone.gameObject);
            }
            
            _sessionCd = 0;
        }
    }
}
