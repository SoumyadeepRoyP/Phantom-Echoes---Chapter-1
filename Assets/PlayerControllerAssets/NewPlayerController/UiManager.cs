using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UiManager : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] Slider staminaSlider;
    [SerializeField] float maxStamina = 100f;
    [SerializeField] float staminaDrainRate = 15f;
    [SerializeField] float staminaRegenRate = 10f;
    [SerializeField] float regenDelay = 1.5f;
    [SerializeField] float updateInterval = 0.1f;

    private float currentStamina;
    private bool isSprinting;
    private bool isRegenerating;
    private Coroutine staminaCoroutine;
    private WaitForSeconds updateWait;

    void Start()
    {
        InitializeStamina();
        updateWait = new WaitForSeconds(updateInterval);
    }

    void InitializeStamina()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = maxStamina;
    }

    // Call this from other scripts to control stamina drain
    public void SetSprinting(bool sprinting)
    {
        if (isSprinting == sprinting) return;

        isSprinting = sprinting;

        if (staminaCoroutine != null)
        {
            StopCoroutine(staminaCoroutine);
            staminaCoroutine = null;
        }

        if (isSprinting)
        {
            // Start draining stamina
            staminaCoroutine = StartCoroutine(DrainStamina());
        }
        else
        {
            // Start regeneration after delay
            staminaCoroutine = StartCoroutine(DelayedRegen());
        }
    }

    IEnumerator DrainStamina()
    {
        isRegenerating = false;
        
        while (isSprinting && currentStamina > 0)
        {
            currentStamina -= staminaDrainRate * updateInterval;
            currentStamina = Mathf.Max(currentStamina, 0);
            staminaSlider.value = currentStamina;
            
            if (currentStamina <= 0)
            {
                OnStaminaDepleted();
                yield break; // Exit coroutine when depleted
            }
            
            yield return updateWait;
        }
    }

    IEnumerator DelayedRegen()
    {
        isRegenerating = false;
        
        // Wait before starting regeneration
        yield return new WaitForSeconds(regenDelay);
        
        // Start regenerating
        staminaCoroutine = StartCoroutine(RegenerateStamina());
    }

    IEnumerator RegenerateStamina()
    {
        isRegenerating = true;
        
        while (!isSprinting && currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * updateInterval;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
            staminaSlider.value = currentStamina;
            yield return updateWait;
        }
        
        isRegenerating = false;
    }

    void OnStaminaDepleted()
    {
        // Notify other systems that stamina is depleted
        Debug.Log("Stamina depleted!");
    }

    public bool CanSprint()
    {
        return currentStamina > staminaDrainRate * updateInterval;
    }

    public bool IsStaminaDepleted()
    {
        return currentStamina <= 0;
    }
}