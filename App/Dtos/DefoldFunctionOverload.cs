using System.Diagnostics.CodeAnalysis;

namespace App.Dtos
{
    public class DefoldFunctionOverload
    {
        public DefoldParameter[] Parameters { get; set; }
        public DefoldReturnValue[] ReturnValues { get; set; }

        public DefoldFunctionOverload()
        {
            Parameters = Array.Empty<DefoldParameter>();
            ReturnValues = Array.Empty<DefoldReturnValue>();
        }
    }

    class DefoldFunctionOverloadUniquenessEqualityComparer : IEqualityComparer<DefoldFunctionOverload>
    {
        public static readonly IEqualityComparer<DefoldFunctionOverload> Instance = new DefoldFunctionOverloadUniquenessEqualityComparer();

        public bool Equals(DefoldFunctionOverload? x, DefoldFunctionOverload? y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;

            var xParamsTypes = x.Parameters.SelectMany(x => x.Types).OrderBy(typeName => typeName);
            var yParamsTypes = y.Parameters.SelectMany(y => y.Types).OrderBy(typeName => typeName);

            var xReturnValues = x.ReturnValues.SelectMany(x => x.Types).OrderBy(typeName => typeName);
            var yReturnValues = y.ReturnValues.SelectMany(y => y.Types).OrderBy(typeName => typeName);
            return x.Parameters.Length == y.Parameters.Length &&
                xParamsTypes.SequenceEqual(yParamsTypes) &&
                xReturnValues.SequenceEqual(yReturnValues);
        }

        public int GetHashCode([DisallowNull] DefoldFunctionOverload obj)
        {
            var hashCode = HashCode.Combine(
                obj.Parameters.Length,
                obj.ReturnValues.Length);
            return hashCode;
        }
    }
}