namespace perform_desktop.Data
{
    public readonly record struct IntRange(int Min, int Max)
    {
        public override string ToString()
        {
            return $"{Min}-{Max}";
        }
    }

    public record struct KeyAmount
    {
        public string Key;
        public int Amount;
    }

    public class StatData
    {
        public string Key = string.Empty;
        public string Name = string.Empty;
    }

    public class TokenData
    {
        public string Key = string.Empty;
        public string Name = string.Empty;
    }

    public class MoveData
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public float SuccessChance { get; set; }

        public KeyAmount[] StatRequirements { get; set; } = [];
        public KeyAmount[] ItemRequirements { get; set; } = [];

        public KeyAmount[] ItemBenefits { get; set; } = [];
        public KeyAmount[] StatBenefits { get; set; } = [];

        public KeyAmount[] FailureItemEffects { get; set; } = [];
        public KeyAmount[] FailureStatEffects { get; set; } = [];

        public IntRange Score { get; set; }
        public int ActionPoints { get; set; }
    }

    public class GameData
    {
        public MoveData[] Moves = [];
        public StatData[] Stats = [];
        public TokenData[] Tokens = [];

        public KeyAmount[] StartingStats = [];
        public KeyAmount[] StartingItems = [];

        public int StartingActionPoints;

        public StatData? GetStat(string key)
        {
            return Stats.FirstOrDefault(data => data.Key == key);
        }

        public TokenData? GetTokens(string key)
        {
            return Tokens.FirstOrDefault(data => data.Key == key);
        }
        public MoveData? GetMove(string key)
        {
            return Moves.FirstOrDefault(data => data.Key == key);
        }
    }
}
