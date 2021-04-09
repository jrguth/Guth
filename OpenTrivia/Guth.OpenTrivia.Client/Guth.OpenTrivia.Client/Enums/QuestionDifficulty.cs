using System.Runtime.Serialization;

namespace Guth.OpenTrivia.Client.Enums
{
    public enum QuestionDifficulty
    {
        [EnumMember(Value = "easy")]
        Easy,
        [EnumMember(Value = "medium")]
        Medium,
        [EnumMember(Value = "hard")]
        Hard
    }
}
