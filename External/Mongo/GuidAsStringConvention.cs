using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.Reflection;

namespace Vulpes.Zinc.External.Mongo;
internal class GuidAsStringConvention : ConventionBase, IMemberMapConvention
{
    public void Apply(BsonMemberMap memberMap)
    {
        var memberTypeInfo = memberMap.MemberType.GetTypeInfo();
        if (memberTypeInfo == typeof(Guid))
        {
            var serializer = memberMap.GetSerializer();
            if (serializer is IRepresentationConfigurable representationConfigurableSerializer)
            {
                var representation = BsonType.String;
                var reconfiguredSerializer = representationConfigurableSerializer.WithRepresentation(representation);
                _ = memberMap.SetSerializer(reconfiguredSerializer);
            }
        }
    }
}