using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float horizontalForce;
    [SerializeField] private float deadband;
    private const float maximumBufferTime = 3f;
    private List<InputEntry> inputHistory = new List<InputEntry>();

    private static Dictionary<Vector2Int, InputDirection> directionMap = new()
    {
        { new Vector2Int(0, 0), InputDirection.Neutral },
        { new Vector2Int(0, 1), InputDirection.Up },
        { new Vector2Int(1, 1), InputDirection.UpForward },
        { new Vector2Int(1, 0), InputDirection.Forward },
        { new Vector2Int(1, -1), InputDirection.DownForward },
        { new Vector2Int(0, -1), InputDirection.Down },
        { new Vector2Int(-1, -1), InputDirection.DownBack },
        { new Vector2Int(-1, 0), InputDirection.Back },
        { new Vector2Int(-1, 1), InputDirection.UpBack },
    };

    public struct MotionInput
    {
        public List<InputDirection> InputPattern;
        public InputButton Button;
        public float BufferTime;

        public MotionInput(List<InputDirection> inputPattern, InputButton button, float bufferTime = maximumBufferTime)
        {
            InputPattern = inputPattern;
            Button = button;
            BufferTime = bufferTime;
        }
    }
    public struct InputEntry
    {
        public InputDirection? Direction;
        public InputButton? Button;
        public float Time;

        public InputEntry(InputDirection? direction, InputButton? button, float time)
        {
            Direction = direction;
            Button = button;
            Time = time;
        }
    }
    public enum InputDirection
    {
        Neutral, Forward, Back, Up, Down, UpForward, DownForward, UpBack, DownBack
    }
    public enum InputButton
    {
        Quick, Power, Special, Ultra
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        GetInput();
        Vector2 velocity = new Vector2();

        velocity.x += ApplyDeadband(Input.GetAxisRaw("Horizontal"), deadband) * horizontalForce;
        if (Mathf.Sign(velocity.x) != Mathf.Sign(rb.velocity.x))
        {
            Vector2 currentVelocity = rb.velocity;
            currentVelocity.x = 0;
            rb.velocity = currentVelocity;
        }

        rb.AddForce(velocity, ForceMode2D.Force);

        if (CheckMotionInput(
                new MotionInput(new List<InputDirection> { InputDirection.Forward, InputDirection.Down, InputDirection.Back, InputDirection.Forward }, 
                InputButton.Quick)))
        {
            Debug.Log("POTEMKIN BUSTAHHHHHHHH");
        }
    }

    public void GetInput()
    {
        inputHistory.RemoveAll(entry => Time.time - entry.Time > maximumBufferTime);

        InputButton? button = null;

        if (Input.GetKey(KeyCode.U))
            button = InputButton.Quick;
        else if (Input.GetKey(KeyCode.I))
            button = InputButton.Power;
        else if (Input.GetKey(KeyCode.J))
            button = InputButton.Special;
        else if (Input.GetKey(KeyCode.K))
            button = InputButton.Ultra;

        InputDirection? direction = null;

        Vector2Int directionVector = new(
            Mathf.RoundToInt(ApplyDeadband(Input.GetAxisRaw("Horizontal"), deadband)), 
            Mathf.RoundToInt(ApplyDeadband(Input.GetAxisRaw("Vertical"), deadband)));

        if (directionMap.TryGetValue(directionVector, out InputDirection dir))
            direction = dir;

        if (direction != null || button != null)
        {
            if (inputHistory.Count > 0)
            {
                InputEntry lastInput = inputHistory[inputHistory.Count - 1];
                if (lastInput.Direction != direction || lastInput.Button != button)
                {
                    inputHistory.Add(new InputEntry(direction, button, Time.time));
                }
            } 
            else
            {
                inputHistory.Add(new InputEntry(direction, button, Time.time));
            }
        }
    }

    public bool CheckMotionInput(MotionInput input)
    {
        if (input.BufferTime > maximumBufferTime)
            Debug.LogError("Buffer time of inputs must be less than the maximum buffer time! Defaulting to the maximum buffer time.");

        int patternIndex = 0;
        InputButton? recentButton = null;
        foreach (var entry in inputHistory)
        {
            if (Time.time - entry.Time > input.BufferTime)
                continue;

            if (patternIndex > 0 && entry.Button.HasValue)
                recentButton = entry.Button.Value;

            if (entry.Direction.HasValue && entry.Direction.Value == input.InputPattern[patternIndex])
            {
                patternIndex++;
                if (patternIndex >= input.InputPattern.Count)
                {
                    if (recentButton == input.Button)
                    {
                        return true;
                    }
                    return false;
                }
            }
        }
        return false;
    }

    private float ApplyDeadband(float val, float deadband)
    {
        if (val > deadband)
        {
            return 1;
        }
        else if (val < -deadband)
        {
            return -1;
        }
        return 0;
    }
}
