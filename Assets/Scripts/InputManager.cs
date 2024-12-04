using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();

    private void Awake()
    {
        // Singleton pattern to ensure only one InputManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowHealthBar(GameObject healthBar, float duration)
    {
        // If there's already a coroutine for this health bar, stop it
        if (activeCoroutines.ContainsKey(healthBar))
        {
            StopCoroutine(activeCoroutines[healthBar]);
            activeCoroutines.Remove(healthBar);
        }

        // Start a new coroutine and store it in the dictionary
        Coroutine coroutine = StartCoroutine(ShowHealthBarCoroutine(healthBar, duration));
        activeCoroutines[healthBar] = coroutine;
    }

    private IEnumerator ShowHealthBarCoroutine(GameObject healthBar, float duration)
    {
        healthBar.SetActive(true); // Show the health bar
        yield return new WaitForSeconds(duration); // Wait for the specified time
        healthBar.SetActive(false); // Hide the health bar

        // Remove the coroutine from the dictionary when done
        if (activeCoroutines.ContainsKey(healthBar))
        {
            activeCoroutines.Remove(healthBar);
        }
    }
}
