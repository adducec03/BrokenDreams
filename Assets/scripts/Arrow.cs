using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 0.5f;
    public float lifetime = 5f;

    private bool isActive = false;

    public void Initialize()
    {
        Destroy(gameObject, lifetime);
        Invoke(nameof(Activate), 0.1f); // Ritarda attivazione trigger
    }

    void Activate()
    {
        isActive = true;
    }

    void Update()
    {
        // ✅ Movimento lungo -transform.right per correggere il prefab
        transform.position += -transform.right * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("Il player è stato colpito!");
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
