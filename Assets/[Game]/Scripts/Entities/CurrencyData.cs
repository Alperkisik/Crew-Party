using TriflesGames.ManagerFramework;

namespace Assets._Game_.Scripts.Entities
{
    public class CurrencyData : Entity<CurrencyData>
    {
        #region Variables

        public int Gold;

        #endregion

        protected override bool Init() => true;

        public void AddCurrency(int amount)
        {
            Gold += amount;
            Save();
        }

        public void SubstractCurrency(int amount)
        {
            Gold -= amount;
            Save();
        }
    }
}