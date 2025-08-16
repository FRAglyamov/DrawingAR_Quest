using UnityEngine;

/// <summary>
/// Определяет, касается ли палец виртуальной поверхности для рисования
/// </summary>
public class SurfaceDetector : MonoBehaviour
{
    [Tooltip("Ссылка на трансформ поверхности для рисования (виртуальный лист бумаги)")]
    [SerializeField] private Transform _paperSurface;

    [Tooltip("Допустимое расстояние от поверхности для рисования")]
    [SerializeField] private float _surfaceOffset = 0.01f;

    [Tooltip("Размер области рисования (ширина и высота в локальных координатах)")]
    [SerializeField] private Vector2 _drawingAreaSize = new Vector2(0.5f, 0.5f);

    private void Start()
    {
        if(_paperSurface == null) _paperSurface = GetComponent<Transform>();
    }

    /// <summary>
    /// Проверяет, находится ли точка в пределах поверхности для рисования
    /// </summary>
    /// <param name="position">Мировая позиция для проверки</param>
    /// <param name="surfacePoint">Выходная позиция на поверхности</param>
    /// <returns>True если точка над поверхностью</returns>
    public bool IsTouchingSurface(Vector3 position, out Vector3 surfacePoint)
    {
        Vector3 localPos = _paperSurface.InverseTransformPoint(position);

        // Проверяем, находится ли точка в пределах плоскости
        if (Mathf.Abs(localPos.z) < _surfaceOffset &&
            Mathf.Abs(localPos.x) <= _drawingAreaSize.x &&
            Mathf.Abs(localPos.y) <= _drawingAreaSize.y)
        {
            localPos.z = 0; // Проецируем на поверхность
            surfacePoint = _paperSurface.TransformPoint(localPos);
            return true;
        }

        surfacePoint = Vector3.zero;
        return false;
    }
}
