using UnityEngine;
using UnityEngine.UI;

public class KeyCollector : MonoBehaviour
{
    private int Key = 0;

    public Text keyText;

    private void Start()
    {
        // Automatically find the text in the scene when the player spawns
        if (keyText == null)
        {
            GameObject textObj = GameObject.Find("KeyText");
            if (textObj != null)
            {
                keyText = textObj.GetComponent<Text>();
            }
        }
    }

    // When the player collides with a key, increment the key count and update the UI text
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Key")
        {
            Key++;
            keyText.text = "Keys: " + Key.ToString();
            Debug.Log(Key);
            Destroy(other.gameObject);
        }
    }
}
