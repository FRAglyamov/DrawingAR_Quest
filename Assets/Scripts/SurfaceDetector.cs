using UnityEngine;

/// <summary>
/// ����������, �������� �� ����� ����������� ����������� ��� ���������
/// </summary>
public class SurfaceDetector : MonoBehaviour
{
    [Tooltip("������ �� ��������� ����������� ��� ��������� (����������� ���� ������)")]
    [SerializeField] private Transform _paperSurface;

    [Tooltip("���������� ���������� �� ����������� ��� ���������")]
    [SerializeField] private float _surfaceOffset = 0.01f;

    [Tooltip("������ ������� ��������� (������ � ������ � ��������� �����������)")]
    [SerializeField] private Vector2 _drawingAreaSize = new Vector2(0.5f, 0.5f);

    private void Start()
    {
        if(_paperSurface == null) _paperSurface = GetComponent<Transform>();
    }

    /// <summary>
    /// ���������, ��������� �� ����� � �������� ����������� ��� ���������
    /// </summary>
    /// <param name="position">������� ������� ��� ��������</param>
    /// <param name="surfacePoint">�������� ������� �� �����������</param>
    /// <returns>True ���� ����� ��� ������������</returns>
    public bool IsTouchingSurface(Vector3 position, out Vector3 surfacePoint)
    {
        Vector3 localPos = _paperSurface.InverseTransformPoint(position);

        // ���������, ��������� �� ����� � �������� ���������
        if (Mathf.Abs(localPos.z) < _surfaceOffset &&
            Mathf.Abs(localPos.x) <= _drawingAreaSize.x &&
            Mathf.Abs(localPos.y) <= _drawingAreaSize.y)
        {
            localPos.z = 0; // ���������� �� �����������
            surfacePoint = _paperSurface.TransformPoint(localPos);
            return true;
        }

        surfacePoint = Vector3.zero;
        return false;
    }
}
