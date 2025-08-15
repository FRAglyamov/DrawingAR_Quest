using UnityEngine;

public class SurfaceDetector : MonoBehaviour
{
    [SerializeField] private Transform _paperSurface;
    [SerializeField] private float _surfaceOffset = 0.01f;
    [SerializeField] private Vector2 _drawingAreaSize = new Vector2(0.5f, 0.5f);

    private void Start()
    {
        if(_paperSurface == null) _paperSurface = GetComponent<Transform>();
    }

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
