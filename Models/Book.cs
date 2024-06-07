using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoSimpleCrud.Models;

[BsonIgnoreExtraElements]
public class Book : IBaseModel
{
    public Book()
    {
        Id = Guid.NewGuid().ToString(); 
    }

    [BsonRepresentation(BsonType.String)] // Makes the Id human readable
    public string Id { get; set; } // Mongo driver recognize this field as the "_id"
    public string Name { get;  set; }
    public Author Author { get;  set; }
    public int LaunchYear { get; set; }
    public decimal ReleasePrice { get; set; }

    public void ChangeBookName(string name) => Name = name;
}

public class Author
{
    public string Name { get; set; }
    public Location Location { get; set; }
    public int BirthdayYear { get; set; }
}

public class Location
{
    public string Country { get; set; }
    public string State { get; set; }
}