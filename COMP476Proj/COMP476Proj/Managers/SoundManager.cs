using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace COMP476Proj
{
    public class SoundManager
    {
        #region Attributes

        /// <summary>
        /// Private instance
        /// </summary>
        private static volatile SoundManager instance = null;

        /// <summary>
        /// Mapping of keyboard keys to actions
        /// </summary>
        private Dictionary<string, Dictionary<string, List<SoundEffect>>> soundEffects;

        /// <summary>
        /// Mapping of gamepad buttons to actions
        /// </summary>
        private Dictionary<String, Song> songs;

        private const float distanceThreshold = 300;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        private SoundManager()
        {
            soundEffects = new Dictionary<string, Dictionary<string, List<SoundEffect>>>();

            soundEffects.Add("Streaker", new Dictionary<string,List<SoundEffect>>());
            soundEffects.Add("Girl", new Dictionary<string,List<SoundEffect>>());
            soundEffects.Add("WhiteBoy", new Dictionary<string,List<SoundEffect>>());
            soundEffects.Add("BlackBoy", new Dictionary<string,List<SoundEffect>>());
            soundEffects.Add("DumbCop", new Dictionary<string,List<SoundEffect>>());
            soundEffects.Add("SmartCop", new Dictionary<string,List<SoundEffect>>());
            soundEffects.Add("RoboCop", new Dictionary<string,List<SoundEffect>>());
            soundEffects.Add("Other", new Dictionary<string,List<SoundEffect>>());
            soundEffects.Add("Common", new Dictionary<string, List<SoundEffect>>());

            soundEffects["Streaker"].Add("SuperFlash", new List<SoundEffect>(1));
            soundEffects["Streaker"].Add("Dance", new List<SoundEffect>(1));

            soundEffects["Girl"].Add("Exclamation", new List<SoundEffect>(3));
            soundEffects["Girl"].Add("SuperFlash", new List<SoundEffect>(1));

            soundEffects["WhiteBoy"].Add("Exclamation", new List<SoundEffect>(3));
            soundEffects["WhiteBoy"].Add("SuperFlash", new List<SoundEffect>(1));

            soundEffects["BlackBoy"].Add("Exclamation", new List<SoundEffect>(3));
            soundEffects["BlackBoy"].Add("SuperFlash", new List<SoundEffect>(1));

            soundEffects["DumbCop"].Add("Exclamation", new List<SoundEffect>(6));
            soundEffects["DumbCop"].Add("SuperFlash", new List<SoundEffect>(2));
            soundEffects["DumbCop"].Add("Hit", new List<SoundEffect>(4));

            soundEffects["SmartCop"].Add("Exclamation", new List<SoundEffect>(6));
            soundEffects["SmartCop"].Add("SuperFlash", new List<SoundEffect>(2));
            soundEffects["SmartCop"].Add("Hit", new List<SoundEffect>(4));

            soundEffects["RoboCop"].Add("Activation", new List<SoundEffect>(1));

            soundEffects["Other"].Add("Achievement", new List<SoundEffect>(1));
            soundEffects["Other"].Add("Meter", new List<SoundEffect>(1));
            soundEffects["Other"].Add("Consumable", new List<SoundEffect>(1));
            soundEffects["Other"].Add("Score", new List<SoundEffect>(1));
            soundEffects["Other"].Add("SuperFlash Ready", new List<SoundEffect>(1));

            soundEffects["Common"].Add("Collide", new List<SoundEffect>(1));
            soundEffects["Common"].Add("Fall", new List<SoundEffect>(1));
            soundEffects["Common"].Add("Run", new List<SoundEffect>(1));
            soundEffects["Common"].Add("Hit", new List<SoundEffect>(1));

            songs = new Dictionary<string, Song>();

            MediaPlayer.Volume = 0.5f;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Allows the instance to be retrieved. This acts as the constructor
        /// </summary>
        /// <param name="type">Type of the controller to be used</param>
        /// <returns>The only instance of input manager</returns>
        public static SoundManager GetInstance()
        {
            if (instance == null)
            {
                instance = new SoundManager();
            }
            
            return instance;
        }

        /// <summary>
        /// Load the songs and sounds
        /// </summary>
        /// <param name="content">Content manager</param>
        public void LoadContent(ContentManager content)
        {
            // Load sound effects
            foreach (KeyValuePair<string, Dictionary<string, List<SoundEffect>>> soundSource in soundEffects)
            {
                foreach (KeyValuePair<string, List<SoundEffect>> soundType in soundSource.Value)
                {
                    for (int i = 0; i != soundType.Value.Capacity; ++i)
                    {
                        soundType.Value.Add(content.Load<SoundEffect>("Sound Effects/" + soundSource.Key + "/" + soundType.Key + " " + (i + 1)));
                    }
                }
            }

            // Load songs
            songs.Add("Level", content.Load<Song>("Songs/Level"));
            songs.Add("Game Over", content.Load<Song>("Songs/Game Over"));
            songs.Add("High Score", content.Load<Song>("Songs/High Score"));
            songs.Add("Main Menu", content.Load<Song>("Songs/Main Menu"));
        }

        /// <summary>
        /// Plays the achievement sound
        /// </summary>
        /// <returns>Does the sound effects exist</returns>
        public bool PlayAchievement()
        {
            try
            {
                soundEffects["Other"]["Achievement"][0].Play(1, 0f, 0f);

                return true;
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine("Achievement" + " is not a sound effect of " + "Other");
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Plays the sound effect corresponding to the event
        /// </summary>
        /// <param name="soundSource">What is emitting the sound ex: Streaker</param>
        /// <param name="soundType">Type of sound emmited ex: SuperFlash</param>
        /// <returns>Does the sound effects exist</returns>
        public bool PlaySound(string soundSource, string soundType, Vector2 streakerPosition, Vector2 otherPosition)
        {
            if ((streakerPosition - otherPosition).Length() > distanceThreshold)
            {
                return false;
            }

            if (!soundSource.Equals("Common") && !soundSource.Equals("Streaker") && Game1.random.NextDouble() < 0.8f)
            {
                return false;
            }

            try
            {
                int index = Game1.random.Next(0, soundEffects[soundSource][soundType].Count);

                if (soundSource.Equals("Streaker"))
                {
                    soundEffects[soundSource][soundType][index].Play(1, 0f, 0f);
                }
                else
                {
                    float pitch = (float)(0.4 * Game1.random.NextDouble() - 0.2);
                    soundEffects[soundSource][soundType][index].Play(1, pitch, 0f);
                }
                
                return true;
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(soundType + " is not a sound effect of " + soundSource);
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Plays the sound effect corresponding to the event
        /// </summary>
        /// <param name="soundSource">What is emitting the sound ex: Streaker</param>
        /// <param name="soundType">Type of sound emmited ex: SuperFlash</param>
        /// <returns>Does the sound effects exist</returns>
        public bool PlaySound(string soundSource, string soundType)
        {
            if (!soundSource.Equals("Common") && !soundSource.Equals("Streaker") && Game1.random.NextDouble() < 0.8f)
            {
                return false;
            }

            try
            {
                int index = Game1.random.Next(0, soundEffects[soundSource][soundType].Count);

                if (soundSource.Equals("Streaker"))
                {
                    soundEffects[soundSource][soundType][index].Play(1, 0f, 0f);
                }
                else
                {
                    float pitch = (float)(0.4 * Game1.random.NextDouble() - 0.2);
                    soundEffects[soundSource][soundType][index].Play(1, pitch, 0f);
                }

                return true;
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(soundType + " is not a sound effect of " + soundSource);
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Plays a song
        /// </summary>
        /// <param name="key">Name of the song</param>
        /// <returns>Does the song exist</returns>
        public bool PlaySong(String key)
        {
            if (instance == null)
            {
                return false;
            }

            try
            {
                // Stop the previous song
                MediaPlayer.Stop();

                // Play the new song
                MediaPlayer.Play(songs[key]);

                // Loop the currently playing song
                if (key == "Main Menu" || key == "Level")
                {
                    MediaPlayer.IsRepeating = true;
                }
                else
                {
                    MediaPlayer.IsRepeating = false;
                }

                return true;
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(key + " is not a sound effect.");
                Console.WriteLine(e.Message);
                return false;
            }
        }

        #endregion
    }
}
