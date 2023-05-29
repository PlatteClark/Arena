#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class DoorController : MonoBehaviour
{
    [SerializeField] private Interactable interactable;
    [SerializeField] private Animator? animator = null;
    [SerializeField] private AudioSource? audioSource = null;
    [SerializeField] private Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();

    [SerializeField] private List<string> messages = new List<string>();
    [SerializeField] TMP_Text messageText;
    [SerializeField] private AudioClip[] soundClips;

    public string keyRequired = "key name";

    private void Start()
    {
        interactable.OnInteract += OnInteract;
        interactable.OnPlayerLeftTriggerArea += OnPlayerLeftTriggerArea;
        messageText.text = "";

        for (int i = 0; i < soundClips.Length; i++)
        {
            sounds[soundClips[i].name] = soundClips[i];
        }
    }

    private void OnDestroy()
    {
        interactable.OnInteract -= OnInteract;
        interactable.OnPlayerLeftTriggerArea -= OnPlayerLeftTriggerArea;
    }

    private void OnInteract(List<string> keys)
    {

        if(keys.Contains(keyRequired))
        {
            animator?.SetTrigger("HasKey");
            audioSource.PlayOneShot(sounds.ElementAt(1).Value);
            messageText.text = messages[1];
        }
        else
        {
            audioSource.PlayOneShot(sounds.ElementAt(0).Value);
            messageText.text = messages[0];
        }

        StartCoroutine(HideTextAfterTime(2.0f));
    }


    private void OnPlayerLeftTriggerArea()
    {
        //
    }

    IEnumerator HideTextAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        messageText.text = "";
    }
}

