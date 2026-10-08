using UnityEngine;

public class RestartButtonController : MonoBehaviour
{
    public void ButtonClick()
    {
        GameManager.instance.GameRestart();
    }
}