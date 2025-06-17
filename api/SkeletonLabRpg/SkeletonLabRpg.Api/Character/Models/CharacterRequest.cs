namespace SkeletonLabRpg.Api.Character.Models;

public class CharacterRequest
{
    public string Class { get; set; } = string.Empty;
    public float Strength { get; set; }
    public float Dexterity { get; set; }
    public float Intelligence { get; set; }
    public List<string> Weapons { get; set; } = new();
    public List<string> Spells { get; set; } = new();
    public List<string> Feats { get; set; } = new();
}