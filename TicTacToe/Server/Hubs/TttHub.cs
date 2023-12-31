﻿using Microsoft.AspNetCore.SignalR;
using TicTacToe.Server.Entities;
using TicTacToe.Server.Services;
using TicTacToe.Shared;
using TicTacToe.Shared.Data;
using TicTacToe.Client.Models.Enums;

namespace TicTacToe.Server.Hubs
{

    public class TttHub : Hub<TttClient>
    {
        private readonly MemoryRoomStorage _roomStorage;
        public TttHub(MemoryRoomStorage storage)
        {
            _roomStorage = storage;
        }

        public async Task CreateRoom(string password, string username)
        {
            string roomNumber = Guid.NewGuid().ToString();
            var user = new User { ConnectionId = Context.ConnectionId, Username = username, SignVariant = SignVariant.Cross };
            var room = new Room { firstUser = user, RoomNumber = roomNumber, password = password };
            _roomStorage.AddRoom(roomNumber, room);

            await Groups.AddToGroupAsync(Context.ConnectionId, roomNumber);
            await Clients.Caller.CreateRoomCallback(new Response { Success = true, Message = "Successfully created new room.", RoomNumber = roomNumber });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var room = _roomStorage.GetRoomByConnectionIdOfUser(Context.ConnectionId);
            if (room != null)
            {
                var user = room?.firstUser?.ConnectionId == Context.ConnectionId ? room?.secondUser : room?.firstUser;

                await Clients.Client(user?.ConnectionId).DisconnectCallback(new Response { Success = false, Message = "Second user has terminated the connection.", RoomNumber = room?.RoomNumber });

                await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.RoomNumber);
                await Groups.RemoveFromGroupAsync(user.ConnectionId, room.RoomNumber);

                _roomStorage.RemoveRoom(room.RoomNumber);
            }
        }

        public async Task TapButton(string roomNumber, int i, int j) {
            var room = _roomStorage.GetRoom(roomNumber);
            var receiver = room.firstUser.ConnectionId == Context.ConnectionId ? room.secondUser : room.firstUser;
            Console.WriteLine($"request conn id {Context.ConnectionId}");
            Console.WriteLine($"room person conn id {room.firstUser.ConnectionId} {room.secondUser.ConnectionId}");
            Console.WriteLine($"receiver conn id {receiver.ConnectionId}");
            await Clients.Client(receiver.ConnectionId).TapCallback(i, j);
        }

        public async Task WinnerEndpoint(GameState state, string roomNumber) {
            var room = _roomStorage.GetRoom(roomNumber);
            var winnerName = state == GameState.firstWin ? room.firstUser.Username : room.secondUser.Username;
            await Clients.Client(room.firstUser.ConnectionId).WinnerCallback(new WinnerResponse { winnerName = winnerName });
            await Clients.Client(room.secondUser.ConnectionId).WinnerCallback(new WinnerResponse { winnerName = winnerName });
            await Groups.RemoveFromGroupAsync(room.firstUser.ConnectionId, room.RoomNumber);
            await Groups.RemoveFromGroupAsync(room.secondUser.ConnectionId, room.RoomNumber);
            _roomStorage.RemoveRoom(room.RoomNumber);
        }

        public async Task Register(string roomNumber, string password, string username)
        {
            if (_roomStorage.HasRoom(roomNumber))
            {
                var room = _roomStorage.GetRoom(roomNumber);
                if (room.IsPossibleToAddNewUser())
                {
                    if (room.CanAddUserWithUsername(username))
                    {
                        if (room.password == password)
                        {
                            var user = new User { Username = username, ConnectionId = Context.ConnectionId, SignVariant = SignVariant.Circle };
                            room.secondUser = user;
                            await Groups.AddToGroupAsync(Context.ConnectionId, roomNumber);
                            await Clients.Group(roomNumber).RegisterCallback(new Response { Success = true, Message = $"The step of {room?.firstUser?.Username} user.", RoomNumber = roomNumber });
                            await Clients.Client(room?.firstUser?.ConnectionId).UpdateSign(new SignResponse { Sign = "X" });
                            await Clients.Client(room?.secondUser?.ConnectionId).UpdateSign(new SignResponse { Sign = "O" });
                        }
                        else
                        {
                            await Clients.Caller.RegisterCallback(new Response { Success = false, Message = "Invalid room password.", RoomNumber = roomNumber });
                        }
                    }
                    else
                    {
                        await Clients.Caller.RegisterCallback(new Response { Success = false, Message = "User with such username is already in the room.", RoomNumber = roomNumber });
                    }
                }
                else
                {
                    await Clients.Caller.RegisterCallback(new Response { Success = false, Message = "Already two users in a game room.", RoomNumber = roomNumber });
                }
            }
            else
            {
                await Clients.Caller.RegisterCallback(new Response { Success = false, Message = "No such room in room space.", RoomNumber = roomNumber });
            }
        }
    }
}
