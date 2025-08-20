using UnityEngine;

// Oyuncu hareket ve çarpışma kontrolü
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public float startVelocity, jumpForce;
    public BLOCKCOLOR playerColor;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;
    }

    private void Update()
    {
        // Ekrana tıklandığında oyuncu zıplar
        if (Input.GetMouseButtonDown(0))
        {
            rb.velocity = (Vector2.up * jumpForce + (rb.velocity.x > 0 ? Vector2.right : Vector2.left)) * startVelocity;
        }
    }

    // Oyun başladığında oyuncu hareket etmeye başlar
    public void GameStart()
    {
        rb.simulated = true;
        rb.velocity = (Vector2.up + Vector2.right) * startVelocity;
    }

    // Çarpışma kontrolü
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Death"))
        {
            GameManager.instance.GameOver();
            return;
        }

        if (collision.gameObject.CompareTag("Block"))
        {
            // Renkler uyuşmazsa oyun biter
            if (playerColor != collision.gameObject.GetComponent<Block>().blockColor)
            {
                GameManager.instance.GameOver();
                return;
            }
            GameManager.instance.UpdateScore();
        }
    }
}
