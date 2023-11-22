using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoSimpleCrud.Models;

public class Book : IBaseModel
{
    public Book(string name, string author)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Author = author;
    }

    [BsonRepresentation(BsonType.String)] // Makes the Id human readable
    public string Id { get; private set; } // Mongo driver recognize this field as the "_id"
    public string Name { get; private set; }
    public string Author { get; private set; }

    public void ChangeBookName(string name) => Name = name;
}