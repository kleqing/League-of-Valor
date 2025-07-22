using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float cameraSpeed;

    private Transform player;
    [SerializeField] private float aheadDistance;

    private float lookAheadX;
    private float lookAheadY;

    private void Update()
    {
        if (player == null) return;

        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        lookAheadX = Mathf.Lerp(lookAheadX, (aheadDistance * transform.localScale.x), Time.deltaTime * cameraSpeed);
        lookAheadY = Mathf.Lerp(lookAheadY, (aheadDistance * transform.localScale.y), Time.deltaTime * cameraSpeed);
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }
}
