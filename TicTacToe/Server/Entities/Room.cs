namespace TicTacToe.Server.Entities
{
    public class Room
    {
        public string? RoomNumber { get; set; }
        public List<List<Cell>> gameState { get; set; } = new List<List<Cell>>();
        public User? firstUser;
        public User? secondUser;
        public string? password;
        public User? lastStep;
        public GameStatus status = GameStatus.BeforeStart;

        public bool IsPossibleToAddNewUser() 
        {
            return firstUser == null || secondUser == null;
        }

        public bool CanAddUserWithUsername(string username)
        {
            if(IsPossibleToAddNewUser())
            {
                if(firstUser?.Username != username && secondUser?.Username != username)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
