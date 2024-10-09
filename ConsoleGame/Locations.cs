namespace ConsoleGame;

public interface ILocation
{
    string Name { get; }
    int RequiredLevel { get; }
    List<IEnemy> Enemies { get; }
    List<IItem> PossibleLoot { get; }
}

public class Location(string name, int requiredLevel) : ILocation
{
    public string Name { get; protected set; } = name;
    public int RequiredLevel { get; protected set; } = requiredLevel;
    public List<IEnemy> Enemies { get; protected set; } = [];
    public List<IItem> PossibleLoot { get; protected set; } = [];

    protected void AddEnemy(IEnemy enemy)
    {
        Enemies.Add(enemy);
    }

    protected void AddLoot(IItem item)
    {
        PossibleLoot.Add(item);
    }
}

public class Ermanda : Location
{
    public Ermanda() : base("Ermanda", 1)
    {
        AddEnemy(new Wolf());
        AddLoot(new Weapon("Beginner's sword", 50, 15));
        AddLoot(new Armor("Leather armor", 75, 20));
    }
}

public class SalalTown : Location
{
    public SalalTown() : base("Salal Town", 3)
    {
        AddEnemy(new Bandit());
        AddLoot(new Weapon("Steel sword", 100, 25));
        AddLoot(new Armor("Chainmail", 150, 40));
    }
}

public class HazardCity : Location
{
    public HazardCity() : base("Hazard City", 5)
    {
        AddEnemy(new Alpha());
        AddLoot(new Weapon("Magic sword", 200, 35));
        AddLoot(new Armor("Plate armor", 250, 60));
    }
}


public static class LocationManager
{
    private static readonly List<ILocation> Locations =
    [
        new Ermanda(),
        new SalalTown(),
        new HazardCity()
    ];

    public static List<ILocation> GetAvailableLocations(int playerLevel)
    {
        return Locations.Where(l => l.RequiredLevel <= playerLevel).ToList();
    }
}