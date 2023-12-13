using TicTacToe.Server.Entities;

namespace TicTacToe.Server.Services
{
    public class MemoryRoomStorage
    {
        private Dictionary<string, Room> roomNumberToRoom = new Dictionary<string, Room>();

        public void AddRoom(string roomNumber, Room room) {
            this.roomNumberToRoom[roomNumber] = room;
        }

        public void RemoveRoom(string roomNumber)
        {
            this.roomNumberToRoom.Remove(roomNumber);
        }

        public Room GetRoom(string roomNumber) {
            return this.roomNumberToRoom[roomNumber];
        }

        public bool HasRoom(string roomNumber) { 
            return roomNumberToRoom.ContainsKey(roomNumber);
        }
    }
}
