namespace StriveUp.Shared.Interfaces
{
    public interface INotificationStateService
    {
        /// <summary>
        /// Aktualna liczba nieprzeczytanych powiadomień.
        /// </summary>
        int UnreadCount { get; set; }

        /// <summary>
        /// Zdarzenie wywoływane przy zmianie liczby powiadomień.
        /// </summary>
        event Action? OnChange;

        /// <summary>
        /// Odświeża liczbę nieprzeczytanych powiadomień z serwera.
        /// </summary>
        Task RefreshUnreadCountAsync();
    }
}