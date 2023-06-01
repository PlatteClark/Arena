#nullable enable

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider))]
public class Interactable : MonoBehaviour
{
    public GameObject interactionPrompt;

    public delegate void InteractEvent(List<string> keys);
    public event InteractEvent OnInteract;

    public delegate void PlayerLeftTriggerAreaEvent();
    public event PlayerLeftTriggerAreaEvent OnPlayerLeftTriggerArea;

    public bool Interact(List<string> keys)
    {
        OnInteract?.Invoke(keys);

        return true;
    }


    private void Start()
    {
        interactionPrompt.SetActive(false);
    }

    protected virtual void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactionPrompt.SetActive(false);
            OnPlayerLeftTriggerArea?.Invoke();
        }
    }
}

