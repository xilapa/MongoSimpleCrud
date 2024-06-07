using MongoDB.Bson;
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

    public static Task ReplaceAsync<T>(this IMongoCollection<T> collection, T item) where T : IBaseModel =>
        collection.ReplaceOneAsync(i => i.Id == item.Id, item);

    public static Task DeleteAsync<T>(this IMongoCollection<T> collection, string id) where T : IBaseModel =>
        collection.DeleteOneAsync(i => i.Id == id);

    public static async  Task UpdateAsync<T>(this IMongoCollection<T> collection, T oldObj, T newObj) where T: IBaseModel
    {
        var updateBuilder = Builders<T>.Update;
        var updates = GenerateUpdates(updateBuilder, oldObj, newObj);

        var filter = Builders<T>.Filter.Eq(b => b.Id, oldObj.Id);
        
        await collection.UpdateOneAsync(filter, updateBuilder.Combine(updates));
    }

    private const string IdName = "id";
    private const string StringName = "string";

    private static List<UpdateDefinition<T>> GenerateUpdates<T>(UpdateDefinitionBuilder<T> updateBuilder, T oldObj, T newObj) where T: IBaseModel
    {
        var updates = new List<UpdateDefinition<T>>();

        foreach (var propertyInfo in newObj.GetType().GetProperties())
        {
            var oldValue = propertyInfo.GetValue(oldObj);
            var newValue = propertyInfo.GetValue(newObj);

            if (propertyInfo.Name.Contains(IdName, StringComparison.OrdinalIgnoreCase))
                continue;
            
            if (newValue == default)
            {
                updates.Add(updateBuilder.Set(propertyInfo.Name, BsonNull.Value));
                continue;
            }

            if (!propertyInfo.PropertyType.IsClass || propertyInfo.PropertyType.Name.Contains(StringName, StringComparison.OrdinalIgnoreCase))
            {
                if (newValue != oldValue)
                    updates.Add(updateBuilder.Set(propertyInfo.Name, newValue));
            }                
            else
            {
                GenerateUpdatesForInnerProperties(propertyInfo.Name, updateBuilder, updates, oldValue!, newValue!);
            }
        }
        
        return updates; 
    }

    private static void GenerateUpdatesForInnerProperties<T>(string innerPropertyName, UpdateDefinitionBuilder<T> updateBuilder,
        List<UpdateDefinition<T>> updates, object oldObj, object newObj)
    {
        foreach (var propertyInfo in newObj.GetType().GetProperties())
        {
            var propName = $"{innerPropertyName}.{propertyInfo.Name}";
            var oldValue = propertyInfo.GetValue(oldObj);
            var newValue = propertyInfo.GetValue(newObj);

            if (newValue == default)
            {
                updates.Add(updateBuilder.Set(propertyInfo.Name, BsonNull.Value));
                continue;
            }

            if (!propertyInfo.PropertyType.IsClass || propertyInfo.PropertyType.Name.Contains(StringName, StringComparison.OrdinalIgnoreCase))
            {
                if (newValue != oldValue)
                    updates.Add(updateBuilder.Set(propName, newValue));
            }                
            else
            {
                GenerateUpdatesForInnerProperties(propName, updateBuilder, updates, oldValue!, newValue);
            }
        }
    }
}