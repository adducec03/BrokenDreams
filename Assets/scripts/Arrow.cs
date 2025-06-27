using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 0.5f;
    public float lifetime = 5f;
    public float stuckDuration = 0.3f;

    private bool isActive = false;
    private bool isStuck = false;

    public void Initialize()
    {
        Destroy(gameObject, lifetime);
        Invoke(nameof(Activate), 0.1f); // Ritardo attivazione trigger
    }

    void Activate()
    {
        isActive = true;
    }

    void Update()
    {
        if (isActive && !isStuck)
        {
            // Aggiorno la posizione in modo che segua la giusta direzione dello sparo
            transform.position += -transform.right * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive || isStuck) return;

        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(5f);
            }

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            isStuck = true;

            // Ferma qualsiasi movimento residuo
            speed = 0f;

            // Disattiva il collider per evitare collisioni ripetute
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // Fissa la freccia al punto di impatto
            transform.SetParent(other.transform, true);

            // Distrugge la freccia dopo 2 secondi
            Destroy(gameObject, stuckDuration);
        }
    }
}
