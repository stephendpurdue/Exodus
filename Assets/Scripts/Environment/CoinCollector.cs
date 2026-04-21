using UnityEngine;
using UnityEngine.UI;

public class CoinCollector : MonoBehaviour
{
    private int Coin = 0;

    public Text coinText;

    private void Start()
    {
        // Automatically find the text in the scene when the player spawns
        if (coinText == null)
        {
            GameObject textObj = GameObject.Find("CoinText"); 
            if (textObj != null)
            {
                coinText = textObj.GetComponent<Text>();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Coin")
        {
            Coin++;
            coinText.text = "Coins: " + Coin.ToString();
            Debug.Log(Coin);
            Destroy(other.gameObject);
        }
    }
}
