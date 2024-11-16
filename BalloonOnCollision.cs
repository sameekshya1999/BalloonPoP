using UnityEngine;
using UnityEngine.UI;

public class BalloonOnCollision : MonoBehaviour
{
    public AudioSource balloonPopSound;  
    public AudioSource dangerSound;      
    private GameMaster gameMaster;       

    private void Start()
    {
        gameMaster = FindObjectOfType<GameMaster>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("HIT");

            // Increase the score in GameMaster
            if (gameMaster != null)
            {
                gameMaster.IncreaseScore(1);
            }

            // Destroy the bullet and the balloon
            Destroy(collision.gameObject);  
            Destroy(gameObject);            
        }
        // Check if the player collided with the balloon
        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit balloon - Score Decreased");

            // Decrease the score in GameMaster
            if (gameMaster != null)
            {
                gameMaster.DecreaseScore(1); 
            }


            // Check if the score is less than or equal to -5 and disable the player
            if (gameMaster != null && gameMaster.GetScore() <= -1)
            {

                Debug.Log("Player disabled - Score reached -1");
                collision.gameObject.SetActive(false); // Disable the player

            }
        }
        else
        {
            Debug.Log("NO HIT"); 
        }
    }

}

    
