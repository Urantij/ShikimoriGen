using System.Text.Json.Serialization;
using ShikimoriGen.Models;

namespace ShikimoriGen;

[JsonSerializable(typeof(Response))]
[JsonSerializable(typeof(Anime))]
[JsonSerializable(typeof(Studio))]
[JsonSerializable(typeof(FunnyDate))]
[JsonSerializable(typeof(GraphQL.GraphQLRequest))]
[JsonSerializable(typeof(GraphQL.GraphQLResponse<Response>))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(Order))]
public partial class MyJsonContext : JsonSerializerContext
{
    
}