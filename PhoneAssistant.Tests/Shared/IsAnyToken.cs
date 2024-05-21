using Moq;

namespace PhoneAssistant.Tests.Shared;

[TypeMatcher]
public sealed class IsAnyToken : ITypeMatcher, IEquatable<IsAnyToken>
{
    public bool Matches(Type typeArgument) => true;
    public bool Equals(IsAnyToken? other) => throw new NotImplementedException();
}

