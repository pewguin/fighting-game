using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBuffer : MonoBehaviour
{
	private List<InputEntry> inputHistory = new List<InputEntry>();
	private List<ButtonEntry> buttonHistory = new List<ButtonEntry>();
	public event Action<InputEntry> newButtonPressed;
	[Tooltip("Time before inputs are fully cleared from the buffer")]
	[SerializeField] float maximumBufferTime = 0.5f;
	[SerializeField] float deadband;

	
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
	[Serializable]
	public struct MotionInput {
		public List<InputDirection> InputPattern;
		public InputButton Button;
		public float BufferTime;

		public MotionInput(List<InputDirection> inputPattern, InputButton button, float bufferTime) {
			InputPattern = inputPattern;
			Button = button;
			BufferTime = bufferTime;
		}
	}
	public struct InputEntry {
		public InputDirection? Direction;
		public InputButton? Button;
		public float Time;
		public bool ButtonJustPressed;
		public InputEntry(InputDirection? direction, InputButton? button, float time, bool buttonJustPressed = false) {
			Direction = direction;
			Button = button;
			Time = time;
			ButtonJustPressed = buttonJustPressed;
		}
	}
	public struct ButtonEntry {
		public InputButton Button;
		public float Time;
		public ButtonEntry(InputButton button, float time) {
			Button = button;
			Time = time;
		}
	}
	public enum InputDirection {
		Neutral, Forward, Back, Up, Down, UpForward, DownForward, UpBack, DownBack
	}
	public enum InputButton {
		Light, Medium, Heavy, Special
	}

	public void GetInput() {
		inputHistory.RemoveAll(entry => Time.time - entry.Time > maximumBufferTime);
		buttonHistory.RemoveAll(entry => Time.time - entry.Time > maximumBufferTime);

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

		if (justPressed && button.HasValue)
			buttonHistory.Add(new ButtonEntry(button.Value, Time.time));

		InputDirection? direction = null;

		Vector2Int directionVector = new(
			Mathf.RoundToInt(ApplyDeadband(Input.GetAxisRaw("Horizontal"), deadband)),
			Mathf.RoundToInt(ApplyDeadband(Input.GetAxisRaw("Vertical"), deadband)));

		if (directionMap.TryGetValue(directionVector, out InputDirection dir))
			direction = dir;

		if (direction != null || button != null) {
			InputEntry entry = new InputEntry(direction, button, Time.time, justPressed);
			if (inputHistory.Count > 0) {
				InputEntry lastInput = inputHistory[inputHistory.Count - 1];
				if (lastInput.Direction != direction || lastInput.Button != button) {
					newButtonPressed?.Invoke(entry);
					inputHistory.Add(entry);
				}
			} else {
				inputHistory.Add(entry);
			}
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
					if (CheckButtonHistory(input.Button)) {
						return true;
					}
					return false;
				}
			}
		}
		return false;
	}
	public bool CheckButtonHistory(InputButton button) {
		foreach (var entry in buttonHistory) {
			if (entry.Button == button)
				return true;
		}
		return false;
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
