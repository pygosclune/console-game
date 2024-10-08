namespace ConsoleGame;

public interface IItem
{
    string Name { get; }
    string Type { get; }
    int Value { get; }
    void ApplyEffects(Player player);
    void RemoveEffects(Player player);
}

public abstract class BaseItem : IItem
{
    public string Name { get; protected set; }
    public string Type { get; protected set; }
    public int Value { get; protected set; }

    public abstract void ApplyEffects(Player player);
    public abstract void RemoveEffects(Player player);
}

public class Weapon : BaseItem
{
    public int AttackBonus { get; private set; }

    public Weapon(string name, int value, int attackBonus)
    {
        Name = name;
        Type = "Weapon";
        Value = value;
        AttackBonus = attackBonus;
    }

    public override void ApplyEffects(Player player)
    {
        player.AttackPower += AttackBonus;
    }

    public override void RemoveEffects(Player player)
    {
        player.AttackPower -= AttackBonus;
    }
}

public class Armor : BaseItem
{
    public int HealthBonus { get; private set; }

    public Armor(string name, int value, int healthBonus)
    {
        Name = name;
        Type = "Armor";
        Value = value;
        HealthBonus = healthBonus;
    }

    public override void ApplyEffects(Player player)
    {
        player.MaxHealth += HealthBonus;
        player.Health += HealthBonus;
    }

    public override void RemoveEffects(Player player)
    {
        player.MaxHealth -= HealthBonus;
        player.Health = Math.Min(player.Health, player.MaxHealth);
    }
}
