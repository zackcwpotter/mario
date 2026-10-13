using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public float waitTime = 3f;

    void Start()
    {
        StartCoroutine(LoadRoutine());
    }

    IEnumerator LoadRoutine()
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("World 1-1");
    }
}