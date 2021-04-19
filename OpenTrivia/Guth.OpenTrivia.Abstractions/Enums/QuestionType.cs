using System.Runtime.Serialization;

namespace Guth.OpenTrivia.Abstractions.Enums
{
    public enum QuestionType
    {
        [EnumMember(Value = null)]
        Any,
        [EnumMember(Value = "multiple")]
        MultipleChoice,
        [EnumMember(Value = "boolean")]
        TrueFalse
    }
}
