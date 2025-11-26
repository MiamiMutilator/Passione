using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;

public class Rage : ToggleableBehaviour
{
    [SerializeField] int maxRage = 100;
    public int rageAmount = 100;
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
        if (rageInput.action.triggered && rageAmount == 100)
        {
            Debug.Log("Enraged!");
            enraged = true;
            StartCoroutine(RageDown());
        }
        if (rageAmount == 0)
        {
            enraged = false;
            Debug.Log("No longer enraged");
            StopCoroutine(RageDown());
            //StartCoroutine(RageUp());
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
        if(passionSlider) passionSlider.value = rageAmount;
    }

    private IEnumerator RageDown()
    {
        while (enraged == true && rageAmount > 0)
        {
            rageAmount--;
            yield return new WaitForSeconds(0.5f);
        }
    }
    private IEnumerator RageUp()
    {
        while (enraged == false && rageAmount < 100f)
        {
            rageAmount++;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void IncreaseRage(int amount)
    {
        // ensure rage doesn't go over maximum
        if ((rageAmount + amount) > maxRage)
        {
            Debug.Log($"Rage increased by {maxRage - rageAmount}. Current rage: {maxRage}");
            rageAmount = maxRage;
        }
        else
        {
            rageAmount += amount;
            Debug.Log($"Rage increased by {amount}. Current rage: {rageAmount}");
        }
        
    }

}
