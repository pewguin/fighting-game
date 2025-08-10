using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fighting Game/Move Data")]
public class MoveData : ScriptableObject
{
    public InputBuffer.MotionInput input;
    public int startupFrames;
    public int activeFrames;
    public int recoveryFrames;
	public CollisionManager.Hitbox[][] composedHitboxes;
	public CollisionManager.Hitbox[][] composedHurtboxes;
}
