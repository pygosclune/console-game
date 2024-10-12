using System.Drawing;
using ConsoleGame;
using System.Text.Json;
using Pastel;

internal class Program
{
    private static Player _player;
    private static List<ILocation> _availableLocations;
    private static readonly Color NeutralMessageColor = Color.White;
    private static readonly Color GoodMessageColor = Color.Green;
    private static readonly Color WarningMessageColor = Color.Yellow;
    private static readonly Color BadMessageColor = Color.Red;
    const string ContinueMessage = "Press any key to continue...";
    
    private enum FightResult
    {
        Victory,
        Defeat,
        Escape
    }

    private static void Main()
    {
        
        Console.WriteLine("Welcome in console game!".Pastel(NeutralMessageColor));
        Console.WriteLine("1. New game".Pastel(NeutralMessageColor));
        Console.WriteLine("2. Load game".Pastel(NeutralMessageColor));
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
                    Console.WriteLine("Saved game not found. Starting a new game...".Pastel(NeutralMessageColor));
                    _player = new Player();
                }

                break;
            }
        }

        while (true)
        {
            Console.WriteLine(ContinueMessage.Pastel(NeutralMessageColor));
            Console.ReadKey();
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
                            Console.Write($"{item}\n".Pastel(NeutralMessageColor));
                        }
                        
                        break;
                    }
                    case "i":
                        Console.WriteLine("Your inventory is empty.".Pastel(NeutralMessageColor));
                        break;
                    case "s":
                        Console.WriteLine($"Level {_player.Level} ({_player.Experience}/{_player.CalculateExpNeeded()})".Pastel(NeutralMessageColor));
                        Console.WriteLine($"Max HP: {_player.MaxHealth}".Pastel(NeutralMessageColor));
                        Console.WriteLine($"Attack Power: {_player.AttackPower} Magic Power: {_player.MagicPower}".Pastel(NeutralMessageColor));
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
                            Console.WriteLine("Invalid choice. Try again.".Pastel(BadMessageColor));
                        }

                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Enter proper input.".Pastel(BadMessageColor));
            }
        }
    }

    private static void DisplayMenu()
    {
        const int menuWidth = 50;
        List<string> options = ["Q. Save game", "I. Inventory", "S. Stats"];
        
        Console.WriteLine("╔" + new string('═', menuWidth - 2) + "╗");
        
        Console.WriteLine("║ " + "Available locations:".Pastel(WarningMessageColor) 
                               + new string(' ', menuWidth - "Available locations:".Length - 3) + "║");

        for (int i = 0; i < _availableLocations.Count; i++) //used for because we need index
        {
            string locationText = $"{i + 1}. {_availableLocations[i].Name} " +
                                  $"(Required level: {_availableLocations[i].RequiredLevel})";
            Console.WriteLine("║ " + locationText.Pastel(Color.Cyan) + 
                              new string(' ', menuWidth - locationText.Length - 3) + "║");
        }

        foreach (string option in options) Console.WriteLine("║ " + option.Pastel(GoodMessageColor) + new string(' ', 
            menuWidth - option.Length - 3) + "║");
        
        Console.WriteLine("╚" + new string('═', menuWidth - 2) + "╝");
        Console.Write("Select a location or action: ".Pastel(NeutralMessageColor));
    }
    
    private static void ExploreLocation(ILocation location)
    {
        Console.WriteLine($"\nExploring {location.Name}".Pastel(WarningMessageColor));
        for (int i = 1; i <= 10; i++)
        {
            IEnemy enemy = EnemyFactory.CreateEnemy(location.Enemies[Random.Shared.Next(location.Enemies.Count)].GetType().Name);
            Console.WriteLine($"Round {i}/10".Pastel(Color.Magenta));
            Console.WriteLine($"You encounter {enemy.Name.Pastel(BadMessageColor)} (Level {enemy.Level}, HP {enemy.Health.ToString().Pastel(GoodMessageColor)}, Attack Power {enemy.AttackPower})");
            FightResult result = Fight(enemy);
            switch (result)
            {
                case FightResult.Victory:
                    Console.WriteLine("Congratulations! You have defeated your opponent!".Pastel(GoodMessageColor));
                    _player.AddExperience(enemy.ExperienceValue);
                    DropLoot(location);
                    break;
                case FightResult.Defeat:
                    Console.WriteLine("You have lost the battle. You return to the main menu.".Pastel(BadMessageColor));
                    _player.ResetHp();
                    return;
                case FightResult.Escape:
                    Console.WriteLine("You have successfully escaped. You return to the main menu.".Pastel(WarningMessageColor));
                    _player.ResetHp();
                    return;
            }
        }
        Console.WriteLine("You have completed the exploration of the location!".Pastel(GoodMessageColor));
    }

    private static FightResult Fight(IEnemy enemy)
    {
        Console.WriteLine($"You are fighting the {enemy.Name.Pastel(BadMessageColor)}!");
        while (_player.Health > 0 && enemy.Health > 0)
        {
            Console.WriteLine($"\nYour HP: {_player.Health.ToString().Pastel(GoodMessageColor)}, Opponent's HP: {enemy.Health.ToString().Pastel(BadMessageColor)}");
            Console.WriteLine("1. Attack".Pastel(Color.Cyan));
            Console.WriteLine("2. Escape".Pastel(WarningMessageColor));
            Console.Write("Select an action: ".Pastel(NeutralMessageColor));
            string? choice = Console.ReadLine();

            if (choice == "1")
            {
                // Player's turn
                int calculatedAttackPower = (int) Math.Round(_player.AttackPower * (Random.Shared.NextDouble() + 1));
                enemy.TakeDamage(calculatedAttackPower);
                Console.WriteLine($"You dealt {calculatedAttackPower} damage!".Pastel(Color.Cyan));
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
                Console.WriteLine("You failed to escape. The opponent is attacking!".Pastel(BadMessageColor));
                enemy.Attack(_player);
                if (_player.Health <= 0) return FightResult.Defeat;
            }
            else
            {
                Console.WriteLine("Incorrect choice. You lose a turn!".Pastel(BadMessageColor));
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
        return escapeChance;
    }

    private static void DropLoot(ILocation location)
    {
        if (Random.Shared.Next(100) >= 30) return; // 30% chance for loot
        IItem loot = location.PossibleLoot[Random.Shared.Next(location.PossibleLoot.Count)];
        _player.Inventory.Add(loot);
        Console.WriteLine($"Item obtained: {loot.Name.Pastel(Color.Magenta)}!");
    }

    private static void SaveGame(Player playerObj)
    {
        string jsonString = JsonSerializer.Serialize(playerObj);
        File.WriteAllText("savegame.json", jsonString);
        Console.WriteLine("The game has been saved.".Pastel(GoodMessageColor));
    }

    private static Player? LoadGame()
    {
        if (!File.Exists("savegame.json")) return null;
        string jsonString = File.ReadAllText("savegame.json");
        return JsonSerializer.Deserialize<Player>(jsonString);
    }
}
