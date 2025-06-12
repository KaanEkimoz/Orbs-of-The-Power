using com.game;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
public class IsometricAiming : MonoBehaviour
{
    [Header("Cursor Detection")]
    [SerializeField] private LayerMask cursorDetectMask;
    private Camera _mainCamera;
    private Vector3 _hitPoint;

    [Inject] PlayerOrbController _orbController;

    private void Start()
    {
        _mainCamera = Camera.main;
    }
    private void Update()
    {
        HandleAiming();
    }
    private void HandleAiming()
    {
        if (Game.Paused) return;

        if (!_orbController.IsAiming) return;

        if (TryGetMouseWorldPosition(out var targetPosition))
            AimAtTarget(targetPosition);
    }
    private bool TryGetMouseWorldPosition(out Vector3 position)
    {
        var ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, cursorDetectMask))
        {
            position = hitInfo.point;
            _hitPoint = hitInfo.point;
            return true;
        }

        position = Vector3.zero;
        return false;
    }
    private void AimAtTarget(Vector3 targetPosition)
    {
        var direction = targetPosition - transform.position;
        direction.y = 0; // Keep the direction on the XZ plane
        transform.forward = direction.normalized;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_hitPoint, 0.5f);
    }
#endif
}