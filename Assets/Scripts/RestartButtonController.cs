using UnityEngine;

public class RestartButtonController : MonoBehaviour, IInteractiveButton
{
    public void ButtonClick()
    {
        Debug.Log("Onclick restart button");
        GameManager.instance.GameRestart();
    }
}