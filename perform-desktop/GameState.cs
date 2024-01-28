using System.Text;
using perform_desktop.Data;

namespace perform_desktop;

public class GameState(GameData gameData)
{
    private readonly Dictionary<string, int> _stats = new();
    private readonly Dictionary<string, int> _tokens = new();

    public int Score;
    public int ActionPoints;

    private static bool TryModify(Dictionary<string, int> target, string key, int amount)
    {
        if (target.TryGetValue(key, out var currentAmount))
        {
            target[key] = Math.Clamp(currentAmount + amount, 0, 100);
        }
        else
        {
            target.Add(key, Math.Clamp(amount, 0, 100));
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
            var name = gameData.GetStat(statPair.Key)?.Name ?? statPair.Key;
            str.Append($"{name}: {statPair.Value} ");
        }

        str.Append(" Items: ");

        foreach (var itemPair in _tokens)
        {
            var name = gameData.GetTokens(itemPair.Key)?.Name ?? itemPair.Key;
            str.Append($"{name}: {itemPair.Value} ");
        }

        return str.ToString();
    }

    public bool CanPerform(string key)
    {
        var move = gameData?.GetMove(key);
        if (move != null)
        {
            if (ActionPoints < move.ActionPoints)
                return false;

            foreach (var statReq in move.StatRequirements)
            {
                if (statReq.Amount < 0)
                {
                    if (GetStatAmount(statReq.Key) > Math.Abs(statReq.Amount))
                        return false;
                }
                else if (GetStatAmount(statReq.Key) < statReq.Amount)
                    return false;
            }

            foreach (var itemReq in move.ItemRequirements)
            {
                if (GetItemAmount(itemReq.Key) < itemReq.Amount)
                    return false;
            }
        }
        else
        {
            return false;
        }

        return true;
    }
}