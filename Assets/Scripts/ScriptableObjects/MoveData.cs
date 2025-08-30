using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fighting Game/Move Data")]
public class MoveData : ScriptableObject
{
    public InputBuffer.MotionInput input;
	public List<FrameData> frameData = new List<FrameData>();
	public int TotalFrames() {
		if (frameData == null) return 0;
		return frameData.Count;
	}
}

[Serializable]
public class FrameData {
	public List<Hitbox> hitboxes = new List<Hitbox>();
	public int animationFrame = 0;
}
[Serializable]
public enum HitboxType {
	Hitbox,
	Hurtbox,
}
[Serializable]
public class Hitbox {
	public Rect box = new(0, 0, 1, 1);
	public HitboxType type = 0;
}