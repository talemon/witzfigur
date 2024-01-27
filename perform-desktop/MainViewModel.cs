using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Newtonsoft.Json;
using System.Windows.Input;
using perform_desktop.Data;

namespace perform_desktop
{
    public class StatData
    {
        public string Name { get; set; }
    }
    public class ItemData
    {
        public string Name { get; set; }
    }

    public class StatValue
    {
        public string StatKey { get; set; }
        public int Amount { get; set; }
    }

    public class InventoryItem
    {
        public ItemData Item { get; set; }
        public int Amount { get; set; }
    }


    public class MainViewModel : INotifyPropertyChanged
    {
        private int _score;
        private List<MoveData> _moves = [];
        private int _laughter;
        private int _confusion;
        private int _attention;
        private string _logContent = string.Empty;

        public MainViewModel()
        {
            const string gamedataJson = "gamedata.json";
            if (File.Exists(gamedataJson))
            {
                var gameData = JsonConvert.DeserializeObject<GameData>(File.ReadAllText(gamedataJson));

                if (gameData != null)
                {
                    Moves = [..gameData.Moves];
                }
            }
        }

        public List<MoveData> Moves
        {
            get => _moves;
            set
            {
                if (Equals(value, _moves)) return;
                _moves = value;
                OnPropertyChanged();
            }
        }

        public int Score
        {
            get => _score;
            set
            {
                if (value == _score) return;
                _score = value;
                OnPropertyChanged();
            }
        }

        public int Laughter
        {
            get => _laughter;
            set
            {
                if (value == _laughter) return;
                _laughter = value;
                OnPropertyChanged();
            }
        }

        public int Confusion
        {
            get => _confusion;
            set
            {
                if (value == _confusion) return;
                _confusion = value;
                OnPropertyChanged();
            }
        }

        public int Attention
        {
            get => _attention;
            set
            {
                if (value == _attention) return;
                _attention = value;
                OnPropertyChanged();
            }
        }

        public string LogContent
        {
            get => _logContent;
            set
            {
                if (value == _logContent) return;
                _logContent = value;
                OnPropertyChanged();
            }
        }

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
    }
}
