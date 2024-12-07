using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    [SerializeField] Slider slider;

    private void Update() {
        slider.value = Player.CurrHealth / Player.MaxHealth;
    }
}
