using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Rotor : MonoBehaviour
{
    public Rigidbody Body { get; private set; }
    public Vector3 OriginPosition { get; private set; }


    [SerializeField] private float _maxTrust;


    public void Pull(float power, float dt)
    {
        power = Mathf.Clamp01(power);
        Body.AddForce(transform.up * (_maxTrust * power * dt));
    }
    
    
    private void Awake()
    {
        OriginPosition = transform.position;
        Body = gameObject.GetComponent<Rigidbody>();
    }
}
