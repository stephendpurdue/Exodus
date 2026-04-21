using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    private int Coin = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Coin")
        {
            Coin++;
            Debug.Log(Coin);
            Destroy(other.gameObject);
        }
    }
}
