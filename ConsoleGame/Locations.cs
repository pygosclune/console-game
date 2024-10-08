namespace ConsoleGame;

public interface ILocation
{
    string Name { get; }
    int RequiredLevel { get; }
    List<IEnemy> Enemies { get; }
    List<IItem> PossibleLoot { get; }
}

public class Location : ILocation
{
    public string Name { get; protected set; }
    public int RequiredLevel { get; protected set; }
    public List<IEnemy> Enemies { get; protected set; }
    public List<IItem> PossibleLoot { get; protected set; }

    public Location(string name, int requiredLevel)
    {
        Name = name;
        RequiredLevel = requiredLevel;
        Enemies = new List<IEnemy>();
        PossibleLoot = new List<IItem>();
    }

    public void AddEnemy(IEnemy enemy)
    {
        Enemies.Add(enemy);
    }

    public void AddLoot(IItem item)
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
    private static List<ILocation> _locations =
    [
        new Ermanda(),
        new SalalTown(),
        new HazardCity()
    ];

    public static List<ILocation> GetAvailableLocations(int playerLevel)
    {
        return _locations.Where(l => l.RequiredLevel <= playerLevel).ToList();
    }
}