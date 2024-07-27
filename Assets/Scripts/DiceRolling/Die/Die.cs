using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Die : MonoBehaviour, IDraggable, IRollable
{
    /// <summary>
    /// maximal roll time duration in seconds
    /// </summary>
    [SerializeField] private float _timeOut = 5;
    
    [SerializeField] private List<DieWall> _walls;
    [SerializeField, HideInInspector] private Rigidbody _rigidbody;
    [SerializeField, HideInInspector] private FeelEnhancer _feelEnhancer;
    
    public GameObject GameObject => gameObject;
    public bool CanBeDragged => _canBeDragged;

    private DieWall _drawnWall;
    private bool _canBeDragged = true;

#if UNITY_EDITOR
    private void OnValidate()
    {
        _walls.Clear();
        _walls.AddRange(transform.GetComponentsInChildren<DieWall>());
        _rigidbody = GetComponent<Rigidbody>();
        _feelEnhancer = GetComponent<FeelEnhancer>();
    }
#endif
    
    public Rigidbody GetRigidbody()
    {
        return _rigidbody;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetPosition(Vector3 nextPosition)
    {
        transform.position = nextPosition;
    }

    public void OnStartDragging()
    {
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        ResetWalls();
    }

    public async void OnRollStart()
    {
        _canBeDragged = false;
        GameManager.Instance.DiceController.OnStartRoll();

        var rollEnded = new[]
        {
            DieStopped(),
            TimeOut()
        };
        
        await Task.WhenAny(rollEnded);
        OnRollEnd();
    }

    private async Task DieStopped()
    {
        await Task.Delay(100);
        while (!_feelEnhancer.IsStopped)
            await Task.Delay(100);
    }
    
    private async Task TimeOut()
    {
        await Task.Delay((int)(_timeOut * 1000));
    }
    
    public void OnRollEnd()
    {
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        
        _drawnWall = _walls.OrderByDescending(item => item.transform.position.y).First();
        _drawnWall.OnBeingDrawn();
        
        GameManager.Instance.DiceController.OnValueDraw(_drawnWall.WallValue);
        _canBeDragged = true;
    }

    public async Task<int> GetRolledValue()
    {
        while (_drawnWall == null)
            await Task.Delay(100);

        return _drawnWall.WallValue;
    }

    public void ResetWalls()
    {
        if (_drawnWall != null)
            _drawnWall.ResetToDefault();
        _drawnWall = null;
        _canBeDragged = true;
    }

    public void RandomRoll(float rollForce, float spinForce)
    {
        ResetWalls();
        
        var randomDirection = Vector3.Scale(Random.onUnitSphere, new Vector3(1, 0.5f, 1));
        
        _rigidbody.constraints = RigidbodyConstraints.None;
        _rigidbody.AddForce(randomDirection * rollForce, ForceMode.Impulse);
        _rigidbody.AddTorque(Vector3.Cross(randomDirection, Vector3.down) * spinForce, ForceMode.Impulse);
        
        OnRollStart();
    }
}
