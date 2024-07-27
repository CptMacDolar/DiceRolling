using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [field:SerializeField] public DiceController DiceController { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);
    }
}
