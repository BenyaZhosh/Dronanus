using System;
using UnityEngine;


public class NaturalSelectionTrainer : MonoBehaviour
{
    private int ConscriptsCount => _conscriptsGroupsCount * _conscriptsGroupSize;
    
    [Header("Training arena")]
    [SerializeField] private GameObject _conscriptFactoryObject;
    [SerializeField] private GameObject _arenaPrefab;
    [SerializeField] private float _arenaSize;
    [SerializeField] private bool _singleArena;
    [Header("Training process")]
    [SerializeField] private int _trainingSessionDuration;
    [SerializeField] private int _conscriptsGroupSize;
    [SerializeField] private int _conscriptsGroupsCount;
    
    private IConscriptFactory _factory;

    private IConscript[] _conscripts;
    private int _givenUpCount;
    private float _sessionCooldown;


    private void GenerateTrainingArena()
    {
        if (!_arenaPrefab)
            return;

        Vector3 offset = transform.position;
        
        if (_singleArena) {
            GameObject arena = Instantiate(_arenaPrefab, offset, Quaternion.identity, transform);
            arena.transform.localScale = Vector3.one * _arenaSize;
            return;
        }

        for (int groupIndex = 0; groupIndex < _conscriptsGroupsCount; groupIndex++) {
            for (int conscriptIndex = 0; conscriptIndex < _conscriptsGroupSize; conscriptIndex++) {
                Vector3 arenaPosition = new Vector3(conscriptIndex, 0, groupIndex) * _arenaSize + offset;
                GameObject arena = Instantiate(_arenaPrefab, arenaPosition, Quaternion.identity, transform);
                arena.transform.localScale = Vector3.one * _arenaSize;
            }
        }
    }

    private void CreateConscripts()
    {
        _conscripts = new IConscript[ConscriptsCount];
        Vector3 offset = transform.position;
        
        for (int groupIndex = 0; groupIndex < _conscriptsGroupsCount; groupIndex++) {
            for (int conscriptIndex = 0; conscriptIndex < _conscriptsGroupSize; conscriptIndex++) {
                IConscript conscript = _factory.CreateConscript();

                Vector3 origin = new Vector3(conscriptIndex, 0, groupIndex) * _arenaSize * (_singleArena ? 0 : 1);
                conscript.SetOrigin(origin + offset, _arenaSize);
                conscript.OnGivingUp += Conscript_OnGivingUp;
                
                _conscripts[groupIndex * _conscriptsGroupSize + conscriptIndex] = conscript;
            }
        }
    }

    private void ResetConscripts()
    {
        foreach (IConscript conscript in _conscripts) {
            conscript.Reset();
        }

        _givenUpCount = 0;
    }

    private void ResembleGroup(IConscript prototype, int groupStartingIndex)
    {
        for (int i = groupStartingIndex; i < groupStartingIndex + _conscriptsGroupSize - 1; i++) {
            _conscripts[i].Resemble(prototype);
        }
    }

    private void Conscript_OnGivingUp()
    {
        _givenUpCount++;

        if (_givenUpCount >= ConscriptsCount) {
            _sessionCooldown = 0;
        }
    }


    private void Start()
    {
        _factory = _conscriptFactoryObject.GetComponent<IConscriptFactory>();
        
        GenerateTrainingArena();
        CreateConscripts();
        ResetConscripts();

        _sessionCooldown = _trainingSessionDuration;
    }

    private void Update()
    {
        _sessionCooldown -= Time.deltaTime;

        if (_sessionCooldown <= 0) {
            Array.Sort(_conscripts, (conscript1, conscript2) => -conscript1.Compare(conscript2));

            for (int i = 0; i < _conscriptsGroupsCount; i++) {
                ResembleGroup(_conscripts[i], _conscriptsGroupsCount + (_conscriptsGroupSize - 1) * i);
            }
            
            ResetConscripts();
            
            _sessionCooldown = _trainingSessionDuration;
        }
    }

    private void OnDisable()
    {
        foreach (IConscript conscript in _conscripts) {
            conscript.OnGivingUp -= Conscript_OnGivingUp;
        }
    }
}
