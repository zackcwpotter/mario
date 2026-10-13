using UnityEngine;

public class HomeButtonController : MonoBehaviour, IInteractiveButton
{
    public void ButtonClick()
    {
        Debug.Log("Onclick home button");
        GameManager.instance.ReturnToMenu();
    }
}
