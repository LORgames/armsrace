using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.Core;
using MischiefFramework.WorldX.Assets;

namespace MischiefFramework.WorldX.Information {
    public class GamePlayer {
        public PlayerInput Input;
        public int playerID;
        public int teamID;
        public int baseID;

        public int score;

        public Character character;

        public GamePlayer(PlayerInput input, int playerID) {
            this.Input = input;
            this.playerID = playerID;
        }
    }
}
