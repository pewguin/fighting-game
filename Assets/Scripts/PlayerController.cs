using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
	// stuff that should go in a GameController
	private int framesSinceBeginning = 0;
	private float frameTime = 1f / 60f;
	private float timeSinceFrame = 0f;

	// move tracking
	public bool inMove = false;
	private MoveData currentMove = null;
	private int currentMoveFrame = 0;

	// input stuff
	[SerializeField] InputBuffer buffer;
	[SerializeField] SpriteRenderer sr;
	[SerializeField] Sprite[] animationFrames;
    [SerializeField] private float deadband;
	[SerializeField] private MoveData[] moves;

	//movement
	private Vector2 velocity = Vector2.zero;
	[SerializeField] private float horizontalForce;
	void Update()
    {
        Vector2 velocity = new Vector2();

        velocity.x += Input.GetAxisRaw("Horizontal") * horizontalForce;

		buffer.GetInput(framesSinceBeginning);
		timeSinceFrame += Time.deltaTime;
		while (timeSinceFrame > frameTime) {
			timeSinceFrame -= frameTime;
			Frame();
		}
    }
	public void Frame() {
		framesSinceBeginning++;
		if (!inMove) {
			foreach (var move in moves) {
				if (buffer.CheckMotionInput(move.input)) {
					currentMove = move;
					inMove = true;
					break;
				}
			}
			if (!inMove && buffer.lastMovement != null) {
				Vector2Int dir;
				InputBuffer.directionToVecMap.TryGetValue((InputDirection)buffer.lastMovement, out dir);

			}
		} else {
			if (currentMoveFrame >= currentMove.TotalFrames()) {
				currentMove = null;
				inMove = false;
				currentMoveFrame = 0;
			} else {
				sr.sprite = animationFrames[currentMove.frameData[currentMoveFrame].animationFrame - 1];
				currentMoveFrame++;
			}
        }
		buffer.lastMovement = null;
	}
	public void SetAnimationFrame(int frame) {
		sr.sprite = animationFrames[frame];
	}
	public int TotalAnimationFrames() {
		return animationFrames.Length;
	}
	public void HitByAttack(Hitbox hitbox, MoveData data) {
		Debug.Log("hit by " + data.name);
	}
}
