using UnityEngine;

public class PressurePadTrigger : MonoBehaviour
{
    public string pressurePadID;

    [Header("Porta da aprire dopo aver sconfitto i nemici")]
    public GameObject doorToOpen;

    [Header("Nemici da attivare")]
    public GameObject[] enemiesToSpawn;

    private Animator padAnimator;
    private int playersOnPad = 0;
    private bool activated = false;
    private bool doorOpened = false;

    void Start()
    {
        padAnimator = GetComponent<Animator>();

        if (doorToOpen != null)
            doorToOpen.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playersOnPad++;

            if (padAnimator != null)
                padAnimator.SetBool("isPressed", true);

            // Attiva nemici e salva stato solo la prima volta
            if (!activated)
            {
                activated = true;

                foreach (GameObject enemy in enemiesToSpawn)
                    if (enemy != null)
                        enemy.SetActive(true);

                var save = FindFirstObjectByType<SaveController>();
                if (save != null)
                    save.SetPressurePadActivated(pressurePadID, true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playersOnPad = Mathf.Max(0, playersOnPad - 1);

            if (playersOnPad == 0 && padAnimator != null)
                padAnimator.SetBool("isPressed", false);
        }
    }

    void Update()
    {
        if (activated && !doorOpened && AreAllEnemiesDefeated())
        {
            if (doorToOpen != null)
                doorToOpen.SetActive(false); // O apri con animazione

            doorOpened = true;
        }
    }

    private bool AreAllEnemiesDefeated()
    {
        foreach (var enemy in enemiesToSpawn)
            if (enemy != null && enemy.activeInHierarchy)
                return false;

        return true;
    }
}
