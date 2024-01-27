using System.Text;
using perform_desktop.Data;

namespace perform_desktop;

public class GameState
{
    private GameData _gameData;

    private readonly Dictionary<string, int> _stats = new();
    private readonly Dictionary<string, int> _tokens = new();

    public int Score;
    public int ActionPoints;

    public GameState(GameData gameData)
    {
        _gameData = gameData;
    }

    private bool TryModify(Dictionary<string, int> target, string key, int amount)
    {
        if (amount < 0)
        {
            if (_stats.TryGetValue(key, out var currentAmount))
            {
                if (currentAmount < Math.Abs(currentAmount))
                {
                    return false;
                }

                _stats[key] += amount;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (!_stats.TryAdd(key, amount))
            {
                _stats[key] += amount;
            }
        }
            
        return true;
    }

    public bool TryModifyStat(string key, int amount)
    {
        return TryModify(_stats, key, amount);
    }
    public bool TryModifyInventory(string key, int amount)
    {
        return TryModify(_tokens, key, amount);
    }

    public int GetStatAmount(string key)
    {
        return _stats.GetValueOrDefault(key, 0);
    }

    public int GetItemAmount(string key)
    {
        return _tokens.GetValueOrDefault(key, 0);
    }

    public override string ToString()
    {
        StringBuilder str = new StringBuilder();

        str.Append($"Score: {Score} Action Points: {ActionPoints} - Stats: ");
        foreach (var statPair in _stats)
        {
            var name = _gameData.GetStat(statPair.Key)?.Name ?? statPair.Key;
            str.Append($"{name}: {statPair.Value} ");
        }

        str.Append(" Items: ");

        foreach (var itemPair in _tokens)
        {
            var name = _gameData.GetStat(itemPair.Key)?.Name ?? itemPair.Key;
            str.Append($"{name}: {itemPair.Value} ");
        }

        return str.ToString();
    }
}