using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using perform_desktop.Data;

namespace perform_desktop
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private List<MoveViewModel> _moves = [];
        private StringBuilder _logContent = new StringBuilder();
        private ClearLogCommand? _clearLogCommand;
        private MoveViewModel? _selectedMove;
        private PerformCommand _performCommand;
        private GameData? _gameData;

        private GameState _state;

        public MainViewModel()
        {
            const string gamedataJson = "gamedata.json";
            if (File.Exists(gamedataJson))
            {
                _gameData = JsonConvert.DeserializeObject<GameData>(File.ReadAllText(gamedataJson));

                if (_gameData != null)
                {
                    foreach (var move in _gameData.Moves)
                    {
                        Moves.Add(MoveViewModel.Make(move, _gameData));
                    }

                    _state = new GameState(_gameData);

                    foreach (var statPair in _gameData.StartingStats)
                    {
                        State.TryModifyStat(statPair.Key, statPair.Amount);
                    }
                    foreach (var itemPair in _gameData.StartingItems)
                    {
                        State.TryModifyInventory(itemPair.Key, itemPair.Amount);
                    }
                    State.ActionPoints = _gameData.StartingActionPoints;
                }
            }

            _clearLogCommand = new ClearLogCommand(this);
            _performCommand = new PerformCommand(this);

        }

        public List<MoveViewModel> Moves
        {
            get => _moves;
            set
            {
                if (Equals(value, _moves)) return;
                _moves = value;
                OnPropertyChanged();
            }
        }

        public string LogContent
        {
            get => _logContent.ToString();
        }

        public ClearLogCommand? ClearLogCommand
        {
            get => _clearLogCommand;
            set
            {
                if (Equals(value, _clearLogCommand)) return;
                _clearLogCommand = value;
                OnPropertyChanged();
            }
        }

        public MoveViewModel? SelectedMove
        {
            get => _selectedMove;
            set
            {
                if (Equals(value, _selectedMove)) return;
                _selectedMove = value;
                OnPropertyChanged();
            }
        }

        public PerformCommand PerformCommand
        {
            get => _performCommand;
            set
            {
                if (Equals(value, _performCommand)) return;
                _performCommand = value;
                OnPropertyChanged();
            }
        }

        public GameState State => _state;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void LogText(string str)
        {
            _logContent.AppendLine(str);
            OnPropertyChanged(nameof(LogContent));
        }

        public void ClearLog()
        {
            _logContent.Clear();
            OnPropertyChanged(nameof(LogContent));
        }

        public bool CanPerform(string key)
        {
            var move = _gameData?.GetMove(key);
            if (move != null)
            {
                if (State.ActionPoints < move.ActionPoints)
                    return false;

                foreach (var statReq in move.StatRequirements)
                {
                    if (State.GetStatAmount(statReq.Key) < statReq.Amount)
                        return false;
                }

                foreach (var itemReq in move.ItemRequirements)
                {
                    if (State.GetItemAmount(itemReq.Key) < itemReq.Amount)
                        return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public void Perform(string key)
        {
            var move = _gameData?.GetMove(key);

            if (move == null || CanPerform(key) == false)
                return;

            State.ActionPoints -= move.ActionPoints;

            if (Random.Shared.NextSingle() < move.SuccessChance)
            {
                LogText("Succes!");
                foreach (var statBenefit in move.StatBenefits)
                {
                    State.TryModifyStat(statBenefit.Key, statBenefit.Amount);
                }
                foreach (var tokenBenefit in move.ItemBenefits)
                {
                    State.TryModifyInventory(tokenBenefit.Key, tokenBenefit.Amount);
                }

                State.Score += Random.Shared.Next(move.Score.Min, move.Score.Max);
            }
            else // FAIL
            {
                LogText("Fail!");
                foreach (var statEffect in move.FailureStatEffects)
                {
                    State.TryModifyStat(statEffect.Key, statEffect.Amount);
                }
                foreach (var itemEffect in move.FailureItemEffects)
                {
                    State.TryModifyInventory(itemEffect.Key, itemEffect.Amount);
                }
            }

            if (State.ActionPoints <= 0)
            {
                LogText("GAME OVER!");
            }

            OnPropertyChanged(nameof(State));
        }
    }
}
