using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;

    private Transform player;

    private void Start()
    {
        player = transform.parent;
    }

    private void Update()
    {
        if (player != null)
        {
            transform.RotateAround(player.position, Vector3.back, rotateSpeed * Time.deltaTime);
        }
    }
}
