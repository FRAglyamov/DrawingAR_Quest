using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Система сохранения и загрузки рисунков из линий в формате JSON
/// </summary>
public class DrawingSaver : MonoBehaviour
{
    [SerializeField] private FingerDrawingSystem _drawingSystem;

    private string _savePath; // Полный путь к файлу сохранения

    private void Start()
    {
        if (_drawingSystem == null) _drawingSystem = FindFirstObjectByType<FingerDrawingSystem>();

        _savePath = Path.Combine(Application.persistentDataPath, "drawing.json");
    }

    /// <summary>
    /// Сохраняет текущий рисунок в JSON файл
    /// </summary>
    public void SaveDrawing()
    {
        var data = _drawingSystem.GetDrawingData();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"Drawing saved to {_savePath}");
    }

    /// <summary>
    /// Загружает последний сохраненный рисунок из JSON файла
    /// </summary>
    public void LoadDrawing()
    {
        if (!File.Exists(_savePath)) 
        {
            Debug.LogWarning($"No file to load at this path: {_savePath}");
            return; 
        }

        try
        {
            string json = File.ReadAllText(_savePath);
            var data = JsonUtility.FromJson<DrawingData>(json);
            _drawingSystem.LoadDrawing(data);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load drawing: {e.Message}");
        }
    }
}
