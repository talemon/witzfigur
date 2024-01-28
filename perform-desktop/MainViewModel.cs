using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using perform_desktop.Data;

namespace perform_desktop
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private List<MoveViewModel> _moves = [];
        private readonly StringBuilder _logContent = new();
        private ClearLogCommand? _clearLogCommand;
        private MoveViewModel? _selectedMove;
        private PerformCommand _performCommand;
        private GameData? _gameData;

        private GameState? _state;

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

                    InitializeState(_state, _gameData);
                }
            }

            _clearLogCommand = new ClearLogCommand(this);
            _performCommand = new PerformCommand(this);
            QuitCommand = new RelayCommand(_ =>
            {
                if (Application.Current.MainWindow != null) Application.Current.MainWindow.Close();
            });

            RestartGameCommand = new RelayCommand(_ =>
            {
                Debug.Assert(_gameData != null, nameof(_gameData) + " != null");
                _state = new GameState(_gameData);
                InitializeState(_state, _gameData);
                OnPropertyChanged(nameof(State));
                ClearLog();
            });
        }

        private void InitializeState(GameState gameState, GameData gameData)
        {
            foreach (var statPair in gameData.StartingStats)
            {
                gameState.TryModifyStat(statPair.Key, statPair.Amount);
            }
            foreach (var itemPair in gameData.StartingItems)
            {
                gameState.TryModifyInventory(itemPair.Key, itemPair.Amount);
            }
            gameState.ActionPoints = gameData.StartingActionPoints;
        }

        public event Action? Performed;

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

        public string LogContent => _logContent.ToString();

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

        public RelayCommand QuitCommand { get; }
        public RelayCommand RestartGameCommand { get; }

        public GameState? State => _state;

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

        public void Perform(string key)
        {
            var move = _gameData?.GetMove(key);

            if (move == null || _state == null || _state.CanPerform(key) == false)
                return;

            _state.ActionPoints -= move.ActionPoints;

            if (Random.Shared.NextSingle() < move.SuccessChance)
            {
                LogText($"{move.Name} Succes!");
                foreach (var statBenefit in move.StatBenefits)
                {
                    _state.TryModifyStat(statBenefit.Key, statBenefit.Amount);
                }
                foreach (var tokenBenefit in move.ItemBenefits)
                {
                    _state.TryModifyInventory(tokenBenefit.Key, tokenBenefit.Amount);
                }

                var score = Random.Shared.Next(move.Score.Min, move.Score.Max);
                var laughter = _state.GetStatAmount("laugh");
                double chance = laughter * laughter * 0.00005f;
                if (Random.Shared.NextDouble() < chance)
                {
                    LogText("CRITICAL!");
                    score *= 2;
                }
                _state.Score += score;
            }
            else // FAIL
            {
                LogText($"{move.Name} Fail!");
                foreach (var statEffect in move.FailureStatEffects)
                {
                    _state.TryModifyStat(statEffect.Key, statEffect.Amount);
                }
                foreach (var itemEffect in move.FailureItemEffects)
                {
                    _state.TryModifyInventory(itemEffect.Key, itemEffect.Amount);
                }
            }

            if (_state.ActionPoints <= 0)
            {
                LogText("GAME OVER!");
            }

            OnPropertyChanged(nameof(State));
            Performed?.Invoke();
        }
    }
}
