using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Основная система, менеджер для рисования пальцем на поверхностях
/// </summary>
public class DrawingManager : MonoBehaviour
{
    [Header("Drawing Settings")]
    [SerializeField] private SurfaceDetector _surfaceDetector;
    [SerializeField] private LineRenderer _linePrefab;
    [SerializeField] private float _lineWidth = 0.01f;
    [SerializeField] private int _maxLines = 100; // Лимит линий для предотвращения утечек

    private List<LineRenderer> _lines = new();
    private ObjectPool<LineRenderer> _linePool;
    private Color _currentColor = Color.red;

    private void Start()
    {
        if (_surfaceDetector == null || _linePrefab == null)
        {
            Debug.LogError("Dependencies not assigned!", this);
            enabled = false;
        }

        InitializeLinePool();
    }

    /// <summary>
    /// Инициализация пула объектов для рисования LineRenderer
    /// </summary>
    private void InitializeLinePool()
    {
        _linePool = new ObjectPool<LineRenderer>(
            createFunc: () => Instantiate(_linePrefab, _surfaceDetector.transform), // Создаем новый LineRenderer как дочерний объект поверхности
            actionOnGet: (line) => line.gameObject.SetActive(true),
            actionOnRelease: (line) => line.gameObject.SetActive(false),
            actionOnDestroy: (line) => Destroy(line.gameObject),
            defaultCapacity: 10,
            maxSize: _maxLines
        );
    }

    public bool IsTouchingSurface(Vector3 position, out Vector3 surfacePoint)
    {
        return _surfaceDetector.IsTouchingSurface(position, out surfacePoint);
    }

    /// <summary>
    /// Создает новую линию для руки
    /// </summary>
    /// <param name="startPoint">Начальная точка линии на поверхности</param>
    /// <returns>Созданный LineRenderer</returns>
    public LineRenderer CreateNewLine(Vector3 startPoint)
    {
        // Удаляем самую старую линию, если достигнут лимит
        if (_lines.Count >= _maxLines)
        {
            LineRenderer oldestLine = _lines[0];
            _lines.RemoveAt(0);
            _linePool.Release(oldestLine);
        }

        LineRenderer newLine = _linePool.Get();

        // Настраиваем визуальные параметры
        newLine.startColor = _currentColor;
        newLine.endColor = _currentColor;
        newLine.startWidth = _lineWidth;
        newLine.endWidth = _lineWidth;

        // Устанавливаем начальную точку
        newLine.positionCount = 1;
        newLine.SetPosition(0, startPoint);

        _lines.Add(newLine); // Добавляем в список всех линий
        return newLine;
    }


    public void SetColor(Color color) => _currentColor = color;

    /// <summary>
    /// Очищает все нарисованные линии
    /// </summary>
    public void ClearAll()
    {
        foreach (var line in _lines)
            _linePool.Release(line);
        _lines.Clear();
    }

    /// <summary>
    /// Собирает данные всех линий для сериализации
    /// </summary>
    /// <returns>DrawingData с цветами и координатами всех точек линий</returns>
    public DrawingData GetDrawingData()
    {
        var data = new DrawingData();

        foreach (var line in _lines)
        {
            var lineData = new LineData
            {
                Color = line.startColor,
                Positions = new Vector3[line.positionCount] // Выделяем массив
            };
            line.GetPositions(lineData.Positions); // Копируем точки

            // Преобразуем мировые координаты в локальные относительно поверхности
            for (int i = 0; i < lineData.Positions.Length; i++)
            {
                lineData.Positions[i] = _surfaceDetector.WorldToSurfacePoint(lineData.Positions[i]);
            }

            data.Lines.Add(lineData);
        }

        return data;
    }

    /// <summary>
    /// Восстанавливает рисунок из сохраненных данных
    /// </summary>
    /// <param name="data">Содержит линии с цветами и координатами</param>
    public void LoadDrawing(DrawingData data)
    {
        ClearAll();

        foreach (var lineData in data.Lines)
        {
            if (_lines.Count >= _maxLines) break;

            var line = _linePool.Get();

            line.startColor = lineData.Color;
            line.endColor = lineData.Color;
            line.startWidth = _lineWidth;
            line.endWidth = _lineWidth;

            line.positionCount = lineData.Positions.Length;

            // Преобразуем локальные координаты относительно поверхности в мировые
            Vector3[] worldPositions = new Vector3[lineData.Positions.Length];
            for (int i = 0; i < lineData.Positions.Length; i++)
            {
                worldPositions[i] = _surfaceDetector.SurfaceToWorldPoint(lineData.Positions[i]);
            }

            line.SetPositions(worldPositions);

            _lines.Add(line);
        }
    }
}

/// <summary>
/// Контейнер данных для сериализации одной линии
/// </summary>
[System.Serializable]
public class DrawingData
{
    public List<LineData> Lines = new();
}

/// <summary>
/// Контейнер данных для всего рисунка
/// </summary>
[System.Serializable]
public class LineData
{
    public Color Color;
    public Vector3[] Positions;
}
