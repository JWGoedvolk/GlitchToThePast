using System.Collections.Generic;
using UnityEngine;

public class GameObjectsCountDetector : MonoBehaviour
{
    // Quick plan:
    // Track how many players (gameobjects in this context) are inside of specificx roons.
    // Attach this to each rom that needs to determine a count.

    public List<GameObject> gameObjectsInScene = new List<GameObject>();

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !gameObjectsInScene.Contains(other.gameObject))
        {
            gameObjectsInScene.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (gameObjectsInScene.Contains(other.gameObject))
            {
                gameObjectsInScene.Remove(other.gameObject);
            }
        }
    }
}
