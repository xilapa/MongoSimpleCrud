using MongoDB.Driver;
using MongoSimpleCrud.Models;

namespace MongoSimpleCrud.Data;

public static class MongoCollectionExtensions
{
    public static Task<List<T>> GetAllAsync<T>(this IMongoCollection<T> collection) =>
        collection.Find(_ => true).ToListAsync();
    
    public static Task<T> GetAsync<T>(this IMongoCollection<T> collection, string id) where T : IBaseModel =>
        collection.Find(i => i.Id == id).SingleAsync();
    
    public static Task InsertAsync<T>(this IMongoCollection<T> collection, T item) =>
        collection.InsertOneAsync(item);

    public static Task UpdateAsync<T>(this IMongoCollection<T> collection, T item) where T : IBaseModel =>
        collection.ReplaceOneAsync(i => i.Id == item.Id, item);

    public static Task DeleteAsync<T>(this IMongoCollection<T> collection, string id) where T : IBaseModel =>
        collection.DeleteOneAsync(i => i.Id == id);
}