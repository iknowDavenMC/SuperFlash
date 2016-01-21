using System;
using System.Collections.Generic;

using UnityEngine;
using Assets.Code._XNA;

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
        private GamePadState gamePadState;
        
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

        #region Properties
        public ControllerType GetControllerType
        {
            get { return controllerType; }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        private InputManager()
        {
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                controllerType = ControllerType.GamePad;
                gamePadMapping = new Dictionary<string, Buttons[]>();
            }
            else
            {
                controllerType = ControllerType.Keyboard;
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
                    gamePadMapping.Add("Superflash", new Buttons[1] { Buttons.X });

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
        public static InputManager GetInstance()
        {
            if (instance == null)
            {
                instance = new InputManager();
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
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    instance.gamePadState = GamePad.GetState(PlayerIndex.One);
                }
                else
                {
                    instance = null;
                }
            }
            else
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    instance = null;
                }
                else
                {
                    instance.keyboardState = Keyboard.GetState();
                }
            }
        }

        /// <summary>
        /// Returns whether or not the player is performing the given action
        /// </summary>
        /// <param name="key">Name of the action to check for</param>
        /// <param name="index">Index of the player in question</param>
        /// <returns>Whether or not the action exists</returns>
        public bool IsDoing(string key, PlayerIndex index)
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
                        if (instance.gamePadState.IsButtonDown(instance.gamePadMapping[key][i]))
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
