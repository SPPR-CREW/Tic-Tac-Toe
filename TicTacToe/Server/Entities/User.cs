namespace TicTacToe.Server.Entities
{
    public class User
    {
        public string? ConnectionId { get; set; }
        public string? Username { get; set; }
        public SignVariant? SignVariant { get; set; } = null;
    }
}
