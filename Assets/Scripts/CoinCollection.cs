using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CoinCollection : MonoBehaviour
{
    private int Coin = 0;

    [Header("Message UI")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject messageGroupObj;

    public static List<GameObject> collectedCoins = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin") && other.gameObject.activeSelf)
        {
            Coin++;
            if (!collectedCoins.Contains(other.gameObject)) {
                collectedCoins.Add(other.gameObject);
                Debug.Log("Coin added to collectedCoins list: " + other.gameObject.name);
            }

            other.gameObject.SetActive(false);

            if (messageText != null && messageGroupObj != null)
            {
                StartCoroutine(ShowScaleMessage());
            }
        }
    }

    private IEnumerator ShowScaleMessage()
    {
        messageText.text = "You saved 1 second!";
        messageGroupObj.transform.localScale = Vector3.zero;
        messageGroupObj.SetActive(true);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 4f;
            messageGroupObj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }

        yield return new WaitForSeconds(1.2f);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 4f;
            messageGroupObj.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);
            yield return null;
        }

        messageGroupObj.SetActive(false);
    }
}
