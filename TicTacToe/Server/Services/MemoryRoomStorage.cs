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

        public Room? GetRoomByConnectionIdOfUser(string connectionId) { 
            foreach (var room in  roomNumberToRoom.Values)
            {
                if (room?.firstUser?.ConnectionId == connectionId || room?.secondUser?.ConnectionId == connectionId)
                {
                    return room;
                }
            }
            return null;
        }
    }
}
