using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Platform
{
    public class ViewModel : BaseViewModel
    {
        private static ViewModel _instance;
        private ReaderTxtFile _readerTxtFile;
        private string _pathToFile = string.Empty;
        private string _textLogReadFile = string.Empty;
        private string _textLogMap = string.Empty;
        private string _textLogResult = string.Empty;
        private FileData _fileData;

        public List<ResultRoute> ResultRoutes = new List<ResultRoute>();
        public List<Bus> Buses = new List<Bus>();
        public ICommand PickFileCommand { get; set; }
        public ICommand ReadFileCommand { get; set; }
        public ICommand CalculateCommand { get; set; }

        public string PathToFile
        {
            get
            { return _pathToFile; }
            set
            {
                _pathToFile = value;
                if(_pathToFile == null)
                {

                }
                OnPropertyChanged(nameof(PathToFile));
            }
        }
        public string TextLogReadFile
        {
            get
            { return _textLogReadFile; }
            set
            {
                _textLogReadFile = value;
                OnPropertyChanged(nameof(TextLogReadFile));
            }
        }
        public string TextLogMap
        {
            get
            { return _textLogMap; }
            set
            {
                _textLogMap = value;
                OnPropertyChanged(nameof(TextLogMap));
            }
        }
        public string TextLogResult
        { 
            get 
            { return _textLogResult; } 
            set 
            {
                _textLogResult = value;
                OnPropertyChanged(nameof(TextLogResult));
            } 
        }

        public static ViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ViewModel();

                return _instance;
            }
        }
        protected ViewModel()
        {
            PickFileCommand = new RelayCommand(PickFile);
            ReadFileCommand = new RelayCommand(ReadFile);
            CalculateCommand = new RelayCommand(Calculate);          
        }

        private void PickFile()
        {
            _readerTxtFile = new ReaderTxtFile();
            string path = _readerTxtFile.GetRouteFilePath();
            if (!string.IsNullOrWhiteSpace(path))
            {
                PathToFile = path;
            }                
        }
        private void ReadFile()
        {
            if(!string.IsNullOrWhiteSpace(PathToFile))
            {
                (_fileData, Buses) = _readerTxtFile.ReadFile(PathToFile);

                TextLogReadFile = InfoView.LogRead(_fileData);
                TextLogReadFile += InfoView.LogAfterRead(Buses);
            }            
        }
        private void Calculate()
        {
            TimeSpan timeArrive;
            int? start = MainWindow.Instance.CB_StartStop.SelectedItem as int?;
            int? end = MainWindow.Instance.CB_EndStop.SelectedItem as int?;

            int? hours = MainWindow.Instance.CB_TimeHour.SelectedItem as int?;
            int? minutes = MainWindow.Instance.CB_TimeMinute.SelectedItem as int?;
            if (start == null || end == null || start == end)
            {
                InfoView.InfoMessage("Выберите стартовую и конечную точку (они должны отличаться)");
                return;
            }
            if (hours == null || minutes == null)
            {
                InfoView.InfoMessage("Выберите время отправки");
                return;
            }

            timeArrive = new TimeSpan(Constants.ConvertToInt(hours), Constants.ConvertToInt(minutes), 0);
            FinderPaths finderPaths = new(_fileData.NumberStop, timeArrive);

            ResultRoutes = finderPaths.CreateMap(Constants.ConvertToInt(start), Constants.ConvertToInt(end));  
            
            if(ResultRoutes.Any())
                TextLogResult = InfoView.LogResult(ResultRoutes, Buses);
            else
                InfoView.InfoMessage("Маршруты не найдены");            
        }

    }
}
