using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace COMP476Proj
{
    public class ImputManager : GameComponent
    {
        #region Enumerations

        /// <summary>
        /// Type of controller being used
        /// </summary>
        public enum ControllerType
        { Keyboard, GamePad }

        #endregion

        #region Attributes

        /// <summary>
        /// Controller type
        /// </summary>
        private ControllerType controllerType;

        /// <summary>
        /// States of the different gamepads
        /// </summary>
        private Dictionary<PlayerIndex, GamePadState> gamePadStates;
        
        /// <summary>
        /// States of the keyboard
        /// </summary>
        private KeyboardState keyboardState;

        /// <summary>
        /// Mapping of keyboard keys to actions
        /// </summary>
        private Dictionary<String, Keys[]> keyboardMapping;

        /// <summary>
        /// Mapping of gamepad buttons to actions
        /// </summary>
        private Dictionary<String, Buttons[]> gamePadMapping;

        #endregion

        #region Constructors

        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="game">Current game instance</param>
        /// <param name="type">Type of controller to check for input</param>
        /// <param name="numberOfPlayers">Number of players whose inputs must be checked</param>
        public ImputManager(Game game, ControllerType type)
            : base(game)
	    {
            controllerType = type;

            if (controllerType == ControllerType.GamePad)
            {
                gamePadStates = new Dictionary<PlayerIndex, GamePadState>();

                PlayerIndex index = PlayerIndex.One;

                for (int i = 0; i != 4; ++i, ++index)
                {
                    if (GamePad.GetState(index).IsConnected)
                    {
                        gamePadStates.Add(index, GamePad.GetState(index));
                    }
                }

                gamePadMapping = new Dictionary<string, Buttons[]>();
            }
            else 
            {
                keyboardMapping = new Dictionary<string, Keys[]>();
            }

            #region Button Mappings

            // TODO
            // Assign the various actions to the mapping dictionaries
            // ex: gamePadMapping.Add("MenuSelection", new Buttons[2] {Buttons.A, Buttons.Start});
            switch (controllerType)
            {
                case ControllerType.GamePad:

                    gamePadMapping.Add("MenuSelection", new Buttons[2] { Buttons.A, Buttons.Start });
                    gamePadMapping.Add("Pause", new Buttons[1] { Buttons.Start });
                    gamePadMapping.Add("MenuBack", new Buttons[1] { Buttons.B });
                    gamePadMapping.Add("Down", new Buttons[2] { Buttons.DPadDown, Buttons.LeftThumbstickDown });
                    gamePadMapping.Add("Up", new Buttons[2] { Buttons.DPadUp, Buttons.LeftThumbstickUp });
                    gamePadMapping.Add("Right", new Buttons[2] { Buttons.DPadRight, Buttons.LeftThumbstickRight });
                    gamePadMapping.Add("Left", new Buttons[2] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft });

                    // TODO
                    // ...

                    break;

                case ControllerType.Keyboard:

                    keyboardMapping.Add("MenuSelection", new Keys[2] { Keys.Enter, Keys.Space });
                    keyboardMapping.Add("Pause", new Keys[2] { Keys.Pause, Keys.P });
                    keyboardMapping.Add("MenuBack", new Keys[1] { Keys.Escape });

                    switch (playerIndex)
                    {
                        case PlayerIndex.One:

                            keyboardMapping.Add("Down", new Keys[1] { Keys.Down });
                            keyboardMapping.Add("Up", new Keys[1] { Keys.Up });
                            keyboardMapping.Add("Right", new Keys[1] { Keys.Right });
                            keyboardMapping.Add("Left", new Keys[1] { Keys.Left });

                            // ...

                            break;

                        case PlayerIndex.Two:

                            keyboardMapping.Add("Down", new Keys[1] { Keys.S });
                            keyboardMapping.Add("Up", new Keys[1] { Keys.W });
                            keyboardMapping.Add("Right", new Keys[1] { Keys.D });
                            keyboardMapping.Add("Left", new Keys[1] { Keys.A });

                            // ...

                            break;

                        case PlayerIndex.Three:

                            // ...

                            break;

                        case PlayerIndex.Four:

                            // ...

                            break;
                    }

                    break;
            }

            #endregion
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update method called automatically
        /// </summary>
        /// <param name="gameTime">Current game time</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (controllerType == ControllerType.GamePad)
            {
                gamePadState = GamePad.GetState(playerIndex);

            }
            else
            {
                keyboardState = Keyboard.GetState();
            }
        }

        /// <summary>
        /// Returns whether or not the player is performing the given action
        /// </summary>
        /// <param name="key">Name of the action to check for</param>
        /// <param name="index">Index of the player in question</param>
        /// <returns></returns>
        public bool IsDoing(String key, PlayerIndex index)
        {
            // If gamePad
            if (controllerType == ControllerType.GamePad)
            {
                try
                {
                    for (int i = 0; i != gamePadMapping[key].GetLength(0); ++i)
                    {
                        if (gamePadStates[index].IsButtonDown(gamePadMapping[key][i]))
                        {
                            return true;
                        }
                    }

                    return false;
                }
                catch (KeyNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            // If keyboard
            else
            {
                try
                {
                    for (int i = 0; i != keyboardMapping[key].GetLength(0); ++i)
                    {
                        if (keyboardState.IsKeyDown(keyboardMapping[key][i]))
                        {
                            return true;
                        }
                    }

                    return false;
                }
                catch (KeyNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
        }

        #endregion
    }
}
