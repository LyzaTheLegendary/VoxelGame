using System.Collections.Concurrent;
using VoxelGame.Resources.Components;

namespace Content
{
    static public class GameContent
    {
        static private ConcurrentDictionary<string, Shape> shapes = new ConcurrentDictionary<string, Shape>();
        static private ConcurrentDictionary<string, string> translations = new();

        static public void RegisterTranslation(string key, string value) 
            => translations[key] = value;
        static public string GetTranslation(string key)
            => translations.TryGetValue(key, out var value) ? value : $"Missing string key {key}";
        
        static public void RegisterShape(string name, Shape shape)
        {
            if (!shapes.TryAdd(name, shape))
                throw new Exception($"{name} shape already exists!");
        }
        static public Shape GetShape(string name)
            => shapes.TryGetValue(name, out var shape) ? shape : throw new Exception($"{name} shape does not exist!");

    }
}
