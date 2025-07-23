using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private GridManager _grid;
    
    
    private void Reset()
    {
        _grid = FindAnyObjectByType<GridManager>();
    }
    
    private void Awake()
    {
        _nextButton.onClick.AddListener(OnNextButtonClick);
        _restartButton.onClick.AddListener(OnRestartButtonClick);
    }
    
    
    private void OnNextButtonClick()
    {
        _grid.Play(_grid.CurrentLevelIndex + 1);
    }
    
    private void OnRestartButtonClick()
    {
        _grid.Restart();
    }
}