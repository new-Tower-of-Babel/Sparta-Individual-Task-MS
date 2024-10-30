using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCondition : MonoBehaviour
{
    [SerializeField] private float startValue = 100f;
    [SerializeField] private float maxValue = 100f;
    [SerializeField] private float passiveValue = 2f;
    [SerializeField] private Image uiBar;

    private float _curValue;
    private float previousValue;

    public float CurValue
    {
        get => _curValue;
        private set
        {
            _curValue = Mathf.Clamp(value, 0, maxValue);
            UpdateUI();
        }
    }
    private void Start()
    {
        CurValue = Mathf.Clamp(startValue, 0, maxValue);
        previousValue = CurValue;
    }
    private void Update()
    {
        if (CurValue <= maxValue)
        {
            CurValue += passiveValue * Time.deltaTime * 2;
        }
    }
    private float GetPercentage()
    {
        if (maxValue == 0) return 0;
        return _curValue / maxValue;
    }
    public void Add(float value)
    {
        CurValue += value;
    }
    public void Subtract(float value)
    {
        CurValue -= value;
    }
    private void UpdateUI()
    {
        if (uiBar != null)
        {
            uiBar.fillAmount = GetPercentage();
        }
        else
        {
            Debug.LogWarning("UI 표시줄이 없음");
        }
    }
}
