using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Система сохранения и загрузки рисунков из линий в формате JSON
/// </summary>
public class DrawingSaver : MonoBehaviour
{
    [SerializeField] private DrawingManager _drawingManager;

    private string _savePath; // Полный путь к файлу сохранения

    private void Start()
    {
        if (_drawingManager == null)
        {
            Debug.LogError("Dependencies not assigned!", this);
            enabled = false;
            return;
        }

        _savePath = Path.Combine(Application.persistentDataPath, "drawing.json");
    }

    /// <summary>
    /// Сохраняет текущий рисунок в JSON файл
    /// </summary>
    public void SaveDrawing()
    {
        var data = _drawingManager.GetDrawingData();
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
            Debug.LogWarning($"No file to load at this path: {_savePath}", this);
            return; 
        }

        try
        {
            string json = File.ReadAllText(_savePath);
            var data = JsonUtility.FromJson<DrawingData>(json);
            _drawingManager.LoadDrawing(data);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load drawing: {e.Message}");
        }
    }
}
