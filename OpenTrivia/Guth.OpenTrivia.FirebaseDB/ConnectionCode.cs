namespace Guth.OpenTrivia.FirebaseDB
{
    public sealed class ConnectionCode
    {
        public string Code { get; private set; }
        public bool IsActive { get; private set; }
        public string GameId { get; set; }

        internal ConnectionCode(string code)
        {
            Code = code;
            IsActive = true;
        }
    }
}
