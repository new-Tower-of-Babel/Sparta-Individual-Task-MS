using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpeedUp : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 0.5f;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ddd");
        if (other.CompareTag("Player"))
        {
            Debug.Log("dd");
            CharacterManager.Instance.Player.controller.ActiveBoost();
            Destroy(gameObject,destroyDelay);
        }
    }
}
