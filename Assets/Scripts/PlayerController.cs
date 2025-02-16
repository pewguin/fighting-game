using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float horizontalForce;
    [SerializeField] float deadband;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 velocity = new Vector2();

        velocity.x += ApplyDeadband(Input.GetAxisRaw("Horizontal"), deadband) * horizontalForce;
        if (Mathf.Sign(velocity.x) != Mathf.Sign(rb.velocity.x))
        {
            Vector2 currentVelocity = rb.velocity;
            currentVelocity.x = 0;
            rb.velocity = currentVelocity;
        }

        rb.AddForce(velocity, ForceMode2D.Force);
    }

    private float ApplyDeadband(float val, float deadband)
    {
        if (val > deadband)
        {
            return 1;
        }
        else if (val < -deadband)
        {
            return -1;
        }
        return 0;
    }
}
