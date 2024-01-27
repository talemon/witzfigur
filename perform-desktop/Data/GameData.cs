using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace perform_desktop.Data
{
    public record struct IntRange
    {
        public int Min;
        public int Max;
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
    }
}
