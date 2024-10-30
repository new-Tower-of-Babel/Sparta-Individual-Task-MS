using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectCheck : MonoBehaviour
{
    [SerializeField] private float checkRate = 0.05f;
    [SerializeField] private float maxCheckDistance;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private TextMeshProUGUI promptText;

    private float lastCheckTime;
    private Camera mainCamera;
    private GameObject currentInteractable;

    void Start()
    {
        mainCamera = Camera.main;
        promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Time.time - lastCheckTime > checkRate)
        {
            CheckForInteractable();
            lastCheckTime = Time.time;
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, maxCheckDistance, layerMask))
        {
            if (hit.collider.gameObject != currentInteractable)
            {
                Debug.Log("1");
                currentInteractable = hit.collider.gameObject;
                SetPromptText();
            }
        }
        else
        {
            currentInteractable = null;
            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void SetPromptText()
    {
        if (currentInteractable != null)
        {
            var interactable = currentInteractable.GetComponent<Interactable>();
            if (interactable != null)
            {
                promptText.text = interactable.displayName;
                promptText.gameObject.SetActive(true);
            }
            else
            {
                promptText.text = "Interact";
                promptText.gameObject.SetActive(true);
            }
        }
    }
}
