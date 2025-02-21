using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InputDisplay : MonoBehaviour
{
    [SerializeField] Transform imageParent;
    [SerializeField] PlayerController player;
    [SerializeField] List<Sprite> motionInputs; // Order: Neutral, Forward, Down Forward, etc.
    [SerializeField] List<Sprite> buttonInputs; // Order: Quick, Power, Special, Ultra
    [SerializeField] GameObject inputDisplayPrefab;
    [SerializeField] int inputsToDisplay = 10;
    private Dictionary<PlayerController.InputDirection, Sprite> directionInputMap;
    private Dictionary<PlayerController.InputButton, Sprite> buttonInputMap;
    void Start()
    {
        directionInputMap = new()
        {
            { PlayerController.InputDirection.Neutral, motionInputs[0] },
            { PlayerController.InputDirection.Forward, motionInputs[1] },
            { PlayerController.InputDirection.DownForward, motionInputs[2] },
            { PlayerController.InputDirection.Down, motionInputs[3] },
            { PlayerController.InputDirection.DownBack, motionInputs[4] },
            { PlayerController.InputDirection.Back, motionInputs[5] },
            { PlayerController.InputDirection.UpBack, motionInputs[6] },
            { PlayerController.InputDirection.Up, motionInputs[7] },
            { PlayerController.InputDirection.UpForward, motionInputs[8] },
        };
        buttonInputMap = new()
        {
            { PlayerController.InputButton.Quick, buttonInputs[0] },
            { PlayerController.InputButton.Power, buttonInputs[1] },
            { PlayerController.InputButton.Special, buttonInputs[2] },
            { PlayerController.InputButton.Ultra, buttonInputs[3] },
        };
        player.newButtonPressed += OnNewButton;
    }

    private void OnNewButton(PlayerController.InputEntry input)
    {
        GameObject imageObj = Instantiate(inputDisplayPrefab, imageParent);
        if (input.Direction.HasValue && input.Direction.Value != PlayerController.InputDirection.Neutral && directionInputMap.TryGetValue(input.Direction.Value, out Sprite dir))
        {
            Image dirimg = imageObj.GetComponentsInChildren<Image>()[1];
            dirimg.enabled = true;
            dirimg.sprite = dir;
        }
        if (input.Button.HasValue && buttonInputMap.TryGetValue(input.Button.Value, out Sprite button))
        {
            Image btnimg = imageObj.GetComponentsInChildren<Image>()[0];
            btnimg.enabled = true;
            btnimg.sprite = button;
            if (!input.ButtonJustPressed)
            {
                btnimg.color = new Color(1, 1, 1, 0.3f);
            }
        }
        imageObj.transform.SetAsFirstSibling();
        if (imageParent.childCount > inputsToDisplay)
        {
            for (int i = inputsToDisplay; i < imageParent.childCount - 1; i++)
            {
                Destroy(imageParent.GetChild(i).gameObject);
            }
        }
    }
}
