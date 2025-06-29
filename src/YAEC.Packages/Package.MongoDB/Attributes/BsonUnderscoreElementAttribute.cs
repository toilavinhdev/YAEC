using System.Runtime.CompilerServices;
using MongoDB.Bson.Serialization.Attributes;
using YAEC.Shared.Extensions;

namespace Package.MongoDb.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class BsonUnderscoreElementAttribute([CallerMemberName] string elementName = null!)
    : BsonElementAttribute(elementName.ToUnderscoreCase());