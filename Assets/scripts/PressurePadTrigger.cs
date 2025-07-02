using UnityEngine;
using System.Collections;


public class PressurePadTrigger : MonoBehaviour
{
    public string pressurePadID;

    [Header("Muro da far scomparire dopo aver sconfitto i nemici")]
    public GameObject wallToDisable;

    [Header("Nemici da attivare")]
    public GameObject[] enemiesToSpawn;

    private Animator padAnimator;
    private Animator wallAnimator;
    private GhostWallController ghostWall;

    private int playersOnPad = 0;
    private bool activated = false;
    private bool wallDisabled = false;

    void Start()
    {
        padAnimator = GetComponent<Animator>();

        if (wallToDisable != null)
        {
            wallAnimator = wallToDisable.GetComponent<Animator>();
            ghostWall = wallToDisable.GetComponent<GhostWallController>();
        }

        // Controlla lo stato DOPO che il SaveController ha avuto il tempo di caricare
        StartCoroutine(DelayedLoadState());
    }

    IEnumerator DelayedLoadState()
    {
        Debug.Log($"[PAD] Carico stato: activated={activated}, wallDisabled={wallDisabled}");

        yield return new WaitForSeconds(0.1f); // oppure WaitForEndOfFrame se preferisci

        var save = FindFirstObjectByType<SaveController>();
        if (save != null)
        {
            if (save.IsPressurePadActivated(pressurePadID))
            {
                activated = true;
            }

            if (save.IsWallDisabled(pressurePadID))
            {
                wallDisabled = true;

                if (wallToDisable != null)
                {
                    wallToDisable.SetActive(false);
                }
            }
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playersOnPad++;

            if (padAnimator != null)
                padAnimator.SetBool("isPressed", true);

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
        if (activated && !wallDisabled && AreAllEnemiesDefeated())
        {
            wallDisabled = true;

            if (wallAnimator != null)
            {
                wallAnimator.SetBool("shouldDisappear", true);
            }
            else if (wallToDisable != null)
            {
                wallToDisable.SetActive(false); // fallback
            }
            var save = FindFirstObjectByType<SaveController>();
            if (save != null)
                save.SetWallDisabled(pressurePadID, true);
        }
    }

    private bool AreAllEnemiesDefeated()
    {
        foreach (var enemy in enemiesToSpawn)
            if (enemy != null && enemy.activeInHierarchy)
                return false;

        return true;
    }

    public bool IsActivated()
    {
        return activated;
    }

    public bool IsWallDisabled()
    {
        return wallDisabled;
    }

    public string GetPadID()
    {
        return pressurePadID;
    }

}
