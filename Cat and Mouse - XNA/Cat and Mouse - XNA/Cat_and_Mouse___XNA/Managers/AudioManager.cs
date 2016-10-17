using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Cat_and_Mouse___XNA
{
    public enum SoundKeys { SadCat, HappyCat, SadMouse, HappyMouse }

    class AudioManager
    {

        #region Fields

        protected static AudioManager instance;
        Song song;
        Song fanfare;
        Song gameover;
        Dictionary<SoundKeys, SoundEffect> soundLibrary;

        #endregion

        #region Constructors

        /// <summary>
        /// basic constructor. put any operations needed before initialization here
        /// </summary>
        private AudioManager()
        {
            soundLibrary = new Dictionary<SoundKeys, SoundEffect>();
        }

        #endregion

        #region Public Members

        /// <summary>
        /// singleton instance accessor property 
        /// </summary>
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new AudioManager();

                return instance;
            }
        }

        /// <summary>
        /// initializes the audio manager singleton. loads content and runs first-time operations
        /// </summary>
        /// <param name="Content"></param>
        public void Initialize(ContentManager Content)
        {
            // loads the background music
            song = Content.Load<Song>("Music/benny");
            fanfare = Content.Load<Song>("Music/fanfare");
            gameover = Content.Load<Song>("Music/gameover");

            // load all sound effects into given dictionary
            soundLibrary.Add(SoundKeys.HappyCat, Content.Load<SoundEffect>("Music/happycat"));
            soundLibrary.Add(SoundKeys.SadCat, Content.Load<SoundEffect>("Music/sadcat"));
            soundLibrary.Add(SoundKeys.HappyMouse, Content.Load<SoundEffect>("Music/happymouse"));
            soundLibrary.Add(SoundKeys.SadMouse, Content.Load<SoundEffect>("Music/sadmouse"));

            // start the background music
            //MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// plays a sound from a given key
        /// </summary>
        /// <param name="key"></param>
        public void PlaySound(SoundKeys key)
        {
            //soundLibrary[key].Play();
        }

        /// <summary>
        ///  plays a sound from a given key at a given volume
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vol"></param>
        public void PlaySound(SoundKeys key, float vol)
        {
            soundLibrary[key].Play(vol, 1f, 1f);
        }

        /// <summary>
        /// starts the background music
        /// </summary>
        public void PlayBackground()
        {
            //MediaPlayer.Play(song);
        }

        /// <summary>
        /// plays a victory fanfare
        /// </summary>
        public void PlayFanfare()
        {
            //MediaPlayer.Play(fanfare);
        }

        /// <summary>
        /// plays a game over tune
        /// </summary>
        public void PlayGameover()
        {
            //MediaPlayer.Play(gameover);
        }

        /// <summary>
        /// overloaded method that plays a given song
        /// </summary>
        /// <param name="music"> the song to be played </param>
        public void PlayBackground(Song music)
        {
            MediaPlayer.Play(music);
        }

        /// <summary>
        /// pauses the background music
        /// </summary>
        public void PauseMusic()
        {
            MediaPlayer.Pause();
        }

        #endregion
    }
}
