using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RollBoardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentScoreValue;
    [SerializeField] private TextMeshProUGUI _TotalScoreValue;
    [SerializeField] private Button _randomRollButton;
    
    private void Start()
    {
        GameManager.Instance.DiceController.RollEndEvent.AddListener(OnRollEnd);
        GameManager.Instance.DiceController.StartRollDieEvent.AddListener(OnRollStart);
    }

    private void OnRollEnd(int currentScore, int totalScore)
    {
        _currentScoreValue.text = currentScore.ToString();
        _TotalScoreValue.text = totalScore.ToString();
        _randomRollButton.interactable = true;
    }
    
    private void OnRollStart()
    {
        _currentScoreValue.text = "?";
        _randomRollButton.interactable = false;
    }
    
    public void OnRollButtonClick()
    {
        if(GameManager.Instance.DiceController.RandomRoll())
            _randomRollButton.interactable = false;
    }
}
