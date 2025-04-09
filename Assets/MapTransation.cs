using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;

public class MapTransation : MonoBehaviour
{
    [SerializeField] public PolygonCollider2D mapBoundry;
    CinemachineConfiner2D confiner;

    private void Awake()
    {
        confiner = object.FindFirstObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            confiner.m_BoundingShape2D = mapBoundry;
        }
    }
}
