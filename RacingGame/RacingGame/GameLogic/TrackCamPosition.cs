using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace RacingGame.GameLogic
{
    class TrackCamPosition
    {
       
        public int StartSector { get; set; }
        public Vector3 Position { get; set; }

        public TrackCamPosition(int startSector, float x, float y, float z)
        {          
            StartSector = startSector;
            Position = new Vector3(x, y, z);
        }
    }
}
