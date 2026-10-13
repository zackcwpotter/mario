using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public TMP_Text livesText;
    public float waitTime = 3f;

    void Start()
    {
        //livesText.text = "x " + GameData.Lives; --todo
        StartCoroutine(LoadRoutine());
    }

    IEnumerator LoadRoutine()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("World 1-1");
    }
}