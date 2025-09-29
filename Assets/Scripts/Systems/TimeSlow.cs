using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TimeSlow : IActivateable
{
    private readonly float slowedTimeScale;
    private readonly float duration;

    private bool activated;

    public TimeSlow(float slowedTimeScale, float duration)
    {
        this.duration = duration;
        this.slowedTimeScale = slowedTimeScale;
    }

    public void OnActivation()
    {
        Debug.Log("Time Slow activated");
        if (!activated)
        {
            Time.timeScale = slowedTimeScale;
            activated = true;
        }
    }

    public void Deactivate()
    {
        Debug.Log("Time Slow deactivated");
        activated = false;
        Time.timeScale = 1;
    }
}
