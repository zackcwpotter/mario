using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventIntTool : MonoBehaviour
{
    public int parameter;
    public UnityEvent<int> useInt;

    void Start()
    {
    }

    void Update()
    {
    }

    public void TriggerIntEvent()
    {
        useInt.Invoke(parameter);
    }
}