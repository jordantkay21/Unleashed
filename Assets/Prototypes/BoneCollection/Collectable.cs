using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    private int value;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            Destroy(other.gameObject);
    }
}
