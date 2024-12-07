using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public enum HealthEntityType { Player, Enemy }

    [SerializeField] HealthEntityType healthEntity;
    [SerializeField] Slider slider;

    private void Update() {
        switch (healthEntity) {
            case HealthEntityType.Player:
                slider.value = Player.CurrHealth / Player.MaxHealth;
                break;
            case HealthEntityType.Enemy:
                slider.value = EnemyPrototype_V1.CurrHealth / EnemyPrototype_V1.MaxHealth;
                break;
        }
    }
}
