using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventIntTool : MonoBehaviour
{
    public int parameter;
    public UnityEvent<int> useInt;
    
    private bool hasTriggered = false;

    // This resets the score trigger every time the coin pops out of a block
    void OnEnable()
    {
        hasTriggered = false;
    }

    public void TriggerIntEvent()
    {
        // Only award points if it hasn't given one yet during this specific popup
        if (!hasTriggered)
        {
            useInt.Invoke(parameter);
            hasTriggered = true;
        }
    }
}