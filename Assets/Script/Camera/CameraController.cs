using UnityEngine;

public class CameraController : MonoBehaviour
{
    //* Fixed camera
    [Header("Speed")] 
    [SerializeField] private float cameraSpeed;
    
    //* Camera follow player
    [Header("Player Camera")]
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    
    private float lookAheadX;
    private float lookAheadY;
    
    private void Update()
    {
        //* Camera follow player
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
        lookAheadX = Mathf.Lerp(lookAheadX, (aheadDistance * transform.localScale.x), Time.deltaTime * cameraSpeed);
        lookAheadY = Mathf.Lerp(lookAheadY, (aheadDistance * transform.localScale.y), Time.deltaTime * cameraSpeed);
    }
}
