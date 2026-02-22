using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // для перезагрузки сцены

public class DayManager : MonoBehaviour
{
    public Button endDayButton;  // Кнопка "Закончить день"

    public int rentPerDay = 100;  // Аренда в день
    public int targetMoney = 10000; // Цель для победы

    // Ссылки на UI победы
    public GameObject winPanel;
    public Text winStatsText;
    public Button menuButton;

    void Start()
    {
        if (endDayButton != null)
        {
            endDayButton.onClick.RemoveAllListeners();
            endDayButton.onClick.AddListener(EndDay);
            Debug.Log("✅ Кнопка конца дня привязана");
        }
        else
        {
            Debug.LogError("❌ endDayButton не назначена в Inspector!");
        }

        // Скрываем панель победы при старте
        if (winPanel != null)
            winPanel.SetActive(false);

        // Настраиваем кнопку меню
        if (menuButton != null)
        {
            menuButton.onClick.RemoveAllListeners();
            //menuButton.onClick.AddListener(GoToMenu);
        }
    }

    public void EndDay()
    {
        Debug.Log($"📅 День {GameManager.Instance.currentDay} закончен");

        // 1. Проверяем, не конец ли игры
        if (GameManager.Instance.currentDay >= 5)
        {
            // Это последний день - показываем победу
            ShowWinScreen();
            return;
        }

        // 2. Снимаем аренду
        GameManager.Instance.money -= rentPerDay;
        Debug.Log($"💰 Аренда: -{rentPerDay}₽");

        // 3. Переходим на следующий день
        GameManager.Instance.currentDay++;

        // 4. Обновляем UI
        GameManager.Instance.UpdateUI();

        // 5. Генерируем новые лоты на аукционе
        AuctionManager auction = FindObjectOfType<AuctionManager>();
        if (auction != null)
        {
            auction.GenerateDailyLots();
            Debug.Log("🔄 Новые лоты сгенерированы");
        }

        // 6. Генерируем новых покупателей
        CustomerManager customers = FindObjectOfType<CustomerManager>();
        if (customers != null)
        {
            customers.GenerateCustomers();
            Debug.Log("🔄 Новые покупатели пришли");
        }

        // 7. Обновляем спрос
        PriceCalculator priceCalc = FindObjectOfType<PriceCalculator>();
        if (priceCalc != null)
        {
            priceCalc.RandomizeDemand();
        }

        Debug.Log($"📅 Наступил день {GameManager.Instance.currentDay}");

        // 8. Проверяем банкротство
        if (GameManager.Instance.money < 0)
        {
            Debug.Log("💸 БАНКРОТСТВО! Игра окончена.");
            // Здесь можно показать экран поражения
        }
    }

    // - показывает экран победы
    void ShowWinScreen()
    {
        Debug.Log("🏆 ИГРА ПРОЙДЕНА!");

        // Скрываем кнопку конца дня
        if (endDayButton != null)
            endDayButton.gameObject.SetActive(false);

        // Обновляем текст с результатами
        //if (winStatsText != null)
       // {
        //    int reputationPercent = Mathf.RoundToInt(GameManager.Instance.reputation * 100);
           // winStatsText.text = $"Денег заработано: {GameManager.Instance.money}₽\n" +
              //                  $"Репутация: {reputationPercent}%\n" +
                  //              $"Дней: {GameManager.Instance.currentDay}/5\n\n" +
                     //           $"Цель была: {targetMoney}₽\n" +
                         //       (GameManager.Instance.money >= targetMoney ? "✅ Цель достигнута!" : "❌ Цель не достигнута");
     // }

        // Показываем панель
        if (winPanel != null)
            winPanel.SetActive(true);
    }

    
    //void GoToMenu()
    //{
    //    Debug.Log("🔙 Возврат в меню");

        // Здесь можно загрузить сцену меню
        // SceneManager.LoadScene("MainMenu");

        // Пока просто перезапускаем текущую сцену
      //  SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //}
}
