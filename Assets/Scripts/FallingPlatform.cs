using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallDelay = 1f;

    private Rigidbody2D rb;
    private bool hasPlayerLanded = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasPlayerLanded)
        {
            hasPlayerLanded = true;
            Invoke("TriggerFall", fallDelay);
        }
    }

    private void TriggerFall()
    {
        rb.gravityScale = 1f;
    }
}