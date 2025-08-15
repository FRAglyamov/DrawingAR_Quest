using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Управление пользовательским интерфейсом для системы рисования
/// </summary>
public class DrawingUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FingerDrawingSystem _drawingSystem;
    [SerializeField] private DrawingSaver _drawingSaver;

    [Header("UI Elements")]
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

    /// <summary>
    /// Переключает цвет рисования
    /// </summary>
    private void ToggleColor()
    {
        _currentColorIndex = (_currentColorIndex + 1) % _colors.Length;
        _drawingSystem.SetColor(_colors[_currentColorIndex]);
    }
}
