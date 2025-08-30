using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InputDisplay : MonoBehaviour
{
    [SerializeField] InputBuffer buffer;
    [SerializeField] List<Sprite> motionInputs; // Order: Neutral, Forward, Down Forward, etc.
    [SerializeField] List<Sprite> buttonInputs; // Order: Light, Medium, Heavy, Special
    [SerializeField] GameObject inputDisplayPrefab;
    [SerializeField] int inputsToDisplay = 10;

    private Dictionary<InputDirection, Sprite> directionInputMap;
    private Dictionary<InputButton, Sprite> buttonInputMap;
    void Start()
    {
        directionInputMap = new()
        {
            { InputDirection.Neutral, motionInputs[0] },
            { InputDirection.Forward, motionInputs[1] },
            { InputDirection.DownForward, motionInputs[2] },
            { InputDirection.Down, motionInputs[3] },
            { InputDirection.DownBack, motionInputs[4] },
            { InputDirection.Back, motionInputs[5] },
            { InputDirection.UpBack, motionInputs[6] },
            { InputDirection.Up, motionInputs[7] },
            { InputDirection.UpForward, motionInputs[8] },
        };
        buttonInputMap = new()
        {
            { InputButton.Light, buttonInputs[0] },
            { InputButton.Medium, buttonInputs[1] },
            { InputButton.Heavy, buttonInputs[2] },
            { InputButton.Special, buttonInputs[3] },
        };
        buffer.onInput += OnNewButton;
    }

    private void OnNewButton(InputBuffer.InputEntry input)
    {
        GameObject imageObj = Instantiate(inputDisplayPrefab, transform);
        if (input.Direction.HasValue && input.Direction.Value != InputDirection.Neutral && directionInputMap.TryGetValue(input.Direction.Value, out Sprite dir))
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
        if (transform.childCount > inputsToDisplay)
        {
            for (int i = inputsToDisplay; i < transform.childCount - 1; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
