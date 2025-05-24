namespace StriveUp.Shared.Interfaces
{
    public interface IMedalStateService
    {
        public event Action OnChange;

        int MedalsToClaim { get; }

        void SetMedalsCount(int count);

        void DecrementMedalsCount();

        void IncrementMedalsCount();
    }
}