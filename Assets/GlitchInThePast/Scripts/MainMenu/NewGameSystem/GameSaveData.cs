using UnityEngine;

namespace GameData
{
    /// <summary>
    /// Contains data to save for players when they initiate a new game.
    /// Contains data to load for when players wish to continue from their last progress point in a game.
    /// Potentially supports automatic saving to avoid progress loss.
    /// </summary>
   
    [System.Serializable]
    public class GameSaveData
    {
        [Tooltip("Tracks which chapter players are currently on.")]
        public int CurrentChapter = 1;
        [Tooltip("Tracks the current level players are on.")]
        public int CurrentLevel = 1;

        // Used to compare saved progress to check for any need to save or if players are upto date.
        // IMO Useful for automatic saving!!!
        [Tooltip("Stores which chapter players were last saved on.")]
        public int LastSavedChapter = 1;
        [Tooltip("Stores which level players were last saved on.")]
        public int LastSavedLevel = 1;

        [Tooltip("Checks and stores what kind of input device p1 is using // i.e. Keyboard or Controller")]
        public string Player1Input = "";
        [Tooltip("and what kind of input device p2 is using ... ")]
        public string Player2Input = "";

        [Tooltip("Checks and stores which character did player 1 choose")]
        public string Player1Character = "";
        [Tooltip("and which character did player 2 choose.")]
        public string Player2Character = "";
    }
}