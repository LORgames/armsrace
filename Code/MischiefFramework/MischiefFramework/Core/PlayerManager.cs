using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.WorldX.Information;

namespace MischiefFramework.Core {
    public class PlayerManager {
        public static List<GamePlayer> ActivePlayers = new List<GamePlayer>();

        private PlayerInput[] Controllers;
        private bool _active = false;

        public PlayerManager() {
            Controllers = new PlayerInput[5];

            Controllers[0] = new InputKeyboardMouse();
            Controllers[1] = new InputGamepad(Microsoft.Xna.Framework.PlayerIndex.One);
            Controllers[2] = new InputGamepad(Microsoft.Xna.Framework.PlayerIndex.Two);
            Controllers[3] = new InputGamepad(Microsoft.Xna.Framework.PlayerIndex.Three);
            Controllers[4] = new InputGamepad(Microsoft.Xna.Framework.PlayerIndex.Four);
        }

        public void Update() {
            if (_active) {

            }
        }

        public void Activate() {
            _active = true;
        }

        public void Deactivate() {
            _active = false;
        }


    }
}
