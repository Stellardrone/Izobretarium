using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // ← ЭТО ВАЖНО ДЛЯ UI!

public class InventorySystem : MonoBehaviour
{
    public List<ItemData> items = new List<ItemData>();
    public int maxSlots = 3;

    // ДОБАВЛЯЕМ UI ЭЛЕМЕНТЫ
    public GameObject[] inventorySlots;  // Ссылки на слоты (Panels/Buttons)
    public Image[] slotIcons;            // Ссылки на иконки в слотах
    public Text[] slotTexts;             // Ссылки на тексты в слотах

    void Start()
    {
        // При старте обновляем UI
        UpdateInventoryUI();
    }

    public bool AddItem(ItemData item)
    {
        if (items.Count >= maxSlots)
        {
            Debug.Log("❌ Инвентарь полон!");
            return false;
        }

        items.Add(item);
        UpdateInventoryUI();
        Debug.Log($"✅ {item.name} добавлен в инвентарь");

        //   обновляем UI покупателей
        CustomerManager cm = FindObjectOfType<CustomerManager>();
        if (cm != null)
        {
            cm.UpdateCustomerUI();
        }

        return true;
    }

    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
        UpdateInventoryUI(); // ← ВАЖНО: обновляем экран
        Debug.Log($"🗑️ {item.name} удалён из инвентаря");
    }

    public bool HasSpace()
    {
        return items.Count < maxSlots;
    }

    // НОВЫЙ МЕТОД: обновление интерфейса
    public void UpdateInventoryUI()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            if (i < items.Count)
            {
                ItemData item = items[i];

                // Показываем название
                if (slotTexts != null && i < slotTexts.Length && slotTexts[i] != null)
                    slotTexts[i].text = item.name;

                // Показываем иконку
                if (slotIcons != null && i < slotIcons.Length && slotIcons[i] != null)
                {
                    if (item.icon != null)
                    {
                        slotIcons[i].sprite = item.icon;
                        slotIcons[i].gameObject.SetActive(true);

                        // Меняем цвет в зависимости от состояния
                        switch (item.condition)
                        {
                            case "new":
                                slotIcons[i].color = Color.white;
                                break;
                            case "used":
                                slotIcons[i].color = new Color(0.7f, 0.7f, 0.7f);
                                break;
                            case "broken":
                                slotIcons[i].color = new Color(0.5f, 0.3f, 0.3f);
                                break;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ У предмета {item.name} нет иконки!");
                    }
                }
            }
            else
            {
                // Пустой слот
                if (slotTexts != null && i < slotTexts.Length && slotTexts[i] != null)
                    slotTexts[i].text = "Пусто";

                if (slotIcons != null && i < slotIcons.Length && slotIcons[i] != null)
                    slotIcons[i].gameObject.SetActive(false);
            }
        }
    }

    // ПОЛЕЗНЫЙ МЕТОД: получить предмет по индексу слота
    public ItemData GetItemAtSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < items.Count)
            return items[slotIndex];
        return null;
    }

    // ПОЛЕЗНЫЙ МЕТОД: удалить предмет по индексу слота
    public void RemoveItemAtSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < items.Count)
        {
            items.RemoveAt(slotIndex);
            UpdateInventoryUI();
        }
    }
}
