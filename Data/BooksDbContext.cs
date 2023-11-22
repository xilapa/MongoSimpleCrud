using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoSimpleCrud.Models;

namespace MongoSimpleCrud.Data;

public class BooksDbContext
{
    private readonly IMongoDatabase _mongoDb;
    private readonly BooksDbSettings _booksDbSettings;
    private readonly Lazy<IMongoCollection<Book>> _lazyBooks;

    public BooksDbContext(IOptions<BooksDbSettings> booksDbSettings)
    {
        _booksDbSettings = booksDbSettings.Value;
        var mongoClient = new MongoClient(_booksDbSettings.ConnectionString);
        _mongoDb = mongoClient.GetDatabase(_booksDbSettings.DatabaseName);

        // collection lazy initialization
        _lazyBooks = new Lazy<IMongoCollection<Book>>(() =>
            _mongoDb.GetCollection<Book>(_booksDbSettings.BooksCollectionName));
    }

    public IMongoCollection<Book> Books() => _lazyBooks.Value;
}

