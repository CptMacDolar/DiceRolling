using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DragAndRoll : MonoBehaviour
{
    [Header("Rolling parameters")]
    [SerializeField] private float _rollingForce;
    [SerializeField] private float _spinForce;
    
    [Header("Draggin parameters")]
    [SerializeField] private float _heightAboveBoard = 5;
    [SerializeField] private float _puttingDownHeightAboveBoard = 1;
    /// <summary>
    /// lengthens the time after the drag in which die can be rolled, value closer to 0 means longer time
    /// </summary>
    [SerializeField] private float _smoothingFactor = 0.5f;
    /// <summary>
    /// minimal drag speed on which die can be rolled
    /// </summary>
    [SerializeField] private float _minimalDragSpeed = 0.01f;
    
    private IDraggable _selectedDraggable;
    private Vector3 _smoothedPreviousPosition;
    private Vector3 _deltaPosition;
    private DiceController _diceController;

    private void Start()
    {
        _diceController = GameManager.Instance.DiceController;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PickUp();
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            Roll();
        }
        
        
    }

    private void FixedUpdate()
    {
        if(_selectedDraggable != null)
        {
            Drag();
        }
    }

    private void PickUp()
    {
        if (_selectedDraggable != null)
            return;
        
        RaycastHit hit = CastRay();

        if (hit.collider == null || !hit.collider.CompareTag("Draggable"))
            return;

        if (!hit.collider.TryGetComponent(out IDraggable draggable))
            return;
        
        if(!draggable.CanBeDragged)
            return;
        
        _selectedDraggable = draggable;
        
        draggable.OnStartDragging();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }
    
    private void Drag()
    {
        Vector3 position = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.WorldToScreenPoint(_selectedDraggable.GetPosition()).z);
        
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
        worldPosition = ClampPositionByBoundaries(worldPosition);
        _selectedDraggable.SetPosition(new Vector3(
            worldPosition.x,
            _diceController.BoardFloor.transform.position.y + _heightAboveBoard,
            worldPosition.z));

        _deltaPosition = worldPosition - _smoothedPreviousPosition;
        _deltaPosition = Vector3.Scale(_deltaPosition, new Vector3(1, 0, 1));
        _smoothedPreviousPosition = Vector3.Lerp(_smoothedPreviousPosition, worldPosition, _smoothingFactor);
    }

    private Vector3 ClampPositionByBoundaries(Vector3 worldPos)
    {
        var clampedPos = new Vector3(
            Mathf.Clamp(worldPos.x, _diceController.MinDragX, _diceController.MaxDragX),
            worldPos.y,
            Mathf.Clamp(worldPos.z, _diceController.MinDragZ, _diceController.MaxDragZ));
        
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(clampedPos);
        Cursor.SetCursor(null, new Vector2(screenPosition.x, Screen.height - screenPosition.y), CursorMode.Auto);
        
        return clampedPos;
    }

    private void Roll()
    {
        if(_selectedDraggable == null)
            return;

        print(_deltaPosition.sqrMagnitude);
        
        if (_deltaPosition.sqrMagnitude < _minimalDragSpeed)
        {
            PutDown();
            return;
        }
        
        var rb = _selectedDraggable.GetRigidbody();
        rb.constraints = RigidbodyConstraints.None;
        
        var randomizationFactor = Random.Range(0.8f, 1.2f);
        rb.AddForce(_deltaPosition * (_rollingForce * randomizationFactor), ForceMode.Impulse);
        rb.AddTorque(Vector3.Cross(_deltaPosition, Vector3.down) * (_spinForce * randomizationFactor), ForceMode.Impulse);

        if (_selectedDraggable.GameObject.TryGetComponent(out IRollable rollable))
        {
            rollable.OnRollStart();
        }
        
        _selectedDraggable = null;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void PutDown()
    {
        _selectedDraggable.GetRigidbody().constraints = RigidbodyConstraints.FreezeAll;
        
        _selectedDraggable.SetPosition(new Vector3(
            _selectedDraggable.GetPosition().x,
            _diceController.BoardFloor.transform.position.y + _puttingDownHeightAboveBoard,
            _selectedDraggable.GetPosition().z));
        
        _selectedDraggable = null;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private RaycastHit CastRay()
    {
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane));
        
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint( new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane));

        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out var hit);

        return hit;
    }
}