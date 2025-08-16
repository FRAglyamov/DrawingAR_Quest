using UnityEngine;

/// <summary>
/// Компонент для управления рисованием одной руки
/// </summary>
public class HandDrawingController : MonoBehaviour
{
    [Header("Hand References")]
    [SerializeField] private OVRHand _hand;
    [SerializeField] private OVRSkeleton _skeleton;

    [Header("Dependencies")]
    [SerializeField] private DrawingManager _drawingManager;

    private LineRenderer _currentLine;
    private OVRBone _indexTip;
    private bool _isDrawing;
    private Vector3 _lastPoint;
    private const float MIN_DISTANCE = 0.003f; // Минимальное расстояние между точками линии


    private void Start()
    {
        if (_hand == null || _drawingManager == null)
        {
            Debug.LogError("Dependencies not assigned!", this);
            enabled = false;
            return;
        }

        InitializeFingerTracking();
    }

    /// <summary>
    /// Установка ссылок костей руки
    /// </summary>
    private void InitializeFingerTracking()
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

        if (_indexTip == null)
        {
            Debug.LogError("Couldn't find the index finger bone", this);
        }
    }

    private void Update()
    {
        // Пропускаем кадр, если рука не отслеживается или не найден кончик пальца
        if (!_hand.IsTracked || _indexTip == null) return;

        Vector3 fingerPos = _indexTip.Transform.position;

        if (_drawingManager.IsTouchingSurface(fingerPos, out Vector3 surfacePoint))
        {
            if (!_isDrawing)
            {
                StartNewLine(surfacePoint);
                _lastPoint = surfacePoint;
            }
            else if (Vector3.Distance(_lastPoint, surfacePoint) > MIN_DISTANCE)
            {
                ContinueLine(surfacePoint);
                _lastPoint = surfacePoint;
            }
        }
        else if (_isDrawing)
        {
            EndLine();
        }
    }

    /// <summary>
    /// Начинает новую линию для этой руки
    /// </summary>
    /// <param name="startPoint">Начальная точка линии на поверхности</param>
    private void StartNewLine(Vector3 startPoint)
    {
        _currentLine = _drawingManager.CreateNewLine(startPoint);
        _isDrawing = true;
    }

    /// <summary>
    /// Добавляет новую точку к текущей линии этой руки
    /// </summary>
    /// <param name="newPoint">Начальная точка линии на поверхности</param>
    private void ContinueLine(Vector3 newPoint)
    {
        _currentLine.positionCount++;
        _currentLine.SetPosition(_currentLine.positionCount - 1, newPoint);
    }

    /// <summary>
    /// Завершает текущую линию этой руки
    /// </summary>
    private void EndLine()
    {
        _isDrawing = false;
        _currentLine = null;
    }
}
