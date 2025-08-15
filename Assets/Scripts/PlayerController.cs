using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    private Rigidbody2D rb;
    [SerializeField] private float horizontalForce;
    [SerializeField] private float deadband;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        Vector2 velocity = new Vector2();

        velocity.x += Input.GetAxisRaw("Horizontal") * horizontalForce;
        if (Mathf.Sign(velocity.x) != Mathf.Sign(rb.velocity.x))
        {
            Vector2 currentVelocity = rb.velocity;
            currentVelocity.x = 0;
            rb.velocity = currentVelocity;
        }

        rb.AddForce(velocity, ForceMode2D.Force);
    }
}
