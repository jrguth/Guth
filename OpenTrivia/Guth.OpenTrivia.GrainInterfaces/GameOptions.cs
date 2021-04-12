namespace Guth.OpenTrivia.GrainInterfaces
{
    public class GameOptions
    {
        public int SecondsPerRound { get; }
        public GameOptions(int secondsPerRound = 15)
        {
            SecondsPerRound = secondsPerRound;
        }
    }
}
