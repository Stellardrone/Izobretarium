using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private int sceneIndex;

    // Ётот метод можно прив€зать к кнопке в инспекторе
    public void LoadSceneByIndex()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
