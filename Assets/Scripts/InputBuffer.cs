using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum InputDirection {
	Neutral, Forward, Back, Up, Down, UpForward, DownForward, UpBack, DownBack
}
public enum InputButton {
	Light, Medium, Heavy, Special
}

public class InputBuffer : MonoBehaviour
{
	private List<InputEntry> inputHistory = new List<InputEntry>();
	private List<ButtonEntry> inputBuffer = new List<ButtonEntry>();
	public InputDirection? lastMovement = InputDirection.Neutral;
	public Action<InputEntry> onInput;
	[Tooltip("Time before inputs are fully cleared from the buffer")]
	[SerializeField] int maximumHistoryFrames = 20;
	[SerializeField] int maximumBufferFrames = 5;
	[SerializeField] float deadband;

	public static Dictionary<Vector2Int, InputDirection> vecToDirectionMap = new()
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
	public static Dictionary<InputDirection, Vector2Int> directionToVecMap;

	[Serializable]
	public struct MotionInput {
		public List<InputDirection> InputPattern;
		public InputButton Button;
		public int BufferTime;

		public MotionInput(List<InputDirection> inputPattern, InputButton button, int bufferTime) {
			InputPattern = inputPattern;
			Button = button;
			BufferTime = bufferTime;
		}
	}
	public struct InputEntry {
		public InputDirection? Direction;
		public InputButton? Button;
		public int Time;
		public bool ButtonJustPressed;
		public InputEntry(InputDirection? direction, InputButton? button, int time, bool buttonJustPressed = false) {
			Direction = direction;
			Button = button;
			Time = time;
			ButtonJustPressed = buttonJustPressed;
		}
	}
	public struct ButtonEntry {
		public InputButton Button;
		public int Time;
		public ButtonEntry(InputButton button, int time) {
			Button = button;
			Time = time;
		}
	}
	private void Awake() {
		directionToVecMap = vecToDirectionMap.ToDictionary(kv => kv.Value, kv => kv.Key);
	}

	public void GetInput(int framesSinceBeginning) {
		inputHistory.RemoveAll(entry => framesSinceBeginning - entry.Time > maximumHistoryFrames);
		inputBuffer.RemoveAll(entry => framesSinceBeginning - entry.Time > maximumBufferFrames);

		InputButton? button = null;
		bool justPressed = false;

		if (Input.GetKey(KeyCode.U))
			button = InputButton.Light;
		else if (Input.GetKey(KeyCode.I))
			button = InputButton.Medium;
		else if (Input.GetKey(KeyCode.J))
			button = InputButton.Heavy;
		else if (Input.GetKey(KeyCode.K))
			button = InputButton.Special;

		if (Input.GetKeyDown(KeyCode.U)
			|| Input.GetKeyDown(KeyCode.I)
			|| Input.GetKeyDown(KeyCode.J)
			|| Input.GetKeyDown(KeyCode.K))
			justPressed = true;

		InputDirection? direction = null;

		Vector2Int directionVector = new(
			Mathf.RoundToInt(ApplyDeadband(Input.GetAxisRaw("Horizontal"), deadband)),
			Mathf.RoundToInt(ApplyDeadband(Input.GetAxisRaw("Vertical"), deadband)));

		if (vecToDirectionMap.TryGetValue(directionVector, out InputDirection dir)) {
			direction = dir;
			lastMovement = dir;
		}

		if (direction != null || button != null) {
			InputEntry entry = new InputEntry(direction, button, framesSinceBeginning, justPressed);
			if (inputHistory.Count > 0) {
				InputEntry lastInput = inputHistory[inputHistory.Count - 1];
				if (lastInput.Direction != direction || lastInput.Button != button) {
					inputHistory.Add(entry);
					onInput.Invoke(entry);
				}
			} else {
				inputHistory.Add(entry);
			}
		}
		if (button != null && justPressed) {
			inputBuffer.Add(new ButtonEntry((InputButton)button, framesSinceBeginning));
		}
	}
	public bool CheckMotionInput(MotionInput input) {
		int patternIndex = 0;
		foreach (var entry in inputHistory) {
			if (Time.time - entry.Time > input.BufferTime)
				continue;
			if (entry.Direction.HasValue && entry.Direction.Value == input.InputPattern[patternIndex]) {
				patternIndex++;
				if (patternIndex >= input.InputPattern.Count) {
					return CheckBuffer(input.Button);
				}
			}
		}
		return false;
	}
	public bool CheckBuffer(InputButton[] buttons) {
		List<InputButton> remainingButtons = buttons.ToList();
		foreach (var entry in inputBuffer) {
			remainingButtons.Remove(entry.Button);
			if (remainingButtons.Count <= 0) {
				return true;
			}
		}
		return false;
	}
	public bool CheckBuffer(InputButton button) {
		return CheckBuffer(new InputButton[]{ button });
	}
	private float ApplyDeadband(float val, float deadband) {
		if (val > deadband) {
			return 1;
		} else if (val < -deadband) {
			return -1;
		}
		return 0;
	}
}
