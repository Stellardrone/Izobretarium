using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriceCalculator : MonoBehaviour
{
    public static PriceCalculator Instance;

    // Множители спроса для разных категорий
    public float robotDemand = 1.0f;
    public float techDemand = 1.0f;
    public float retroDemand = 1.0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Получить множитель спроса для категории
    public float GetDemandMultiplier(string category)
    {
        switch (category)
        {
            case "robot": return robotDemand;
            case "tech": return techDemand;
            case "retro": return retroDemand;
            default: return 1.0f;
        }
    }

    // Обновить спрос на основе событий
    public void UpdateDemand(string category, float change)
    {
        switch (category)
        {
            case "robot": robotDemand = Mathf.Max(0.3f, robotDemand + change); break;
            case "tech": techDemand = Mathf.Max(0.3f, techDemand + change); break;
            case "retro": retroDemand = Mathf.Max(0.3f, retroDemand + change); break;
        }

        Debug.Log($"Спрос: Роботы {robotDemand:F1}, Техно {techDemand:F1}, Ретро {retroDemand:F1}");
    }

    // Случайное изменение спроса в начале дня
    public void RandomizeDemand()
    {
        robotDemand = 0.7f + Random.Range(0f, 0.8f);
        techDemand = 0.7f + Random.Range(0f, 0.8f);
        retroDemand = 0.7f + Random.Range(0f, 0.8f);

        Debug.Log("Спрос обновлён!");
    }


}
