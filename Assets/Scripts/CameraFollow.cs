using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Offset & Distance")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -7f);
    [SerializeField] private float minDistance = 4f;
    [SerializeField] private float maxDistance = 10f;

    [Header("Smoothing")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float zoomSpeed = 3f;

    [Header("Dynamic Zoom")]
    [SerializeField] private float idleZoomDelay = 2f;   // seconds before zoom-in when idle
    [SerializeField] private float idleZoomDistance = 3f;  // how close camera gets when idle
    [SerializeField] private float runZoomDistance = 12f;   // how far camera pulls when running

    private float currentZoom;
    private float targetZoom;
    private float idleTimer = 0f;
    private Vector3 lastPlayerPos;

    private void Start()
    {
        currentZoom = offset.magnitude;
        targetZoom = currentZoom;
        lastPlayerPos = player.position;
    }

    private void LateUpdate()
    {
        // detect movement state
        float playerSpeed = (player.position - lastPlayerPos).magnitude / Time.deltaTime;
        lastPlayerPos = player.position;

        bool isIdle = playerSpeed < 0.1f;
        bool isRunning = playerSpeed > 5f;

        // dynamic zoom based on player state
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > idleZoomDelay)
            {
                // slowly zoom in when idle — cinematic close-up
                targetZoom = idleZoomDistance;
            }
        }
        else if (isRunning)
        {
            idleTimer = 0f;
            // pull camera back when sprinting — sense of speed
            targetZoom = runZoomDistance;
        }
        else
        {
            idleTimer = 0f;
            // normal walking distance
            targetZoom = offset.magnitude;
        }

        targetZoom = Mathf.Clamp(targetZoom, minDistance, maxDistance);
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, zoomSpeed * Time.deltaTime);

        // calculate camera position
        Vector3 zoomDirection = offset.normalized;
        Vector3 targetPosition = player.position + zoomDirection * currentZoom;

        // smooth follow
        transform.position = Vector3.Lerp(
            transform.position, targetPosition, followSpeed * Time.deltaTime);

        // smooth look at player
        Quaternion targetRotation = Quaternion.LookRotation(
            player.position - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}