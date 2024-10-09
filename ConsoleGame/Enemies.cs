namespace ConsoleGame;

public interface IEnemy
{
    string Name { get; }
    int Health { get; }
    int Level { get;  }
    int AttackPower { get; }
    int MagicPower { get; }
    int ExperienceValue { get; }
    void Attack(Player player);
    void TakeDamage(int damage);
}

public abstract class BaseEnemy : IEnemy
{
    public string Name { get; protected set ; }
    public int Health { get; protected set; }
    public int AttackPower  { get; protected set; }
    public int MagicPower { get; protected set; }
    public int ExperienceValue { get; protected set; }
    public abstract void Attack(Player player);
    public virtual void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health < 0) Health = 0;
        Console.WriteLine($"{Name} received {damage} damage. Left health: {Health}");
    }
    
    public int Level { get; protected set; }

    protected void ScaleStats()
    {   
        if (Level == 1) return;
        // old formula Health = (int)Math.Round(Health * ((Level * 0.15) + 1));
        Health = (int)Math.Round(Health * (float)Math.Pow(1 + 0.15f, Level - 1));
        AttackPower = (int)Math.Round(AttackPower * (float)Math.Pow(1 + 0.10f, Level - 1));
        //ExperienceValue += (Level - 1) * 10;
    }
}

public class Wolf : BaseEnemy
{
    public Wolf()
    {
        Name = "Wolf";
        Health = 50;
        AttackPower = 10;
        ExperienceValue = Random.Shared.Next(10, 31);
        Level = Random.Shared.Next(1, 11); // Max 10 lvl
        ScaleStats();
    }

    public override void Attack(Player player)
    {
        Console.WriteLine($"{Name} is biting player!");
        player.TakeDamage(AttackPower);
    }
}

public class Bandit : BaseEnemy
{
    public Bandit()
    {
        Name = "Bandit";
        Health = 100;
        AttackPower = 20;
        ExperienceValue = Random.Shared.Next(20, 51);
        Level = Random.Shared.Next(1, 16); // Max 15 lvl
        ScaleStats();
    }

    public override void Attack(Player player)
    {
        Console.WriteLine($"{Name} is attacking with sword!");
        player.TakeDamage(AttackPower);
    }
}

public class Alpha : BaseEnemy
{
    public Alpha()
    {
        Name = "Alpha";
        Health = 200;
        AttackPower = 30;
        MagicPower = 20;
        ExperienceValue = Random.Shared.Next(50, 101);
        Level = Random.Shared.Next(1, 21); // Max 20 lvl
        ScaleStats();
    }

    public override void Attack(Player player)
    {
        Console.WriteLine($"{Name} is attacking player with thunder!");
        player.TakeDamage(AttackPower + MagicPower);
    }
}

public static class EnemyFactory
{
    public static IEnemy CreateEnemy(string enemyType)
    {
        switch (enemyType.ToLower())
        {
            case "wolf":
                return new Wolf();
            case "bandit":
                return new Bandit();
            case "alpha":
                return new Alpha();
            default:
                throw new ArgumentException("Unknown enemy type", nameof(enemyType));
        }
    }
}