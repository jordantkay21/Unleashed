using UnityEngine;
using TMPro;

public class CollectionManager : MonoBehaviour
{
    [SerializeField]
    private int score = 0;
    public TMP_Text scoreText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bone"))
        {
            score++;
            scoreText.text = $"Score: {score}";
            Destroy(other.gameObject);
        }
    }
}
