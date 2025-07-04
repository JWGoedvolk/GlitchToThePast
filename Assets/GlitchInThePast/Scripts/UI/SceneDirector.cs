using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
    public void TakePlayerToScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}