using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// �������� ������� ��� ��������� ������� �� ������������
/// </summary>
public class FingerDrawingSystem : MonoBehaviour
{
    [Header("Hand References")]
    [SerializeField] private OVRHand _hand;
    [SerializeField] private OVRSkeleton _skeleton;

    [Header("Drawing Settings")]
    [SerializeField] private SurfaceDetector _surfaceDetector;
    [SerializeField] private LineRenderer _linePrefab;
    [SerializeField] private float _lineWidth = 0.01f;
    [SerializeField] private int _maxLines = 100; // ����� ����� ��� �������������� ������

    private LineRenderer _currentLine;
    private List<LineRenderer> _lines = new();
    private ObjectPool<LineRenderer> _linePool;
    private OVRBone _indexTip;
    private bool _isDrawing;
    private Color _currentColor = Color.red;


    private void Start()
    {
        InitializeLinePool();

        InitializeFingerTracking();
    }

    /// <summary>
    /// ������������� ���� �������� ��� ��������� LineRenderer
    /// </summary>
    private void InitializeLinePool()
    {
        _linePool = new ObjectPool<LineRenderer>(
            createFunc: () => Instantiate(_linePrefab, _surfaceDetector.transform), // ������� ����� LineRenderer ��� �������� ������ �����������
            actionOnGet: (line) => line.gameObject.SetActive(true),
            actionOnRelease: (line) => line.gameObject.SetActive(false),
            actionOnDestroy: (line) => Destroy(line.gameObject),
            defaultCapacity: 10,
            maxSize: _maxLines
        );
    }

    /// <summary>
    /// ��������� ������ ������ ����
    /// </summary>
    private void InitializeFingerTracking()
    {
        if (_skeleton == null) _skeleton = _hand.GetComponent<OVRSkeleton>();

        // ������� ����� ������� ������������� ������
        foreach (var bone in _skeleton.Bones)
        {
            if (bone.Id == OVRSkeleton.BoneId.XRHand_IndexTip)
            {
                _indexTip = bone;
                break;
            }
        }

        if (_indexTip == null)
        {
            Debug.LogError("Couldn't find the index finger bone");
        }
    }

    private void Update()
    {
        if (!_hand.IsTracked || _indexTip == null) return;

        Vector3 fingerPos = _indexTip.Transform.position;

        // ��������� ������� �����������
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

    /// <summary>
    /// �������� ����� �����
    /// </summary>
    /// <param name="startPoint">��������� ����� �� �����������</param>
    private void StartNewLine(Vector3 startPoint)
    {
        // ������� ����� ������ �����, ���� ��������� �����
        if (_lines.Count >= _maxLines)
        {
            LineRenderer oldestLine = _lines[0];
            _lines.RemoveAt(0);
            _linePool.Release(oldestLine);
        }

        _currentLine = _linePool.Get();

        // ����������� ���������� ���������
        _currentLine.startColor = _currentColor;
        _currentLine.endColor = _currentColor;
        _currentLine.startWidth = _lineWidth;
        _currentLine.endWidth = _lineWidth;

        // ������������� ��������� �����
        _currentLine.positionCount = 1;
        _currentLine.SetPosition(0, startPoint);

        _lines.Add(_currentLine); // ��������� � ������ ���� �����
        _isDrawing = true;
    }

    /// <summary>
    /// ��������� ����� � ������� �����
    /// </summary>
    /// <param name="newPoint">����� ����� �� �����������</param>
    private void ContinueLine(Vector3 newPoint)
    {
        _currentLine.positionCount++;
        _currentLine.SetPosition(_currentLine.positionCount - 1, newPoint);
    }

    /// <summary>
    /// ��������� ������� �����
    /// </summary>
    private void EndLine()
    {
        _isDrawing = false;
        _currentLine = null;
    }

    public void SetColor(Color color) => _currentColor = color;

    /// <summary>
    /// ������� ��� ������������ �����
    /// </summary>
    public void ClearAll()
    {
        foreach (var line in _lines)
            _linePool.Release(line);
        _lines.Clear();
    }

    /// <summary>
    /// �������� ������ ���� ����� ��� ������������
    /// </summary>
    /// <returns>DrawingData � ������� � ������������ ���� ����� �����</returns>
    public DrawingData GetDrawingData()
    {
        var data = new DrawingData();

        foreach (var line in _lines)
        {
            var lineData = new LineData
            {
                Color = line.startColor,
                Positions = new Vector3[line.positionCount] // �������� ������
            };
            line.GetPositions(lineData.Positions); // �������� �����
            data.Lines.Add(lineData);
        }

        return data;
    }

    /// <summary>
    /// ��������������� ������� �� ����������� ������
    /// </summary>
    /// <param name="data">�������� ����� � ������� � ������������</param>
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
            line.SetPositions(lineData.Positions);

            _lines.Add(line);
        }
    }
}

/// <summary>
/// ��������� ������ ��� ������������ ����� �����
/// </summary>
[System.Serializable]
public class DrawingData
{
    public List<LineData> Lines = new();
}

/// <summary>
/// ��������� ������ ��� ����� �������
/// </summary>
[System.Serializable]
public class LineData
{
    public Color Color;
    public Vector3[] Positions;
}
