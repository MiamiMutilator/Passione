using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class Rage : MonoBehaviour
{
    public int RageMeter = 100;
    public bool enraged;
    public InputActionReference rageInput;
    public Slider passionSlider;

    private void OnEnable()
    {
        rageInput.action.Enable();
    }
    private void OnDisable()
    {
        rageInput.action.Disable();
    }

    void Update()
    {
        if (rageInput.action.triggered && RageMeter == 100)
        {
            Debug.Log("Enraged!");
            enraged = true;
            StartCoroutine(RageDown());
        }
        if (RageMeter == 0)
        {
            enraged = false;
            Debug.Log("No longer enraged");
            StopCoroutine(RageDown());
            StartCoroutine(RageUp());
        }

        //rage buffs
        if (enraged == true)
        {
            //punches do more damage
            //different punch animations
        }
        else if (enraged == false)
        {
            //punches return to normal damage
            //punch animations return to previous normal animations
        }
        if(passionSlider) passionSlider.value = RageMeter;
    }

    private IEnumerator RageDown()
    {
        while (enraged == true && RageMeter > 0)
        {
            RageMeter--;
            yield return new WaitForSeconds(0.5f);
        }
    }
    private IEnumerator RageUp()
    {
        while (enraged == false && RageMeter < 100f)
        {
            RageMeter++;
            yield return new WaitForSeconds(0.5f);
        }
    }

}
