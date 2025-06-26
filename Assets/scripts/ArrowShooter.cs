using UnityEngine;

public class ArrowShooter : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float fireRate = 2f;
    public Vector2 shootDirection = Vector2.left;
    [SerializeField] private Vector2 spawnOffset = new Vector2(0.5f, 0f);

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    void Shoot()
    {
        Vector2 dynamicOffset = new Vector2(
            Mathf.Sign(shootDirection.x) * Mathf.Abs(spawnOffset.x),
            Mathf.Sign(shootDirection.y) * Mathf.Abs(spawnOffset.y)
        );

        Vector2 spawnPosition = (Vector2)transform.position + dynamicOffset;
        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);

        // Rotazione visiva = anche direzione di movimento
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Inizializza freccia (ora non serve più passare direction)
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.Initialize();
        }
    }
    void OnDrawGizmosSelected()
    {
        Vector2 dynamicOffset = new Vector2(
            Mathf.Sign(shootDirection.x) * Mathf.Abs(spawnOffset.x),
            Mathf.Sign(shootDirection.y) * Mathf.Abs(spawnOffset.y)
        );

        Vector2 spawnPosition = (Vector2)transform.position + dynamicOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(spawnPosition, 0.1f);
    }
}
