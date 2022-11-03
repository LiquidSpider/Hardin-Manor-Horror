using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    public GameObject key;
    public GameObject newPos;
    public AudioClip unlockSound;
    public AudioClip lockedSound;

    bool isUnlocked = false;

    [SerializeField]
    string LockedDesc = "The handle turns but the door won't move.";

    public void Clicked() {
        if (isUnlocked) {
            FindObjectOfType<GameManager>().CameraTransition(newPos);
        }
        else {
            if (FindObjectOfType<GameManager>().inventory.Contains(key)) {
                FindObjectOfType<GameManager>().PlaySound(unlockSound);
                isUnlocked = true;
                FindObjectOfType<GameManager>().MessagePlayer("I unlocked the deadbolt.");
                FindObjectOfType<GameManager>().CameraTransition(newPos);
            }
            else {
                FindObjectOfType<GameManager>().MessagePlayer(LockedDesc);
                FindObjectOfType<GameManager>().PlaySound(lockedSound);
            }
        }
    }
}
