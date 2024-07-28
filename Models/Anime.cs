namespace ShikimoriGen.Models;

public class Anime
{
    public string name { get; set; }
    public string russian { get; set; }
    public string english { get; set; }
    public string japanese { get; set; }
    public FunnyDate? airedOn { get; set; }
    public string[] synonyms { get; set; }
    public Studio[] studios { get; set; }
}