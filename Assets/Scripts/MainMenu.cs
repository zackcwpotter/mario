using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        //GameData.Lives = 3;
        SceneManager.LoadScene("Loading Scene");
    }

    public void ResetHighscore()
    {
        Debug.Log("Highscore nullstilt");

        GameObject eventSystem = GameObject.Find("EventSystem");
        eventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }
}