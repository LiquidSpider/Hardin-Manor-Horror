using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    GameManager gm;
    public enum ButtonType {
        Mode,
        Item,
        Settings
    }
    public GameManager.HandMode mode;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMode() {
        Debug.Log("Received change order");
        gm.ChangeMode(mode);
    }
}
