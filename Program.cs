using System.Text.Json;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using GraphQL.Query.Builder;
using ShikimoriGen.Models;

namespace ShikimoriGen;

class Program
{
    static async Task Main(string[] appArgs)
    {
        Console.WriteLine("Hello, World!");

        Arguments args = ArgumentsParser.Parse(appArgs);

        // было бы здорово из модели целиком генерировать запрос, но рефлексия это далёкое будущее.
        // да и aot кабутабы поприкольнее... ну ладно.
        string query = new Query<Anime>("animes")
            .AddArgument("search", args.Search)
            .AddArgument("limit", args.Limit)
            .AddArgument("order", Order.aired_on)
            .AddField(anime => anime.name)
            .AddField(anime => anime.russian)
            .AddField(anime => anime.english)
            .AddField(anime => anime.japanese)
            .AddField(anime => anime.airedOn,
                b => b.AddField(date => date!.year))
            .AddField(anime => anime.synonyms)
            .AddField<Studio>(anime => anime.studios,
                b => b.AddField(studio => studio.name))
            .Build();

        GraphQLHttpClient client = new(new GraphQLHttpClientOptions()
        {
            EndPoint = new Uri("https://shikimori.one/api/graphql"),
        }, new SystemTextJsonSerializer(new JsonSerializerOptions()
        {
            TypeInfoResolver = new MyJsonContext()
        }));

        GraphQLResponse<Response> result = await client.SendQueryAsync<Response>(new GraphQLRequest('{' + query + '}'));

        Anime[]? animes = result.Data?.animes;

        if (animes == null)
        {
            Console.WriteLine("не удалось");
            return;
        }

        if (animes.Length == 0)
        {
            Console.WriteLine("не нашёл");
            return;
        }

        Anime targetAnime;
        if (animes.Length == 1)
        {
            targetAnime = animes[0];

            Console.WriteLine($"{targetAnime.name} ({targetAnime.russian}) {targetAnime.airedOn?.year}");
            Console.ReadLine();
        }
        else
        {
            Console.WriteLine("выбирай");
            Console.WriteLine(string.Join('\n',
                animes.Select((anime, i) => $"[{i + 1}] {anime.name} ({anime.russian}) {anime.airedOn?.year}")));

            int index = int.Parse(Console.ReadLine()!) - 1;

            targetAnime = animes[index];
        }

        string title = targetAnime.name.Replace(":", "");

        string[] aliases = new string[]
            {
                targetAnime.english,
                targetAnime.russian,
                targetAnime.japanese,
            }
            .Concat(targetAnime.synonyms)
            .Where(s => s != null)
            .Where(s => !s.Equals(title, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToArray();

        string al = string.Join("\n", aliases.Select(a => $"  - {a}"));

        string path = Path.Combine(args.FolderPath ?? "./", $"{title}.md");

        await File.WriteAllTextAsync(path, $"""
                                            ---
                                            aliases:
                                            {al}
                                            ---

                                            """);
    }
}