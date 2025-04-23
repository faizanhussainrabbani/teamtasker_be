namespace TeamTasker.Application.Common.Interfaces
{
    /// <summary>
    /// Interface for getting the current user
    /// </summary>
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string Username { get; }
        bool IsAuthenticated { get; }
    }
}
