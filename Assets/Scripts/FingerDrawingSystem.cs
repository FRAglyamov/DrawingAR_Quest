using System.Collections.Generic;
using UnityEngine;

public class FingerDrawingSystem : MonoBehaviour
{
    [SerializeField] private OVRHand _hand;
    [SerializeField] private OVRSkeleton _skeleton;
    [SerializeField] private SurfaceDetector _surfaceDetector;
    [SerializeField] private LineRenderer _linePrefab;

    [SerializeField] private float _lineWidth = 0.01f;

    private LineRenderer _currentLine;
    private List<LineRenderer> _lines = new();
    private OVRBone _indexTip;
    private bool _isDrawing;
    private Color _currentColor = Color.red;

    private void Start()
    {
        if (_skeleton == null) _skeleton = _hand.GetComponent<OVRSkeleton>();

        // Находим кость кончика указательного пальца
        foreach (var bone in _skeleton.Bones)
        {
            if (bone.Id == OVRSkeleton.BoneId.XRHand_IndexTip)
            {
                _indexTip = bone;
                break;
            }
        }
    }

    private void Update()
    {
        if (!_hand.IsTracked || _indexTip == null) return;

        Vector3 fingerPos = _indexTip.Transform.position;

        if (_surfaceDetector.IsTouchingSurface(fingerPos, out Vector3 surfacePoint))
        {
            if (!_isDrawing)
            {
                StartNewLine(surfacePoint);
            }
            else
            {
                ContinueLine(surfacePoint);
            }
        }
        else if (_isDrawing)
        {
            EndLine();
        }
    }

    private void StartNewLine(Vector3 startPoint)
    {
        _currentLine = Instantiate(_linePrefab, _surfaceDetector.transform);
        _currentLine.startColor = _currentColor;
        _currentLine.endColor = _currentColor;
        _currentLine.startWidth = _lineWidth;
        _currentLine.endWidth = _lineWidth;
        _currentLine.positionCount = 1;
        _currentLine.SetPosition(0, startPoint);
        _lines.Add(_currentLine);
        _isDrawing = true;
    }

    private void ContinueLine(Vector3 newPoint)
    {
        _currentLine.positionCount++;
        _currentLine.SetPosition(_currentLine.positionCount - 1, newPoint);
    }

    private void EndLine()
    {
        _isDrawing = false;
        _currentLine = null;
    }

    public void SetColor(Color color) => _currentColor = color;

    public void ClearAll()
    {
        foreach (var line in _lines)
            Destroy(line.gameObject);
        _lines.Clear();
    }

    public DrawingData GetDrawingData()
    {
        var data = new DrawingData();
        foreach (var line in _lines)
        {
            var lineData = new LineData
            {
                Color = line.startColor,
                Positions = new Vector3[line.positionCount] // Создаём массив нужного размера
            };
            line.GetPositions(lineData.Positions); // Заполняем массив точками
            data.Lines.Add(lineData);
        }
        return data;
    }

    public void LoadDrawing(DrawingData data)
    {
        ClearAll();
        foreach (var lineData in data.Lines)
        {
            var line = Instantiate(_linePrefab, _surfaceDetector.transform);
            line.startColor = lineData.Color;
            line.endColor = lineData.Color;
            line.startWidth = _lineWidth;
            line.endWidth = _lineWidth;
            line.positionCount = lineData.Positions.Length;
            line.SetPositions(lineData.Positions);
            _lines.Add(line);
        }
    }
}

[System.Serializable]
public class DrawingData
{
    public List<LineData> Lines = new();
}

[System.Serializable]
public class LineData
{
    public Color Color;
    public Vector3[] Positions;
}
