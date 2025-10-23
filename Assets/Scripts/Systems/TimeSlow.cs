using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TimeSlow : IActivateable
{
    private readonly float slowedTimeScale;

    public bool Activated { get; private set; }

    public TimeSlow(float slowedTimeScale)
    {
        this.slowedTimeScale = slowedTimeScale;
    }

    public void OnActivation()
    {
        Debug.Log("Time Slow activated");
        if (!Activated)
        {
            Time.timeScale = slowedTimeScale;
            Activated = true;
        }
    }

    public void Deactivate()
    {
        Debug.Log("Time Slow deactivated");
        Activated = false;
        Time.timeScale = 1;
    }
}
