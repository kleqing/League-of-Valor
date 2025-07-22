using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public int Coin;
    public int Item1;
    public Text Price;
    void Start()
    {
        Coin = PlayerPrefs.GetInt("Coin", 0); // Load coin amount from PlayerPrefs
        Coin = 500; // Example starting coin amount
        Item1 = 200; // Example item price
        Price.text = "Price: " + Item1 + " coins";
        PlayerPrefs.SetInt("Coin", Coin); // Save initial coin amount

    }
    public void Buy()
    {
        if(Coin >= Item1)
        {
            Coin -= Item1;
            Price.text = "Purchased!";
        }
        else
        {
            Price.text = "Not enough coins!";
        }
    }

}
