using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentDay = 1;
    public int money = 1000;
    public float reputation = 0.5f;

    public Text moneyText;
    public Text dayText;
    public Text reputationText;

    public float minReputation = 0f;
    public float maxReputation = 1f;
    public Image reputationBar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"Деньги: {money}₽";
        }

        if (dayText != null)
        {
            dayText.text = $"День: {currentDay}/5";
        }

        if (reputationText != null)
        {
            int reputationPercent = Mathf.RoundToInt(reputation * 100);
            reputationText.text = $"Репутация: {reputationPercent}%";

            // Цвет текста репутации
            if (reputation < 0.3f)
                reputationText.color = Color.red;
            else if (reputation > 0.7f)
                reputationText.color = Color.green;
            else
                reputationText.color = Color.white;
        }

        // Полоска репутации
        if (reputationBar != null)
        {
            reputationBar.fillAmount = reputation;
            reputationBar.color = Color.Lerp(Color.red, Color.green, reputation);
        }
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("Money", money);
        PlayerPrefs.SetInt("Day", currentDay);
        PlayerPrefs.SetFloat("Reputation", reputation);
        PlayerPrefs.Save();
        Debug.Log("💾 Игра сохранена");
    }

    public void LoadGame()
    {
        money = PlayerPrefs.GetInt("Money", 1000);
        currentDay = PlayerPrefs.GetInt("Day", 1);
        reputation = PlayerPrefs.GetFloat("Reputation", 0.5f);
        UpdateUI();
        Debug.Log("📂 Игра загружена");
    }

    void Update()
    {
        // Тест: нажми T чтобы добавить 100 денег
        if (Input.GetKeyDown(KeyCode.T))
        {
            money += 100;
            UpdateUI();
            Debug.Log($"💰 Денег добавлено! Теперь: {money}₽");
        }

        // Тест: нажми R чтобы добавить репутацию
        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeReputation(0.1f);
        }

        // Тест: нажми G чтобы сгенерировать покупателей
        if (Input.GetKeyDown(KeyCode.G))
        {
            CustomerManager cm = FindObjectOfType<CustomerManager>();
            if (cm != null) cm.GenerateCustomers();
        }

        // Тест: нажми I чтобы добавить предмет в инвентарь
        if (Input.GetKeyDown(KeyCode.I))
        {
            ItemDatabase db = FindObjectOfType<ItemDatabase>();
            InventorySystem inv = FindObjectOfType<InventorySystem>();
            if (db != null && inv != null && db.allItems.Count > 0)
            {
                inv.AddItem(db.allItems[Random.Range(0, db.allItems.Count)]);
            }
        }
    }

    public void ChangeReputation(float delta)
    {
        reputation += delta;
        reputation = Mathf.Clamp(reputation, minReputation, maxReputation);
        UpdateUI();

        if (reputation < 0.3f)
        {
            Debug.Log("⚠️ НИЗКАЯ РЕПУТАЦИЯ! Покупатели осторожны");
        }
        else if (reputation > 0.8f)
        {
            Debug.Log("✨ ВЫСОКАЯ РЕПУТАЦИЯ! Все хотят с тобой торговать");
        }
    }

    public float GetReputationBonus()
    {
        // Репутация влияет на цены (от -30% до +30%)
        return 1f + (reputation - 0.5f) * 0.6f;
    }
}
