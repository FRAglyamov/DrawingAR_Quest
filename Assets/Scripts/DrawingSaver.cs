using UnityEngine;
using System.IO;
using System;

public class DrawingSaver : MonoBehaviour
{
    private string _savePath;
    private FingerDrawingSystem _drawingSystem;

    private void Awake()
    {
        _savePath = Path.Combine(Application.persistentDataPath, "drawing.json");
        _drawingSystem = FindFirstObjectByType<FingerDrawingSystem>();
    }

    public void SaveDrawing()
    {
        var data = _drawingSystem.GetDrawingData();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"Drawing saved to {_savePath}");
    }

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
