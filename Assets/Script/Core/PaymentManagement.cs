using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PaymentManagement : MonoBehaviour
{
    [SerializeField] public string playerName = "kleqing";

    private HealthBar healthBar;
    private void Awake()
    {
        healthBar = FindFirstObjectByType<Player>().GetComponent<HealthBar>();
    }
    public void BuyRevive()
    {
        StartCoroutine(SendReviveRequest());
    }

    public void BuyMeleeWeapon()
    {
        StartCoroutine(SendUnlockWeaponRequest("melee"));
    }
    IEnumerator SendReviveRequest()
    {
        string url = $"https://localhost:7028/api/Payment/revive?playerName={playerName}";
        ReviveRequest req = new ReviveRequest { description = "Revive player" };
        string jsonBody = JsonUtility.ToJson(req);
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var json = request.downloadHandler.text;
            string paymentUrl = JsonUtility.FromJson<PaymentUrlResponse>(json).url;

            Application.OpenURL(paymentUrl);
        }
        else
        {
            Debug.LogError("Payment error: " + request.error);
            Debug.LogError("Detail: " + request.downloadHandler.text);
        }
    }

    public IEnumerator CheckPremium(System.Action<bool> callback)
    {
        string url = $"https://localhost:7028/api/Payment/status?playerName={playerName}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var json = www.downloadHandler.text;
                bool isPaid = JsonUtility.FromJson<StatusResponse>(json).isPaid;
                callback.Invoke(isPaid);
            }
            else
            {
                Debug.LogError("Status error: " + www.error);
                callback.Invoke(false);
            }
        }
    }

    IEnumerator SendUnlockWeaponRequest(string weaponType)
    {
        string url = $"https://localhost:7028/api/Payment/weapon?playerName={playerName}&weaponType={weaponType}";

        ReviveRequest req = new ReviveRequest { description = $"Unlock {weaponType} weapon" };
        string jsonBody = JsonUtility.ToJson(req);
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var json = request.downloadHandler.text;
            string paymentUrl = JsonUtility.FromJson<PaymentUrlResponse>(json).url;
            Application.OpenURL(paymentUrl);
        }
        else
        {
            Debug.LogError("Unlock weapon failed: " + request.error);
        }
    }

    public IEnumerator CheckPaymentStatus(string playerName)
    {
        string url = $"https://localhost:7028/api/payment/status?playerName={playerName}";
        while (true)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    var json = www.downloadHandler.text;
                    if (json.Contains("\"isPaid\":true"))
                    {
                        Debug.Log("Payment success, revive player!");
                        var uiManager = FindAnyObjectByType<UIManager>();
                        if (uiManager != null)
                        {
                            uiManager.Revive();
                            uiManager.SendMessage("CloseGameOverUI");
                        }
                        else
                        {
                            Debug.LogWarning("UI Manager not found.");
                        }
                        yield break;
                    }
                    else if (json.Contains("\"isPaid\":false"))
                    {
                        Debug.Log("Please pay for me to revive huhu!");
                    }
                }

            }
            yield return new WaitForSeconds(3);
        }
    }

    public IEnumerator CheckWeaponUnlocked(string weaponType, System.Action<bool> callback)
    {
        string url = $"https://localhost:7028/api/Payment/status?playerName={playerName}_{weaponType}";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                var json = www.downloadHandler.text;
                bool isPaid = JsonUtility.FromJson<StatusResponse>(json).isPaid;
                callback.Invoke(isPaid);
            }
            else
            {
                Debug.LogError("Check unlock status error: " + www.error);
                callback.Invoke(false);
            }
        }
    }


    [System.Serializable]
    public class PaymentUrlResponse
    {
        public string url;
    }

    [System.Serializable]
    public class StatusResponse
    {
        public bool isPaid;
    }

    [System.Serializable]
    public class ReviveRequest
    {
        public string description;
    }
}
