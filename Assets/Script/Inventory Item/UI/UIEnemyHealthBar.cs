using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class UIEnemyHealthBar : MonoBehaviour
{
    private EnemyEntity entity;
    private RectTransform rectTransform;
    private Slider slider;
    private CharacterStats myStats;
    private void Start()
    {
        myStats = GetComponentInParent<CharacterStats>();
        rectTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<EnemyEntity>();
        slider = GetComponentInChildren<Slider>();
        entity.onFlipped += FlipUi;
        myStats.OnHealthChanged += UpdateHealthUI;
    }



    private void UpdateHealthUI()
    {
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }
    private void FlipUi()
    {
        rectTransform.Rotate(0, 180, 0);
    }

    private void OnDisable()
    {
        entity.onFlipped -= FlipUi;
        myStats.OnHealthChanged -= UpdateHealthUI;
    }
}
