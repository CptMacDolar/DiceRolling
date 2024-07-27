using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DiceController : MonoBehaviour
{
    [SerializeField] private List<Die> _dice;
    [field:SerializeField] public Transform BoardFloor { get; private set; }
    
    [Header("Dragging Borders")]
    [SerializeField] private float _minDragX, _maxDragX, _minDragZ, _maxDragZ;

    public float MinDragX => _minDragX + transform.position.x;
    public float MaxDragX => _maxDragX + transform.position.x;
    public float MinDragZ => _minDragZ + transform.position.z;
    public float MaxDragZ => _maxDragZ + transform.position.z;
    
    [Header("Random Roll")]
    [SerializeField] private float _randomRollForce;
    [SerializeField] private float _randomSpinForce;
    
    private int _currentScore;
    private int _totalScore;
    
    [HideInInspector] public UnityEvent StartRollDieEvent = new();
    [HideInInspector] public UnityEvent<int, int> RollEndEvent = new();

    
#if UNITY_EDITOR
    private void OnValidate()
    {
        _dice.Clear();
        _dice.AddRange(FindObjectsOfType<Die>());
    }
#endif

    public void OnValueDraw(int value)
    {
        _currentScore = value;
        _totalScore += value;
        
        RollEndEvent?.Invoke(_currentScore, _totalScore);
    }

    public void OnStartRoll()
    {
        StartRollDieEvent?.Invoke();
    }
    
    public bool RandomRoll()
    {
        var die = _dice.FirstOrDefault();

        if (die == null)
            return false;
        
        die.RandomRoll(_randomRollForce, _randomSpinForce);
        return true;
    }
}
