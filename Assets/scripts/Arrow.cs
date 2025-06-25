using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Colpito il player!");
            // Qui potresti chiamare player.Damage()
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            // Colpito un muro o altro
            Destroy(gameObject);
        }
    }
}
