namespace SkeletonLabRpg.Common.Configuration;

public class StorageConfiguration
{
    public const string Name = "Storage";
    
    public Blob Blob { get; set; }
    
    public Queue Queue { get; set; }
}

public class Blob
{
    public string Endpoint { get; set; }
    
    public CharacterAttributeModels CharacterAttributeModels { get; set; }
}

public class CharacterAttributeModels
{
    public string Name { get; set; }
}

public class Queue
{
    public string Endpoint { get; set; }
    
    public CharacterAttributeQueue CharacterAttributeQueue { get; set; }
}

public class CharacterAttributeQueue
{
    public string Name { get; set; }
}