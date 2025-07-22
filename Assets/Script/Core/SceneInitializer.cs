using UnityEngine;

public class SceneInitializer : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    private void Start()
    {
        Debug.Log("Selected weapon: " + GameManager.Instance.selectedWeapon);

        GameObject playerPrefab = GameManager.Instance.GetSelectedPlayerPrefab();
        GameObject player = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);

        // Gán cho Camera follow đúng player
        Camera.main.GetComponent<CameraController>().SetTarget(player.transform);
    }
}
