using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Управление пользовательским интерфейсом для системы рисования
/// </summary>
public class DrawingUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private DrawingManager _drawingManager;
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
        if (_drawingManager == null || _drawingSaver == null)
        {
            Debug.LogError("Dependencies not assigned!", this);
            enabled = false;
            return;
        }

        _colorButton.onClick.AddListener(ToggleColor);
        _saveButton.onClick.AddListener(_drawingSaver.SaveDrawing);
        _loadButton.onClick.AddListener(_drawingSaver.LoadDrawing);
        _clearButton.onClick.AddListener(_drawingManager.ClearAll);
    }

    /// <summary>
    /// Переключает цвет рисования
    /// </summary>
    private void ToggleColor()
    {
        _currentColorIndex = (_currentColorIndex + 1) % _colors.Length;
        _drawingManager.SetColor(_colors[_currentColorIndex]);
    }
}
