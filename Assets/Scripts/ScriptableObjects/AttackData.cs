using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Fighting Game/Attack")]
public class AttackData : ScriptableObject
{
    public PlayerController.MotionInput motionInput;
    public int startupFrames;
    public int activeFrames;
    public int recoveryFrames;
    public GameObject hitbox;
}
