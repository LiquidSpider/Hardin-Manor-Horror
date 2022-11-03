using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameManager gm;
    [SerializeField]
    Image imgInteract;
    [SerializeField]
    Image imgExamine;
    [SerializeField]
    Image iconExamine;
    [SerializeField]
    Text textExamine;

    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (gm.currentHand) {
            case GameManager.HandMode.Interact:
                imgInteract.enabled = true;
                imgExamine.enabled = false;
                break;
            case GameManager.HandMode.Examine:
                imgInteract.enabled = false;
                imgExamine.enabled = true;
                break;
            case GameManager.HandMode.UseItem:

                break;
        }
    }

    public void ShowText(bool toggle) {
        if (toggle) {
            iconExamine.enabled = false;
            textExamine.enabled = true;
        }
        else {
            iconExamine.enabled = true;
            textExamine.enabled = false;
        }
    }
}
