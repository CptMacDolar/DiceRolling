using System.Threading.Tasks;
using UnityEngine;

public interface IRollable
{
    GameObject GameObject { get ; } 
    void OnRollStart();
    void OnRollEnd();
    Task<int> GetRolledValue();
    void ResetWalls();
}
