using UnityEngine;

public class FeelEnhancer : MonoBehaviour
{
    public bool IsStopped { get; private set; }
    [Header("Parameters")]
    [SerializeField] private float _gravityScale = 1f;
    [SerializeField] private float _stoppingVelocity;
    
    [SerializeField, HideInInspector] private Rigidbody _rigidbody;
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
#endif

    private void FixedUpdate()
    {
        _rigidbody.AddForce(Physics.gravity * (_gravityScale - 1), ForceMode.Acceleration);
        IsStopped = _rigidbody.velocity.sqrMagnitude < _stoppingVelocity;
    }
}
