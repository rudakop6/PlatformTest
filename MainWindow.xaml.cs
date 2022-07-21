using System;
using System.Linq;
using System.Windows;

namespace Platform
{
    public partial class MainWindow : Window
    {
        private readonly int[] _hours = new int[Constants.NumberHoursInDay];
        private readonly int[] _minutes = new int[Constants.NumberMinutesInHour]; 
        private static MainWindow? _instance;

        public static MainWindow Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MainWindow();

                return _instance;
            }
        }
        public MainWindow()
        {
            _instance = this;
            InitializeComponent();
            InitTimeCB();
            MainForm.DataContext = ViewModel.Instance;
        }
        private void InitTimeCB()
        {
            for (int i = 0; i < Constants.NumberMinutesInHour; i++)
            {
                _minutes[i] = i;
                if (i < Constants.NumberHoursInDay)
                    _hours[i] = i;               
            }
            CB_TimeHour.ItemsSource = _hours;
            CB_TimeMinute.ItemsSource = _minutes;

            CB_TimeHour.SelectedItem = CB_TimeHour.Items[0];
            CB_TimeMinute.SelectedItem = CB_TimeMinute.Items[0];
        }

        public void InitPointCB(int numberStop)
        {
            int[] _stops = new int[numberStop];
            for (int i = 1; i < numberStop + 1; i++)
            {
                _stops[i - 1] = i;
            }
            MainWindow.Instance.CB_StartStop.ItemsSource = MainWindow.Instance.CB_EndStop.ItemsSource = _stops;
            MainWindow.Instance.CB_StartStop.SelectedItem = MainWindow.Instance.CB_StartStop.Items[0];
            MainWindow.Instance.CB_EndStop.SelectedItem = MainWindow.Instance.CB_EndStop.Items[numberStop - 1];
            MainWindow.Instance.CB_StartStop.IsEnabled = MainWindow.Instance.CB_EndStop.IsEnabled = true;
        }
    }
}
