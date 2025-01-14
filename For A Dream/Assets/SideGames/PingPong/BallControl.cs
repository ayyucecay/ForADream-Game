﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour {

	private Rigidbody2D rb2d;

	void GoBall() {
		float rand = Random.Range (-20, 20);
		
			rb2d.AddForce (new Vector2 (-30, rand));
		
	}

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		Invoke ("GoBall", 2);
	}

	void ResetBall() {
		rb2d.velocity = new Vector2 (0, 0);
		transform.position = Vector2.zero;
	}

	void RestartGame() {
		ResetBall ();
		Invoke ("GoBall", 1);
	}

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.collider.CompareTag ("Player")) {
			FindObjectOfType<AudioManager>().Play("PingPong");
			Vector2 vel;
			vel.x = rb2d.velocity.x;
			vel.y = (rb2d.velocity.y / 2.0f) + (coll.collider.attachedRigidbody.velocity.y / 3.0f);
			rb2d.velocity = vel;
		}
	}

}
