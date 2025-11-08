using System;
using Unity.VisualScripting;
using UnityEngine;


public class TimerManager : Singleton<TimerManager>
{
    public static Action<float> OnUpdate;

    private void Update()
    {
        OnUpdate?.Invoke(Time.deltaTime);
    }
}
