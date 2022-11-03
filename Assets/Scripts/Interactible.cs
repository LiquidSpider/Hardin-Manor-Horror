using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactible : MonoBehaviour
{
    [SerializeField]
    public string description;
    [SerializeField]
    public string interactibleName;


    public void GetDesc() {
        FindObjectOfType<GameManager>().MessagePlayer(description);
    }

    public void GetName() {
        FindObjectOfType<GameManager>().MessagePlayer(interactibleName, true);
    }
}
