using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDevice : MonoBehaviour
{
    [SerializeField] private float jumpPower;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("ddd");
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("dd");
            CharacterManager.Instance.Player.controller._rigidbody.AddForce(Vector3.up*jumpPower, ForceMode.Impulse);
        }
    }

}
