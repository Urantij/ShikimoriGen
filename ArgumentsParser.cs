namespace ShikimoriGen;

public class Arguments(string search, string? folderPath, int limit)
{
    public string Search { get; } = search;
    public string? FolderPath { get; } = folderPath;
    public int Limit { get; } = limit;
}

public static class ArgumentsParser
{
    public static Arguments Parse(string[] args)
    {
        string? search = null;
        string? folderPath = null;
        int? limit = null;

        int counter = 0;
        while (counter < args.Length)
        {
            string arg = args[counter];

            if (arg.Equals("-l"))
            {
                limit = int.Parse(args[counter + 1]);
                counter++;
            }
            else if (arg.Equals("-f"))
            {
                folderPath = args[counter + 1];
                counter++;
            }
            else if (counter + 1 == args.Length)
            {
                search = arg;
            }
            else
            {
                throw new Exception($"непонятный аргумент {arg}");
            }

            counter++;
        }

        if (search == null)
        {
            throw new Exception("Что искать то?");
        }

        if (limit == null)
        {
            limit = 5;
        }

        return new Arguments(search, folderPath, limit.Value);
    }
}