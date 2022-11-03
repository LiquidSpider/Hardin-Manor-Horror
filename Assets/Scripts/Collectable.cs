using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public AudioClip pickupSound;
    [SerializeField]
    string pickupText = "Picked up a key.";

    public void Clicked() {
        FindObjectOfType<GameManager>().PlaySound(pickupSound);
        FindObjectOfType<GameManager>().MessagePlayer(pickupText);
        FindObjectOfType<GameManager>().AddItem(gameObject);
        gameObject.SetActive(false);
    }
}
