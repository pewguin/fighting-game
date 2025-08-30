using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {
	List<ActiveHitbox> hitboxes;
	List<ActiveHitbox> hurtboxes;
	public void CheckCollisions() {
		foreach (var hit in hitboxes) {
			var hitWorld = hit.GetWorldRect();
			foreach (var hurt in hurtboxes) {
				var hurtWorld = hurt.GetWorldRect();
				if (hitWorld.Overlaps(hurtWorld)) {
					hurt.owner.HitByAttack(hit.hitbox, hit.moveData);
				}
			}
		}
	}
	public void AddHitbox(Hitbox hitbox, PlayerController owner, MoveData move) {
		hitboxes.Add(new(hitbox, owner, move));
	}
	public void AddHurtbox(Hitbox hitbox, PlayerController owner, MoveData move) {
		hurtboxes.Add(new(hitbox, owner, move));
	}
}

public class ActiveHitbox {
	public Hitbox hitbox;
	public PlayerController owner;
	public MoveData moveData;
	public ActiveHitbox(Hitbox hitbox, PlayerController owner, MoveData move) {
		this.hitbox = hitbox;
		this.owner = owner;
		this.moveData = move;
	}
	public Rect GetWorldRect() {
		return new(
			hitbox.box.position + (Vector2)owner.transform.position,
			hitbox.box.size
			);
	}
}