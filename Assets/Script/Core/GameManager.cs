using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum WeaponType { Melee, Range }
    public WeaponType selectedWeapon;

    [Header("Prefabs")]
    public GameObject meleePlayerPrefab;
    public GameObject rangePlayerPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetSelectedPlayerPrefab()
    {
        return selectedWeapon == WeaponType.Melee ? meleePlayerPrefab : rangePlayerPrefab;
    }
}
