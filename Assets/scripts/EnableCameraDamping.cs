using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class EnableCameraDampingAfterLoad : MonoBehaviour
{
    public CinemachineCamera virtualCamera;
    public Vector3 dampingToRestore = new Vector3(3f, 3f, 1f); // modifica in base al tuo damping ideale

    void Start()
    {
        StartCoroutine(EnableDampingNextFrame());
    }

    IEnumerator EnableDampingNextFrame()
    {
        yield return null;
        var composer = virtualCamera.GetComponent<CinemachinePositionComposer>();
        if (composer != null)
        {
            composer.Damping = dampingToRestore;
        }
    }
}
