using UnityEngine;

public interface IDraggable
{
    GameObject GameObject { get ; }
    bool CanBeDragged { get; }
    Rigidbody GetRigidbody();
    Vector3 GetPosition();
    void SetPosition(Vector3 nextPosition);
    void OnStartDragging();
}
