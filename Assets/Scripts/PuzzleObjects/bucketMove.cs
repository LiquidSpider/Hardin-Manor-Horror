using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bucketMove : MonoBehaviour
{

    bool hasMoved = false;

    public void Clicked() {
        if (!hasMoved) {
            GetComponent<Animation>().Play();
            hasMoved = true;
        }
    }
}
