using System;
using UnityEngine;

using Random = UnityEngine.Random;

public class DroneConscript : MonoBehaviour, IConscript
{
    public event Action OnGivingUp;


    [SerializeField] private DroneNeuralNetworkSettings _neuralNetworkSettings;
    [SerializeField] private float _maxOffsetRatio;
    [SerializeField] private Rotor[] _rotors;
    [SerializeField] private Rigidbody _coreBody;


    private NeuralNetwork<SigmoidNeuron> _network;
    private Transform _coreTransform;
    private ObstacleDetector[] _groundDetectors;
    
    private Vector3 _origin;
    private float _arenaSize;
    
    private float _liveTime;
    private Vector3 _startPosition;
    
    private float[] _rotorsPowers;
    private bool _isActive = true;


    #region Conscript methods

    public void SetOrigin(Vector3 origin, float arenaSize)
    {
        _arenaSize = arenaSize;
        _origin = origin + Vector3.up * _arenaSize / 2;
    }

    public int Compare(IConscript comparable)
    {
        DroneConscript comparableDrone = comparable as DroneConscript;
        if (!comparableDrone)
            return 0;

        if (_liveTime > comparableDrone._liveTime) {
            return 1;
        }

        if (_liveTime < comparableDrone._liveTime) {
            return -1;
        }

        float dist = (_startPosition - _coreTransform.position).magnitude;
        float comparableDist = (comparableDrone._startPosition - comparableDrone._coreTransform.position).magnitude;
            
        if (dist > comparableDist) {
            return 1;
        }

        return dist < comparableDist ? -1 : 0;
    }

    public void Resemble(IConscript prototype)
    {
        DroneConscript prototypeDrone = prototype as DroneConscript;
        if (!prototypeDrone)
            return;

        _network = prototypeDrone._network.CreateMutatedChild(_neuralNetworkSettings.MutationStrength, _neuralNetworkSettings.MutationChance);
    }

    public void Reset()
    {
        Vector3 randomPosition = new Vector3(Random.value, Random.value, Random.value) * (_maxOffsetRatio * _arenaSize / 2);
        _startPosition = _origin + randomPosition;
        
        SetPosition(_startPosition);
        WakeUp();
    }

    #endregion

    #region Utilities

    private void SetPosition(Vector3 newPosition)
    {
        _coreTransform.rotation = Quaternion.identity;
        _coreTransform.position = newPosition;
        foreach (Rotor rotor in _rotors) {
            Transform rotorTransform = rotor.transform;
            
            rotorTransform.rotation = Quaternion.identity;
            rotorTransform.position = rotor.OriginPosition + newPosition;
        }
    }

    private void Sleep()
    {
        _isActive = false;
        
        _coreBody.constraints = RigidbodyConstraints.FreezeAll;
        foreach (Rotor rotor in _rotors) {
            rotor.Body.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void WakeUp()
    {
        _isActive = true;
        
        _coreBody.constraints = RigidbodyConstraints.None;
        foreach (Rotor rotor in _rotors) {
            rotor.Body.constraints = RigidbodyConstraints.None;
        }
    }

    private void CalculateRotorsPowers()
    {
        Vector3[] rotorsVelocities = new Vector3[4];
        for (int i = 0; i < 4; i++) {
            rotorsVelocities[i] = _rotors[i].Body.velocity;
        }

        Vector3 coreRotation = _coreTransform.eulerAngles;
        float height = _coreTransform.position.y;

        _rotorsPowers = PredictRotorPowers(rotorsVelocities, coreRotation, height);
    }

    private void ApplyRotorsPowers(float dt)
    {
        for (int i = 0; i < _rotors.Length; i++) {
            _rotors[i].Pull(_rotorsPowers[i], dt);
        }
    }
    
    private float[] PredictRotorPowers(Vector3[] rotorVelocities, Vector3 coreRotation, float height)
    {
        float[] inputs = new float[_neuralNetworkSettings.InputSize];

        for (int i = 0; i < rotorVelocities.Length; i++) {
            inputs[i * 3] = rotorVelocities[i].x;
            inputs[i * 3 + 1] = rotorVelocities[i].y;
            inputs[i * 3 + 2] = rotorVelocities[i].z;
        }

        inputs[12] = coreRotation.x;
        inputs[13] = coreRotation.y;
        inputs[14] = coreRotation.z;

        inputs[15] = height;

        return _network.Predict(inputs);
    }

    #endregion

    #region Events

    private void ObstacleDetector_OnObstacleDetected()
    {
        if (!_isActive)
            return;
        
        OnGivingUp?.Invoke();
        Sleep();
    }

    #endregion

    #region Unity

    private void Awake()
    {
        _network = new NeuralNetwork<SigmoidNeuron>(_neuralNetworkSettings.Layers, _neuralNetworkSettings.NeuronConstructor);
        _network.InitializeRandomly();

        _coreTransform = _coreBody.transform;
        _groundDetectors = GetComponentsInChildren<ObstacleDetector>();
        
        _rotorsPowers = new float[4];
    }
    
    private void Start()
    {
        foreach (ObstacleDetector detector in _groundDetectors) {
            detector.OnObstacleDetected += ObstacleDetector_OnObstacleDetected;
        }
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
            detector.OnObstacleDetected -= ObstacleDetector_OnObstacleDetected;
        }
    }

    #endregion
}