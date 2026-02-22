using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriceTest : MonoBehaviour
{
 /*/   void Start()
    {
        // Создаём тестовый предмет
        ItemData testItem = new ItemData
        {
            basePrice = 500,
            condition = "used",
            rarity = "rare"
        };

        // Тест 1: Базовая цена
        Debug.Log($"База: {testItem.basePrice}");

        // Тест 2: С учётом состояния
        Debug.Log($"Состояние (used): {testItem.basePrice * testItem.ConditionMultiplier()}");

        // Тест 3: С учётом редкости
        float withRarity = testItem.basePrice * testItem.ConditionMultiplier() * testItem.RarityMultiplier();
        Debug.Log($"+ редкость: {withRarity}");

        // Тест 4: С учётом спроса
        float withDemand = withRarity * 1.5f;
        Debug.Log($"+ спрос (высокий): {withDemand}");

        // Тест 5: Финальная формула
        int final = testItem.GetFinalPrice(1.5f, 1.2f);
        Debug.Log($"Финальная (спрос×1.5, репутация×1.2): {final}₽");
    }/*/
}
