#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class KeyController : MonoBehaviour
{
    public string keyName = "Key Name";

    [SerializeField] private Interactable interactable;
    [SerializeField] private AudioSource? audioSource = null;
    [SerializeField] TMP_Text messageText;
    [SerializeField] GameObject interactText;
    

    private void Start()
    {
        interactable.OnInteract += OnInteract;
        interactable.OnPlayerLeftTriggerArea += OnPlayerLeftTriggerArea;
        messageText.text = "";
    }

    private void OnDestroy()
    {
        interactable.OnInteract -= OnInteract;
        interactable.OnPlayerLeftTriggerArea -= OnPlayerLeftTriggerArea;
    }

    private void OnInteract(List<string> keys)
    {
        keys.Add(keyName);
        messageText.text = "Picked up " + keyName;
        audioSource.Play();

        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers) 
        {
            renderer.enabled = false;
        }

        StartCoroutine(HideTextAfterTime(4.0f));
    }

    private void OnPlayerLeftTriggerArea()
    {
        //
    }

    IEnumerator HideTextAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        messageText.text = "";
    }
}

