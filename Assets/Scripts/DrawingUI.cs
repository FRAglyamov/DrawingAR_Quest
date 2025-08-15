using UnityEngine;
using UnityEngine.UI;

public class DrawingUI : MonoBehaviour
{
    [SerializeField] private FingerDrawingSystem _drawingSystem;
    [SerializeField] private DrawingSaver _drawingSaver;
    [SerializeField] private Button _colorButton;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _loadButton;
    [SerializeField] private Button _clearButton;

    private Color[] _colors = { Color.red, Color.blue };
    private int _currentColorIndex;

    private void Start()
    {
        _colorButton.onClick.AddListener(ToggleColor);
        _saveButton.onClick.AddListener(_drawingSaver.SaveDrawing);
        _loadButton.onClick.AddListener(_drawingSaver.LoadDrawing);
        _clearButton.onClick.AddListener(_drawingSystem.ClearAll);
    }

    private void ToggleColor()
    {
        _currentColorIndex = (_currentColorIndex + 1) % _colors.Length;
        _drawingSystem.SetColor(_colors[_currentColorIndex]);
    }
}
