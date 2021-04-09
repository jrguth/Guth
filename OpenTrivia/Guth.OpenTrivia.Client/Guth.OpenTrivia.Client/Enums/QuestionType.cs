using System.Runtime.Serialization;

namespace Guth.OpenTrivia.Client.Enums
{
    public enum QuestionType
    {
        [EnumMember(Value = "multiple")]
        MultipleChoice,
        [EnumMember(Value = "boolean")]
        TrueFalse
    }
}
