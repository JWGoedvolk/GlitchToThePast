using UnityEngine;

public class ObjectsActivenessToggler : MonoBehaviour
{
    [SerializeField] private GameObjectsCountDetector[] detectors;

    private void Update()
    {
        for (int roomIndex = 0; roomIndex < detectors.Length; roomIndex++)
        {
            foreach (GameObject player in detectors[roomIndex].gameObjectsInScene)
            {
                if (player == null) continue;

                foreach (Transform child in player.transform)
                {
                    child.gameObject.SetActive(false);
                }

                if (roomIndex < player.transform.childCount)
                {
                    player.transform.GetChild(roomIndex).gameObject.SetActive(true);
                }
            }
        }
    }
}