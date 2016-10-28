using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Cat_and_Mouse___XNA
{
    class SaveLoadManager
    {
        #region Fields

        private static SaveLoadManager instance;
        private Game1 game;

        BinaryFormatter bformatter;

        #endregion

        #region Private Members

        private SaveLoadManager()
        {

        }

        private void SaveGame(object obj, EventArgs e)
        {
            BinaryFormatter bf = new BinaryFormatter();
            File.WriteAllText("GameData.bin", String.Empty);
            FileStream stream = new FileStream("GameData.bin", FileMode.Append, FileAccess.Write);

            // pass stream to necessary objects
            EntityManager.Instance.SaveData(stream);

            // serialize game
            bformatter.Serialize(stream, Game1.loader);

            stream.Close();
        }

        private void LoadGame(object obj, EventArgs e)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream("GameData.bin", FileMode.Open, FileAccess.Read);

            long position = 0;

            EntityManager.Instance.LoadData(ref stream, ref position);

            if (position < stream.Length)
            {
                stream.Seek(position, SeekOrigin.Begin);
                Game1.loader = (GameLoader)bf.Deserialize(stream);
                position = stream.Position;
            }

            stream.Close();
        }

        #endregion

        #region Public Members

        public static SaveLoadManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new SaveLoadManager();

                return instance;
            }
        }

        public void Initialize(Game1 game)
        {
            this.game = game;

            bformatter = new BinaryFormatter();

            InputManager.Instance.SavePressed += SaveGame;
            InputManager.Instance.LoadPressed += LoadGame;
        }

        #endregion

    }
}