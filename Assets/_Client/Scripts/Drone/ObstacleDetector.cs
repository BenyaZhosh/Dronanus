using System;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class ObstacleDetector : MonoBehaviour
{
    public event Action OnObstacleDetected;


    [SerializeField] private LayerMask _obstacleLayer;


    private void OnCollisionEnter(Collision collision)
    {
        if ((_obstacleLayer.value & (1 << collision.gameObject.layer)) > 0) {
            OnObstacleDetected?.Invoke();
        }
    }
}
