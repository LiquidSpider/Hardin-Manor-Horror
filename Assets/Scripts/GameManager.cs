using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour {
    public RenderTexture mainView;
    public RectTransform viewPort;
    public Canvas mainUI;
    public Text messageBox;
    public RectTransform debugMousePointer;
    public Volume postprocessing;
    public float shadowStrength = 1;
    private LiftGammaGain lGG;
    private ShadowsMidtonesHighlights SMH;
    private Vector4 SMHOn;
    private Vector4 SMHOff;
    private float gainLerp = 0;
    [SerializeField]
    private float gainLerpSpeed = 0.5f;
    Vector4 gain;
    private GameObject nextCamera;

    // Every time the player moves location, the timer advances
    public int steps = 0;

    // Global modifier for added horror events
    public int horrorLevel = 0;

    public AudioSource audio;

    public List<GameObject> inventory = new List<GameObject>();

    bool isTransitioning = false; // Is the manager currently animating a variable?
    bool newCameraOrder = false; // Has the manager been given a new camera?

    // Cursor mode
    public enum HandMode {
        Interact,
        UseItem,
        Examine
    }

    public HandMode currentHand = HandMode.Interact;
    public GameObject heldObject;

    // Messagebox vars
    [SerializeField]
    private float messageFadeDelay = 3;
    [SerializeField]
    private float messageFadeSpeed = 0.5f;
    private float messageFadeLerp = 0;
    private float messageFadeTimer = 0;

    public LayerMask clickables; // I hate layermasks - this is easiest.

    public bool renderGame = true;
    public bool inGame = true;
    public bool canClick = true;

    // Start is called before the first frame update
    void Start() {
        Cursor.visible = false;
        Object.DontDestroyOnLoad(gameObject);
        postprocessing.profile.TryGet(out lGG);
        postprocessing.profile.TryGet(out SMH);
        gain = lGG.gain.value;
        gain.w = 0;
        lGG.gain.value = gain;
        SMHOff = SMH.shadows.value;
        SMHOn = SMH.shadows.value;
        SMHOn.w = shadowStrength;
        DarkAdaptation(true);
    }

    // Update is called once per frame
    void Update() {
        if (inGame) HandlePlayerInput();
        HandleAnimations();
        MessageBox();
    }

    private void LateUpdate() {
        if (inGame) SendHover(GetMousePositionOnViewport());
    }

    void HandlePlayerInput() {
        //Debug.Log(Input.mousePosition.ToString());
        DebugShowMouseLoc();
        if (Input.GetMouseButtonDown(0) && canClick) {
            switch (currentHand) {
                case HandMode.Interact:
                    SendClick(GetMousePositionOnViewport());
                    break;
                case HandMode.Examine:
                    SendExamine(GetMousePositionOnViewport());
                    break;
            }
            
        }
        //Check for hoverables

    }



    void HandleAnimations() {
        // Start transition
        if (newCameraOrder) {
            // Handle fade
            if (gainLerp < 1) {
                canClick = false;
                steps += 1;
                gainLerp += gainLerpSpeed * Time.deltaTime;
                gainLerp = Mathf.Clamp01(gainLerp);
            }else if (gainLerp == 1) {
                Camera.main.transform.position = nextCamera.transform.position;
                Camera.main.transform.rotation = nextCamera.transform.rotation;
                newCameraOrder = false;
            }
        }
        else {
            if (gainLerp > 0) {
                gainLerp -= gainLerpSpeed * Time.deltaTime;
                gainLerp = Mathf.Clamp01(gainLerp);
            }
            else {
                canClick = true;
            }
        }
        gain.w = Mathf.Lerp(0, -1, gainLerp);
        lGG.gain.value = gain;
    }

    void MessageBox() {
        // Set messageFadeTimer to 0 to reset this animation
        // Show message for this much time
        if (messageFadeTimer < messageFadeDelay) {
            messageFadeTimer += Time.deltaTime;
        }
        // Begin fadeout
        else {
            if (messageFadeLerp < 1) {
                messageFadeLerp += messageFadeSpeed * Time.deltaTime;
            }
            
        }
        Color alpha = messageBox.color;
        alpha.a = Mathf.Lerp(1, 0, messageFadeLerp);
        messageBox.color = alpha;
    }

    public void MessagePlayer(string message) {
        // Always reset timer if new message received
        messageFadeTimer = 0;
        messageFadeLerp = 0;
        messageBox.text = message;
    }

    // Set to true for messages on hover
    // Should be overwritten by delayed messages as they are more important
    public void MessagePlayer(string message, bool ignoreDelay) {
        // Always reset timer if new message received
        if (!ignoreDelay) {
            messageFadeTimer = 0;
            messageFadeLerp = 0;
            messageBox.text = message;
        }
        else {
            if (messageFadeTimer >= 3) {
                messageBox.text = message;
                messageFadeLerp = 0;
            }
        }
            
            
        
    }

    private void SendClick(Vector2 pos) {

        Ray whereToClick = Camera.main.ViewportPointToRay(pos);
        RaycastHit hit;
        if (Physics.Raycast(whereToClick, out hit, Mathf.Infinity, clickables)){
            Debug.Log("Clicked something");
            hit.collider.gameObject.SendMessage("Clicked", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void SendExamine(Vector2 pos) {

        Ray whereToClick = Camera.main.ViewportPointToRay(pos);
        RaycastHit hit;
        if (Physics.Raycast(whereToClick, out hit, Mathf.Infinity, clickables)) {
            Debug.Log("Examined something");
            currentHand = HandMode.Interact;
            hit.collider.gameObject.SendMessage("GetDesc", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void SendHover(Vector2 pos) {
        Ray whereToHover = Camera.main.ViewportPointToRay(pos);
        RaycastHit hit;
        if (Physics.Raycast(whereToHover, out hit, Mathf.Infinity, clickables)) {
            Debug.Log("Hovering on something");
            hit.collider.gameObject.SendMessage("GetName", SendMessageOptions.DontRequireReceiver);
        }
    }

    // Sets the new camera and resets variables to begin fade transition
    public void CameraTransition(GameObject newPos) {
        nextCamera = newPos;
        isTransitioning = true;
        gainLerp = 0;
        newCameraOrder = true;
    }

    // Converts mouse location on viewport to coordinate for ViewportToRay
    public Vector2 GetMousePositionOnViewport() {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(viewPort, Input.mousePosition, null, out Vector2 localPos);
        localPos.x += viewPort.rect.width / 2f;
        localPos.y += viewPort.rect.height / 2f;
        Vector2 viewportSize = viewPort.rect.size;

        // Convert mouse relative location to value from 0 to 1
        localPos.x = localPos.x / viewportSize.x;
        localPos.y = localPos.y / viewportSize.y;

        // Clamp so off-screen objects cannot be clicked
        localPos.x = Mathf.Clamp01(localPos.x);
        localPos.y = Mathf.Clamp01(localPos.y);

        //Debug.Log("mouse is located on viewport at " + localPos);
        return localPos;
    }

    private void DebugShowMouseLoc() {
        debugMousePointer.position = Input.mousePosition;
    }

    // Handle cursor change, including unequipping item if one held
    public void ChangeMode(HandMode mode) {
        currentHand = mode;
        heldObject = null;
    }
    
    public void PlaySound(AudioClip sound) {
        audio.Stop();
        audio.PlayOneShot(sound);
    }

    public void AddItem(GameObject item) {
        inventory.Add(item);
    }

    public void CheckStatus() {
        switch (horrorLevel) {
            case 0:
                MessagePlayer("Besides the crawling sensation, I'm okay.");
                break;
            case 1:
                MessagePlayer("Starting to feel a bit paranoid.");
                break;
            case 2:
                MessagePlayer("I think I'm hallucinating. I can't feel my fingers.");
                break;
            case 3:
                MessagePlayer("The crawling is unbearable. I'm starting to lose it.");
                break;
            case 4:
                MessagePlayer("I don't know what's real anymore.");
                break;
            case 5:
                MessagePlayer("It's in my head.");
                break;
            default:
                MessagePlayer("Debug message. Horror level beyond expectations.");
                break;
        }
    }

    void DarkAdaptation(bool toggle) {
        
        switch (toggle) {
            case true:
                SMH.shadows.value = SMHOn;
            break;
            case false:
                SMH.shadows.value = SMHOff;
                break;
        }
    }
}
