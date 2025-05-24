using StriveUp.Shared.Interfaces;

namespace StriveUp.Infrastructure.Services
{
    public class MedalStateService : IMedalStateService
    {
        public event Action OnChange;

        public int MedalsToClaim { get; private set; }

        public void SetMedalsCount(int count)
        {
            MedalsToClaim = count;
            NotifyStateChanged();
        }

        public void DecrementMedalsCount()
        {
            if (MedalsToClaim > 0)
            {
                MedalsToClaim--;
                NotifyStateChanged();
            }
        }

        public void IncrementMedalsCount()
        {
            if (MedalsToClaim > 0)
            {
                MedalsToClaim++;
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}