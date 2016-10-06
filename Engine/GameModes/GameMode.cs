using CustomEngine.Worlds.Actors;

namespace CustomEngine.GameModes
{
    public class GameMode
    {
        public Pawn PlayableCharacter { get { return _playableCharacter; } set { _playableCharacter = value; } }
        public Pawn _playableCharacter;
    }
}
