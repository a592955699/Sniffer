using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sniffer.Login.VerificationCodes.Slide
{
    public class MoveEntity
    {
        public MoveEntity() { }
        public MoveEntity(int x,int y,int millisecondsTimeout) {
            X = x;
            Y = y;
            MillisecondsTimeout = millisecondsTimeout;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public int MillisecondsTimeout { get; set; }
    }
}
