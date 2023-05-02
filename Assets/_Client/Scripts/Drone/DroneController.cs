using System;
using UnityEngine;


[RequireComponent(typeof(DroneBrain))]
public class DroneController : MonoBehaviour
{
    public DroneBrain Brain => _brain;
    public Transform Core => _coreTransform;
    public float LiveTime => _liveTime;


    [SerializeField] private Rotor[] _rotors;
    [SerializeField] private Rigidbody _coreBody;

    private Transform _coreTransform;
    private DroneBrain _brain;
    private ObstacleDetector[] _groundDetectors;

    private float[] _rotorsPowers;
    private float _liveTime;

    private bool _isActive = true;


    private void CalculateRotorsPowers()
    {
        Vector3[] rotorsVelocities = new Vector3[4];
        for (int i = 0; i < 4; i++) {
            rotorsVelocities[i] = _rotors[i].Body.velocity;
        }

        Vector3 coreRotation = _coreTransform.eulerAngles;
        float height = _coreTransform.position.y;
        
        //euler angles may be not the same as quaternion angles
        //it's cool to make an experiment in comparing accuracy of ml with both angle formats
        
        _rotorsPowers = _brain.PredictRotorPowers(rotorsVelocities, coreRotation, height);
    }

    private void ApplyRotorsPowers(float dt)
    {
        for (int i = 0; i < _rotors.Length; i++) {
            _rotors[i].Pull(_rotorsPowers[i], dt);
        }
    }

    private void Deactivate()
    {
        _isActive = false;
        
        _coreBody.Sleep();

        foreach (Rotor rotor in _rotors) {
            rotor.Body.Sleep();
        }
    }


    private void Awake()
    {
        _brain = GetComponent<DroneBrain>();
        _coreTransform = _coreBody.transform;
        _groundDetectors = GetComponentsInChildren<ObstacleDetector>();
    }

    private void Start()
    {
        foreach (ObstacleDetector detector in _groundDetectors) {
            detector.OnObstacleDetected += Deactivate;
        }
        
        _rotorsPowers = new float[4];
    }

    private void Update()
    {
        if (!_isActive) return;
        
        _liveTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!_isActive) return;
        
        CalculateRotorsPowers();
        ApplyRotorsPowers(Time.fixedDeltaTime);
    }

    private void OnDisable()
    {
        foreach (ObstacleDetector detector in _groundDetectors) {
            detector.OnObstacleDetected -= Deactivate;
        }
    }
}