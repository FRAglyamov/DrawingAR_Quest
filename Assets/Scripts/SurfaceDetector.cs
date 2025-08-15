using UnityEngine;

/// <summary>
/// ќпредел€ет, касаетс€ ли палец виртуальной поверхности дл€ рисовани€
/// </summary>
public class SurfaceDetector : MonoBehaviour
{
    [Tooltip("—сылка на трансформ поверхности дл€ рисовани€ (виртуальный лист бумаги)")]
    [SerializeField] private Transform _paperSurface;

    [Tooltip("ƒопустимое рассто€ние от поверхности дл€ рисовани€")]
    [SerializeField] private float _surfaceOffset = 0.01f;

    [Tooltip("–азмер области рисовани€ (ширина и высота в локальных координатах)")]
    [SerializeField] private Vector2 _drawingAreaSize = new Vector2(0.5f, 0.5f);

    private void Start()
    {
        if(_paperSurface == null) _paperSurface = GetComponent<Transform>();
    }

    /// <summary>
    /// ѕровер€ет, находитс€ ли точка в пределах поверхности дл€ рисовани€
    /// </summary>
    /// <param name="position">ћирова€ позици€ дл€ проверки</param>
    /// <param name="surfacePoint">¬ыходна€ позици€ на поверхности</param>
    /// <returns>True если точка над поверхностью</returns>
    public bool IsTouchingSurface(Vector3 position, out Vector3 surfacePoint)
    {
        Vector3 localPos = _paperSurface.InverseTransformPoint(position);

        // ѕровер€ем, находитс€ ли точка в пределах плоскости
        if (Mathf.Abs(localPos.z) < _surfaceOffset &&
            Mathf.Abs(localPos.x) <= _drawingAreaSize.x &&
            Mathf.Abs(localPos.y) <= _drawingAreaSize.y)
        {
            localPos.z = 0; // ѕроецируем на поверхность
            surfacePoint = _paperSurface.TransformPoint(localPos);
            return true;
        }

        surfacePoint = Vector3.zero;
        return false;
    }
}
