namespace BenaaInvitations.API.Services
{
    public interface IAIDesignService
    {
        Task<string> GenerateInvitationBackgroundAsync(string prompt);
    }
}
