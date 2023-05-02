using System;
using UnityEngine;

public interface IConscript
{
    event Action OnGivingUp;

    void SetOrigin(Vector3 origin, float arenaSize);
    int Compare(IConscript comparable);
    void Resemble(IConscript prototype);
    void Reset();
}