using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {
	[Serializable]
	public struct Hitbox {
		public Vector2 center;
		public Vector2 extends;
	}
}
