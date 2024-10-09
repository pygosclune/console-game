using System.Drawing;
using ConsoleGame;
using System.Text.Json;
using Pastel;

internal class Program
{
    private static Player _player;
    private static List<ILocation> _availableLocations;

    private enum FightResult
    {
        Victory,
        Defeat,
        Escape
    }

    private static void Main()
    {
        Console.WriteLine("Welcome in console game!".Pastel(Color.White));
        Console.WriteLine("1. New game");
        Console.WriteLine("2. Load game");
        string ?choice = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(choice)) return;
        switch (choice)
        {
            case "1":
                _player = new Player();
                break;
            case "2":
            {
                _player = LoadGame();
                if (_player == null)
                {
                    Console.WriteLine("Saved game not found. Starting a new game...");
                    _player = new Player();
                }

                break;
            }
        }

        while (true)
        {
            _availableLocations = LocationManager.GetAvailableLocations(_player.Level);
            DisplayMenu();
            string ?userInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(userInput))
            {
                switch (userInput.ToLower())
                {
                    case "q":
                        SaveGame(_player);
                        break;
                    case "i" when _player.Inventory.Count > 0:
                    {
                        foreach (var item in _player.Inventory)
                        {
                            Console.Write(item + " ");
                        }

                        break;
                    }
                    case "i":
                        Console.WriteLine("Your inventory is empty.");
                        break;
                    case "s":
                        Console.WriteLine($"Level {_player.Level} ({_player.Experience}/{_player.CalculateExpNeeded()})");
                        Console.WriteLine($"Max HP: {_player.MaxHealth}");
                        Console.WriteLine($"Attack Power: {_player.AttackPower} Magic Power: {_player.MagicPower}");
                        break;
                    default:
                    {
                        if (int.TryParse(userInput, out int locationIndex) && locationIndex > 0 &&
                            locationIndex <= _availableLocations.Count)
                        {
                            ExploreLocation(_availableLocations[locationIndex - 1]);
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice. Try again.");
                        }

                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Enter proper input.");
            }
        }
    }

    private static void DisplayMenu()
    {
        Console.WriteLine("\nAvailable locations:");
        for (int i = 0; i < _availableLocations.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {_availableLocations[i].Name} (Required level: {_availableLocations[i].RequiredLevel})");
        }
        Console.WriteLine("Q. Save game");
        Console.WriteLine("I. Inventory");
        Console.WriteLine("S. Stats");
        Console.Write("Select a location or action: ");
    }

    private static void ExploreLocation(ILocation location)
    {
        Console.WriteLine($"\nExploring {location.Name}");
        for (int i = 1; i <= 10; i++)
        {
            IEnemy enemy = EnemyFactory.CreateEnemy(location.Enemies[Random.Shared.Next(location.Enemies.Count)].GetType().Name);
            Console.WriteLine($"Round {i}/10\nYou encounter {enemy.Name} (Level {enemy.Level}, HP {enemy.Health}, Attack Power {enemy.AttackPower})");
            FightResult result = Fight(enemy);
            switch (result)
            {
                case FightResult.Victory:
                    Console.WriteLine("Congratulations! You have defeated your opponent!");
                    _player.AddExperience(enemy.ExperienceValue);
                    DropLoot(location);
                    break;
                case FightResult.Defeat:
                    Console.WriteLine("You have lost the battle. You return to the main menu.");
                    _player.ResetHp();
                    return;
                case FightResult.Escape:
                    Console.WriteLine("You have successfully escaped. You return to the main menu.");
                    _player.ResetHp();
                    return;
            }
        }
        Console.WriteLine("You have completed the exploration of the location!");
    }

    private static FightResult Fight(IEnemy enemy)
    {
        Console.WriteLine($"You are fighting the {enemy.Name}!");
        while (_player.Health > 0 && enemy.Health > 0)
        {
            Console.WriteLine($"\nYour HP: {_player.Health}, Opponent's HP: {enemy.Health}");
            Console.WriteLine("1. Attack");
            Console.WriteLine("2. Escape");
            Console.Write("Select an action: ");
            string? choice = Console.ReadLine();

            if (choice == "1")
            {
                // Player's turn
                int calculatedAttackPower = (int) Math.Round(_player.AttackPower * (Random.Shared.NextDouble() + 1));
                enemy.TakeDamage(calculatedAttackPower);
                if (enemy.Health <= 0) return FightResult.Victory;

                // Opponent's turn
                enemy.Attack(_player);
                if (_player.Health <= 0) return FightResult.Defeat;
            }
            else if (choice == "2")
            {
                double escapeChance = CalculateEscapeChance(_player, enemy);
                if (Random.Shared.NextDouble() < escapeChance)
                {
                    return FightResult.Escape;
                }
                Console.WriteLine("You failed to escape. The opponent is attacking!");
                enemy.Attack(_player);
                if (_player.Health <= 0) return FightResult.Defeat;
                
            }
            else
            {
                Console.WriteLine("Incorrect choice. You lose a turn!");
            }
        }
        return _player.Health > 0 ? FightResult.Victory : FightResult.Defeat;
    }

    private static double CalculateEscapeChance(Player playerObj, IEnemy enemy)
    {
        double baseChance = 0.3; // 30% base chance
        double levelDifference = playerObj.Level - enemy.Level;
        double additionalChance = Math.Max(0, levelDifference * 0.05); // 5% for every level of diff
        double escapeChance = Math.Min(0.9, baseChance + additionalChance); // Max 90%
        Console.WriteLine($"Chance to escape: {escapeChance:P}");
        return escapeChance;
    }

    private static void DropLoot(ILocation location)
    {
        if (Random.Shared.Next(100) >= 30) return; // 30% chance for loot
        IItem loot = location.PossibleLoot[Random.Shared.Next(location.PossibleLoot.Count)];
        _player.Inventory.Add(loot);
        Console.WriteLine($"Item obtained: {loot.Name}!");
    }

    private static void SaveGame(Player playerObj)
    {
        string jsonString = JsonSerializer.Serialize(playerObj);
        File.WriteAllText("savegame.json", jsonString);
        Console.WriteLine("The game has been saved.");
    }

    private static Player? LoadGame()
    {
        if (!File.Exists("savegame.json")) return null;
        string jsonString = File.ReadAllText("savegame.json");
        return JsonSerializer.Deserialize<Player>(jsonString);
    }
}
