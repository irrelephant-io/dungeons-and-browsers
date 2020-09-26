namespace Irrelephant.DnB.Server.Authentication.Models
{
    public class IdTokenValidationResult
    {
        public bool IsValid { get; set; }

        public string Email { get; set; }

        public string DisplayName { get; set; }
    }
}
