using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacToe.Shared.Data;

namespace TicTacToe.Shared
{
    public interface TttClient
    {
        public Task RegisterCallback(Response response);

        public Task CreateRoomCallback(Response response);

        public Task DisconnectCallback(Response response);
    }
}
