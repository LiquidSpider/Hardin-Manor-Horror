using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusButton : MonoBehaviour
{

    private void Start() {
        
    }

    public void ReportStatus() {
        FindObjectOfType<GameManager>().CheckStatus();
    }

}
