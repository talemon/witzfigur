using System.Text;

namespace perform_desktop.Data;

public class MoveViewModel
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public float SuccessChance { get; set; }

    public string StatRequirements { get; set; } = string.Empty;
    public string ItemRequirements { get; set; } = string.Empty;

    public string ItemBenefits { get; set; } = string.Empty;
    public string StatBenefits { get; set; } = string.Empty;

    public string FailureItemEffects { get; set; } = string.Empty;
    public string FailureStatEffects { get; set; } = string.Empty;

    public string Score { get; set; } = string.Empty;
    public int ActionPoints { get; set; }

    public static MoveViewModel Make(MoveData data, GameData gameData)
    {
        var vm = new MoveViewModel
        {
            Key = data.Key,
            Name = data.Name,
            SuccessChance = data.SuccessChance,
            Score = data.Score.ToString(),
            ActionPoints = data.ActionPoints
        };

        var statReqs = new StringBuilder();
        foreach (var pair in data.StatRequirements)
        {
            var stat = gameData.GetStat(pair.Key);
            if (stat != null)
            {
                statReqs.AppendLine($"{stat.Name}: {pair.Amount}");
            }
        }

        vm.StatRequirements = statReqs.ToString();

        var itemReqs = new StringBuilder();
        foreach (var pair in data.ItemRequirements)
        {
            var token = gameData.GetTokens(pair.Key);
            if (token != null)
            {
                itemReqs.AppendLine($"{token.Name}: {pair.Amount}");
            }
        }

        vm.ItemRequirements = itemReqs.ToString();

        var statBenefits = new StringBuilder();
        foreach (var pair in data.StatBenefits)
        {
            var stat = gameData.GetStat(pair.Key);
            if (stat != null)
            {
                statBenefits.AppendLine($"{stat.Name}: {pair.Amount}");
            }
        }

        vm.StatBenefits = statBenefits.ToString();

        var itemBenefits = new StringBuilder();
        foreach (var pair in data.ItemBenefits)
        {
            var token = gameData.GetTokens(pair.Key);
            if (token != null)
            {
                itemBenefits.AppendLine($"{token.Name}: {pair.Amount}");
            }
        }

        vm.ItemBenefits = itemBenefits.ToString();

        var failStats = new StringBuilder();
        foreach (var pair in data.FailureStatEffects)
        {
            var stat = gameData.GetStat(pair.Key);
            if (stat != null)
            {
                failStats.AppendLine($"{stat.Name}: {pair.Amount}");
            }
        }

        vm.FailureStatEffects = failStats.ToString();

        var failItems = new StringBuilder();
        foreach (var pair in data.FailureItemEffects)
        {
            var token = gameData.GetTokens(pair.Key);
            if (token != null)
            {
                failItems.AppendLine($"{token.Name}: {pair.Amount}");
            }
        }

        vm.FailureItemEffects = failItems.ToString();

        return vm;
    }
}