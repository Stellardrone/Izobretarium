using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeManager : MonoBehaviour
{
    public CustomerManager customerManager;
    public InventorySystem inventory;

    public ItemData selectedItem;
    public Customer selectedCustomer;

    public GameObject tradePanel;
    public Text offerText;
    public Text customerResponseText;

    // Кнопки
    public Button acceptButton;
    public Button rejectButton;
    public Button haggleButton;

    public bool CanCounterOffer = true;
    public int currentOffer;
    private bool warnedCustomer;

    // Флаг, что мы уже торговались
    private bool hasHaggled = false;

    void Start()
    {
        if (tradePanel != null)
            tradePanel.SetActive(false);

        SetupButtons();
    }

    void SetupButtons()
    {
        if (acceptButton != null)
        {
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(AcceptDeal);
            Debug.Log("✅ AcceptButton привязана");
        }

        if (rejectButton != null)
        {
            rejectButton.onClick.RemoveAllListeners();
            rejectButton.onClick.AddListener(RejectDeal);
            Debug.Log("✅ RejectButton привязана");
        }

        if (haggleButton != null)
        {
            haggleButton.onClick.RemoveAllListeners();
            haggleButton.onClick.AddListener(OnHaggleClick); // ← ИСПРАВЛЕНО
            Debug.Log("✅ HaggleButton привязана");
        }
    }

    // 👇 НОВЫЙ МЕТОД - вызывается при нажатии "ТОРГ"
    public void OnHaggleClick()
    {
        Debug.Log("💰 Нажата кнопка ТОРГ");

        if (selectedItem == null || selectedCustomer == null)
        {
            Debug.Log("❌ Нет выбранного предмета или покупателя");
            return;
        }

        if (hasHaggled)
        {
            customerResponseText.text = "Я уже дал лучшее предложение!";
            Debug.Log("❌ Уже торговались");
            return;
        }

        // Текущее предложение
        int currentPrice = customerManager.MakeOffer(selectedCustomer, selectedItem);
        Debug.Log($"💰 Текущая цена: {currentPrice}₽");

        // УВЕЛИЧИВАЕМ В 6 РАЗ!
        int newPrice = currentPrice * 6;
        Debug.Log($"💰 Новая цена (x6): {newPrice}₽");

        // Проверяем бюджет покупателя
        if (newPrice <= selectedCustomer.budget)
        {
            // Покупатель соглашается
            currentOffer = newPrice;
            offerText.text = $"Предложение: {newPrice}₽";
            customerResponseText.text = $"Хорошо! Даю {newPrice}₽, это всё, что у меня есть!";

            // Обновляем предложение в Customer (для AcceptDeal)
            selectedCustomer.currentOffer = newPrice;

            Debug.Log($"✅ Покупатель согласился на {newPrice}₽");
        }
        else
        {
            // У покупателя не хватает денег
            customerResponseText.text = $"У меня только {selectedCustomer.budget}₽, это максимум!";
            offerText.text = $"Предложение: {selectedCustomer.budget}₽ (максимум)";
            currentOffer = selectedCustomer.budget;
            selectedCustomer.currentOffer = selectedCustomer.budget;

            Debug.Log($"⚠️ Покупатель может дать только {selectedCustomer.budget}₽");
        }

        hasHaggled = true;
        CanCounterOffer = false;

        // Делаем кнопку "ТОРГ" неактивной
        if (haggleButton != null)
            haggleButton.interactable = false;
    }

    public void SelectItem(int inventorySlot)
    {
        if (inventory == null)
        {
            Debug.LogError("❌ inventory = null в SelectItem!");
            return;
        }

        if (inventory.items.Count > inventorySlot)
        {
            selectedItem = inventory.items[inventorySlot];
            Debug.Log($"✅ Выбран предмет: {selectedItem.name}");
            TryShowTradePanel();
        }
        else
        {
            Debug.Log($"❌ Нет предмета в слоте {inventorySlot}");
        }
    }

    public void SelectCustomer(Customer customer)
    {
        if (customer == null)
        {
            Debug.LogError("❌ customer = null в SelectCustomer!");
            return;
        }

        selectedCustomer = customer;
        Debug.Log($"✅ Выбран покупатель: {customer.name}");
        TryShowTradePanel();
    }

    void TryShowTradePanel()
    {
        if (selectedItem != null && selectedCustomer != null)
        {
            ShowTradePanel();
        }
        else
        {
            Debug.Log("⏳ Ожидание: нужен и предмет, и покупатель");
        }
    }

    public void ShowTradePanel()
    {
        if (selectedItem == null)
        {
            Debug.LogError("❌ selectedItem = null в ShowTradePanel!");
            return;
        }

        if (selectedCustomer == null)
        {
            Debug.LogError("❌ selectedCustomer = null в ShowTradePanel!");
            return;
        }

        if (customerManager == null)
        {
            Debug.LogError("❌ customerManager = null в ShowTradePanel!");
            return;
        }

        // Сбрасываем флаг торговли для новой сделки
        hasHaggled = false;
        CanCounterOffer = true;

        // Восстанавливаем кнопку "ТОРГ"
        if (haggleButton != null)
            haggleButton.interactable = true;

        int offer = customerManager.MakeOffer(selectedCustomer, selectedItem);
        currentOffer = offer;
        offerText.text = $"Предложение: {offer}₽";
        customerResponseText.text = $"{selectedCustomer.name} хочет купить {selectedItem.name}";

        tradePanel.SetActive(true);

        Debug.Log($"✅ TradePanel открыта. Начальное предложение: {offer}₽");
    }

    public void AcceptDeal()
    {
        Debug.Log("🔘 AcceptDeal нажата!");

        if (selectedItem == null || selectedCustomer == null) return;

        ItemData soldItem = selectedItem;
        Customer buyer = selectedCustomer;
        string itemCondition = soldItem.condition;
        string customerType = buyer.type;

        // Используем currentOffer, а не MakeOffer
        int price = currentOffer > 0 ? currentOffer : customerManager.MakeOffer(buyer, soldItem);

        GameManager.Instance.money += price;
        inventory.RemoveItem(soldItem);
        customerManager.currentCustomers.Remove(buyer);
        customerManager.UpdateCustomerUI();

        // Репутация
        if (itemCondition == "broken" && warnedCustomer)
        {
            GameManager.Instance.ChangeReputation(0.05f);
        }
        else if (itemCondition == "broken" && !warnedCustomer)
        {
            GameManager.Instance.ChangeReputation(-0.15f);
        }
        else if (customerType == "student" && price < soldItem.basePrice)
        {
            GameManager.Instance.ChangeReputation(0.03f);
        }
        else
        {
            GameManager.Instance.ChangeReputation(0.02f);
        }

        GameManager.Instance.UpdateUI();
        tradePanel.SetActive(false);

        Debug.Log($"✅ Продано {soldItem.name} за {price}₽!");

        selectedItem = null;
        selectedCustomer = null;
        warnedCustomer = false;
        currentOffer = 0;
    }

    public void RejectDeal()
    {
        Debug.Log("🔘 RejectDeal нажата!");

        if (selectedCustomer != null)
        {
            if (Random.Range(0f, 1f) > 0.5f)
            {
                customerManager.currentCustomers.Remove(selectedCustomer);
                customerManager.UpdateCustomerUI();
                Debug.Log("❌ Покупатель ушёл!");
                GameManager.Instance.ChangeReputation(-0.01f);
            }
            else
            {
                Debug.Log("⏳ Покупатель остался");
            }
        }

        tradePanel.SetActive(false);
        selectedItem = null;
        selectedCustomer = null;
        currentOffer = 0;
    }
}



