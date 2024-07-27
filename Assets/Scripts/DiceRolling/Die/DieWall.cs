using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class DieWall : MonoBehaviour
{
    [Header("Adjustable")]
    
    [field: SerializeField] public int WallValue;
    [SerializeField] private Color _numberColor;
    [SerializeField] private Color _rolledNumberColor;
    
    
    [Header("Components")]
    
    [SerializeField] private TextMeshPro _numberText;
    
    private void OnValidate()
    {
        WallValue = Mathf.Clamp(WallValue, -99, 99);
        var valueStr = WallValue.ToString();

        if (Regex.IsMatch(valueStr, "^[69]+$"))
            _numberText.text = valueStr + ".";
        else
            _numberText.text = valueStr;
                
        _numberText.color = _numberColor;
    }

    public void OnBeingDrawn()
    {
        _numberText.color = _rolledNumberColor;
    }
    
    public void ResetToDefault()
    {
        _numberText.color = _numberColor;
    }

    public float GetAngleToVertical()
    {
        return Vector3.Angle(-transform.forward, Vector3.up);
    }
}
