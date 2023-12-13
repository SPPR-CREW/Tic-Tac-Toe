using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Shared.Data
{
    public class Response
    {
        public string? Message { get; set; }
        public bool? Success { get; set; }
        public string? RoomNumber { get; set; }
    }
}
