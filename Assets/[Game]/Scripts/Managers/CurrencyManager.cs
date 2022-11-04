using Assets._Game_.Scripts.Entities;
using Sirenix.OdinInspector;
using TriflesGames.ManagerFramework;
using TriflesGames.Managers;
using UnityEngine;

public class CurrencyManager : Manager<CurrencyManager>
{
    [Title("# Currency Settings #")]
    public int defaultCurrencyAmount;

    public CurrencyData currencyData { get; private set; }

    protected override void MB_Start()
    {
        currencyData = CurrencyData.Get();
        if (currencyData == null)
        {
            currencyData = new CurrencyData();
            bool isSuccess = currencyData.Register();
            if (!isSuccess) Debug.LogError("Currency Data Entity register error!");
        }

        // Load currency data
        currencyData.Load();
        if (currencyData.Gold < defaultCurrencyAmount)
            currencyData.Gold = defaultCurrencyAmount;
    }

    protected override void MB_Listen(bool status)
    {
        if (status)
        {
            GameManager.Instance.Subscribe(ManagerEvents.GameStatus_Init, OnGameInit);
        }
        else
        {
            GameManager.Instance.Unsubscribe(ManagerEvents.GameStatus_Init, OnGameInit);
        }
    }

    private void OnGameInit(object[] arguments) => Publish(ManagerEvents.ShowCurrency, currencyData.Gold);

    #region Currency Manager API

    public void AddCurrency(int amount) => currencyData.AddCurrency(amount);
    public void SubstractCurrency(int amount) => currencyData.SubstractCurrency(amount);

    #endregion
}