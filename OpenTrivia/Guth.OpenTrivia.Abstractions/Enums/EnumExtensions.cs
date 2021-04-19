using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Guth.OpenTrivia.Abstractions.Enums
{
    public static class EnumExtensions
    {
        public static string GetEnumMemberAttributeValue(this Enum e)
        {
            var memInfo = e.GetType().GetMember(e.ToString());
            var attr = memInfo[0].GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault();
            return attr is null
                ? null
                : attr.Value;
        }
    }
}
