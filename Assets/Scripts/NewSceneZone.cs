using UnityEngine;
using UnityEngine.SceneManagement;

public class NewSceneZone : MonoBehaviour
{

    int currentSceneIndex;
    
    void Awake(){
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    } 

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if player contact, switch to next scene
        if (other.CompareTag("Player"))
        {
            currentSceneIndex += 1;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }
}