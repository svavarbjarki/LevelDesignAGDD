using UnityEngine;
using AGDDPlatformer;
public class Spikes : MonoBehaviour
{
    public GameManager gameManager;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player1"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                gameManager.ResetLevel();
            }
        }
    }
}
