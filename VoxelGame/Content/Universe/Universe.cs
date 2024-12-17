using Content.Universe;

namespace VoxelGame.Content.Universe
{
    public class Universe
    {
        public List<World> Worlds { get; init; }
        public string Name { get; init; }
        public Universe(string name, int seed)
        {
            Name = name;
            Worlds = new List<World>();
        }

        public void Update(double time)
        {

        }
    }
}
