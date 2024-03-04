using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Charts.Commands;
using Charts.Utils;
using Microsoft.Win32;

namespace Charts
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            LoadDataCommand = new RelayCommand(LoadData);
        }

        public ICommand LoadDataCommand { get; set; }


        private IReadOnlyDictionary<int, (double Value, DateTime ValueDate)> _stockData;
        public IReadOnlyDictionary<int, (double Value, DateTime ValueDate)> StockData
        {
            get => _stockData;
            set
            {
                if (_stockData == value)
                    return;
                
                _stockData = value;
                OnPropertyChanged();
            }
        }

        private void LoadData()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() != true)
                return;

            var filePath = openFileDialog.FileName;

            var startingColumnIndex = 0;
            var data = CsvUtils.ReadCsvFile(filePath, startingColumnIndex);

            try
            {
                var index = 0;
                var stocks = data.ToDictionary(x => index, y =>
                {
                    var str = y[4].Replace('.', ',');
                    var newStr = Convert.ToDouble(str);
                    index++;
                    return (newStr, DateTime.Parse(y[0]));
                });

                StockData = stocks;
            
            }
            catch (Exception)
            {
                MessageBox.Show("Incorrect data");
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
