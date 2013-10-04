using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MischiefFramework.WorldX.Information;

namespace MischiefFramework.Core {
    public class PlayerManager {
        private const int MAX_ACTIVE = 4;
        public static List<GamePlayer> ActivePlayers = new List<GamePlayer>();

        private const int TOTAL_INPUTS = 5;
        private PlayerInput[] Controllers;
        private bool _active = false;

        public PlayerManager() {
            Controllers = new PlayerInput[TOTAL_INPUTS];

            Controllers[0] = new InputKeyboardMouse();
            Controllers[1] = new InputGamepad(Microsoft.Xna.Framework.PlayerIndex.One);
            Controllers[2] = new InputGamepad(Microsoft.Xna.Framework.PlayerIndex.Two);
            Controllers[3] = new InputGamepad(Microsoft.Xna.Framework.PlayerIndex.Three);
            Controllers[4] = new InputGamepad(Microsoft.Xna.Framework.PlayerIndex.Four);
        }

        public void Update(float dt) {
            int i = ActivePlayers.Count;
            while (--i > -1) {
                ActivePlayers[i].Input.Update(dt);
            }

            if (_active && ActivePlayers.Count < MAX_ACTIVE) {
                i = TOTAL_INPUTS;
                while (--i > -1) {
                    if (!Controllers[i].isUsed) {
                        Controllers[i].Update(dt);

                        if (Controllers[i].GetStart()) {
                            Controllers[i].isUsed = true;
                            ActivePlayers.Add(new GamePlayer(Controllers[i], ActivePlayers.Count));
                        }
                    }
                }
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
