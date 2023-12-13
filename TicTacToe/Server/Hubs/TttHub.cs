using Microsoft.AspNetCore.SignalR;
using TicTacToe.Server.Entities;
using TicTacToe.Shared;
using TicTacToe.Shared.Data;

namespace TicTacToe.Server.Hubs
{

    public class TttHub : Hub<TttClient>
    {
        private Dictionary<string, Room> roomNumberToRoom = new Dictionary<string, Room>(); 

        public async Task CreateRoom(string username, string password)
        {
            string roomNumber = Guid.NewGuid().ToString();
            var user = new User { ConnectionId=Context.ConnectionId, Username=username, SignVariant=SignVariant.Cross};
            var room = new Room { firstUser = user, RoomNumber = roomNumber, password = password };
            roomNumberToRoom[roomNumber] = room;

            await Groups.AddToGroupAsync(Context.ConnectionId, roomNumber);
            await Clients.Caller.CreateRoomCallback(new Response {Success=true, Message="Successfully created new room.", RoomNumber=roomNumber});
        }

        public async Task Register(string roomNumber, string username, string password)
        {
            if (roomNumberToRoom.ContainsKey(roomNumber))
            {
                var room = roomNumberToRoom[roomNumber];
                if (room.IsPossibleToAddNewUser())
                {
                    if (room.CanAddUserWithUsername(username))
                    {
                        if (room.password == password)
                        {
                            var user = new User { Username = username, ConnectionId = Context.ConnectionId, SignVariant = SignVariant.Circle };
                            room.secondUser = user;
                            await Groups.AddToGroupAsync(Context.ConnectionId, roomNumber);
                            await Clients.Group(roomNumber).RegisterCallback(new Response { Success = true, Message = "Successfully joined room.", RoomNumber = roomNumber });
                        }
                        else {
                            await Clients.Caller.RegisterCallback(new Response { Success = false, Message = "Invalid room password.", RoomNumber = roomNumber });
                        }
                    }
                    else {
                        await Clients.Caller.RegisterCallback(new Response { Success = false, Message = "User with such username is already in the room.", RoomNumber = roomNumber });
                    }
                }
                else {
                    await Clients.Caller.RegisterCallback(new Response { Success = false, Message = "Already two users in a game room." , RoomNumber=roomNumber});
                }
            }
            else {
                await Clients.Caller.RegisterCallback(new Response { Success = false, Message = "No such room in room space.", RoomNumber = roomNumber });
            }
        }
    }
}
