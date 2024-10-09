namespace ConsoleGame;

public class Player
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int MagicPower { get; set; }
    public int AttackPower { get; set; }
    public List<IItem> Inventory { get; set; }
    public Dictionary<string, IItem> EquippedItems { get; set; }

    public Player()
    {
        MaxHealth = 100;
        Health = MaxHealth;
        Level = 1;
        Experience = 0;
        AttackPower = 10;
        MagicPower = 5;
        Inventory = new List<IItem>();
        EquippedItems = new Dictionary<string, IItem>();
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0;
        Console.WriteLine($"You received {damage} damage. Your health: {Health}");
    }

    public void EquipItem(IItem item)
    {
        if (EquippedItems.ContainsKey(item.Type))
        {
            UnequipItem(item.Type);
        }
        EquippedItems[item.Type] = item;
        item.ApplyEffects(this);
        Console.WriteLine($"Equipped item: {item.Name}");
    }

    public void UnequipItem(string itemType)
    {
        if (EquippedItems.TryGetValue(itemType, out IItem? item))
        {
            item.RemoveEffects(this);
            EquippedItems.Remove(itemType);
            Console.WriteLine($"Unequipped item: {item.Name}");
        }
    }

    public void AddExperience(int exp)
    {
        Experience += exp;
        Console.WriteLine($"You have gained {exp} experience points!");
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int expNeeded = CalculateExpNeeded();
        while (Experience >= expNeeded)
        {
            Level++;
            Experience -= expNeeded;
            MaxHealth += 20 + (Level * 5);
            Health = MaxHealth;
            AttackPower += 5 + (Level * 2);
            MagicPower += 3 + (Level * 1);
            Console.WriteLine($"Congratulations, you have just advanced your level to {Level}!");
            expNeeded = CalculateExpNeeded();
        }
    }

    public int CalculateExpNeeded()
    {
        return (int)(100 * Math.Pow(Level, 1.5));
    }

    public void ResetHp() // not sure if needed
    {
        Health = MaxHealth;
    }
}