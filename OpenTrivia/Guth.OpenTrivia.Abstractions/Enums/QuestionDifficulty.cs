using System.Runtime.Serialization;

namespace Guth.OpenTrivia.Abstractions.Enums
{
    public enum QuestionDifficulty
    {
        [EnumMember(Value = null)]
        Any,
        [EnumMember(Value = "easy")]
        Easy,
        [EnumMember(Value = "medium")]
        Medium,
        [EnumMember(Value = "hard")]
        Hard
    }
}
