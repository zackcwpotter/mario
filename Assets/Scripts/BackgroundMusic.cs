using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    private AudioSource music;

    void Start()
    {
        music = GetComponent<AudioSource>();
        GameManager.instance.gameRestart.AddListener(RestartMusic);
    }

    // play the background music from the beginning on restart
    public void RestartMusic()
    {
        music.Stop();
        music.Play();
    }

    void OnDestroy()
    {
        if (GameManager.instance != null)
            GameManager.instance.gameRestart.RemoveListener(RestartMusic);
    }
}
