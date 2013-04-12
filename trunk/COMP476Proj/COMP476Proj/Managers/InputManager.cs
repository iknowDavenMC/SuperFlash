using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace COMP476Proj
{
    public class InputManager
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
        /// Private instance
        /// </summary>
        private static volatile InputManager instance = null;
 
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
        private InputManager(ControllerType type)
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
                    gamePadMapping.Add("Dance", new Buttons[1] { Buttons.A });

                    break;

                case ControllerType.Keyboard:

                    keyboardMapping.Add("MenuSelection", new Keys[2] { Keys.Enter, Keys.Space });
                    keyboardMapping.Add("Pause", new Keys[2] { Keys.Pause, Keys.P });
                    keyboardMapping.Add("MenuBack", new Keys[1] { Keys.Escape });
                    keyboardMapping.Add("Down", new Keys[1] { Keys.Down });
                    keyboardMapping.Add("Up", new Keys[1] { Keys.Up });
                    keyboardMapping.Add("Right", new Keys[1] { Keys.Right });
                    keyboardMapping.Add("Left", new Keys[1] { Keys.Left });
                    keyboardMapping.Add("Dance", new Keys[1] { Keys.D });
                    keyboardMapping.Add("Superflash", new Keys[1] { Keys.S });
                    keyboardMapping.Add("Fall", new Keys[1] { Keys.F });

                    break;
            }

            #endregion
        }

        #endregion

        #region Methods

        /// <summary>
        /// Allows the instance to be retrieved. This acts as the constructor
        /// </summary>
        /// <param name="type">Type of the controller to be used</param>
        /// <returns>The only instance of input manager</returns>
        public static InputManager GetInstance(ControllerType type)
        {
            if (instance == null || type != instance.controllerType)
            {
                instance = new InputManager(type);
            }
            
            return instance;
        }

        /// <summary>
        /// Allows the instance to be retrieved. If the instance is null, a new one is instantiated assuming keyboard input.
        /// </summary>
        /// <returns>The only instance of input manager</returns>
        public static InputManager GetInstance()
        {
            if (instance == null)
            {
                instance = new InputManager(ControllerType.Keyboard);
            }

            return instance;
        }

        /// <summary>
        /// Update method
        /// </summary>
        /// <param name="gameTime">Current game time</param>
        public void Update(GameTime gameTime)
        {
            if (instance == null)
            {
                return;
            }

            if (instance.controllerType == ControllerType.GamePad)
            {
                for (int i = 0; i != instance.gamePadStates.Count; ++i)
                {
                    instance.gamePadStates[(PlayerIndex)i] = GamePad.GetState((PlayerIndex)i);
                }
            }
            else
            {
                instance.keyboardState = Keyboard.GetState();
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
            if (instance == null)
            {
                return false;
            }

            // If gamePad
            if (instance.controllerType == ControllerType.GamePad)
            {
                try
                {
                    for (int i = 0; i != instance.gamePadMapping[key].GetLength(0); ++i)
                    {
                        if (instance.gamePadStates[index].IsButtonDown(instance.gamePadMapping[key][i]))
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
                    for (int i = 0; i != instance.keyboardMapping[key].GetLength(0); ++i)
                    {
                        if (instance.keyboardState.IsKeyDown(instance.keyboardMapping[key][i]))
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
