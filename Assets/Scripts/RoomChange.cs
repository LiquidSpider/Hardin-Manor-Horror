using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomChange : MonoBehaviour
{
    GameManager gm;
    public GameObject newPos;

    private void Start() {
        gm = FindObjectOfType<GameManager>();
    }

    public void Clicked() {
        gm.CameraTransition(newPos);
    }

}
