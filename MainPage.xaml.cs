using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace StopwatchApp
{
    public partial class MainPage : ContentPage
    {
        private Stopwatch _stopwatch;
        private Stopwatch _lapStopwatch; // Stopwatch for lap time
        private bool _isRunning;
        private ObservableCollection<LapTimeModel> _lapTimes;
        private TimeSpan _lastLapTime;

        public MainPage()
        {
            InitializeComponent();
            _stopwatch = new Stopwatch();
            _lapStopwatch = new Stopwatch();
            _lapTimes = new ObservableCollection<LapTimeModel>();
            LapListView.ItemsSource = _lapTimes;
            Device.StartTimer(TimeSpan.FromMilliseconds(10), UpdateTime); // Update every 10 ms
        }

        private void OnStartStopButtonClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);

            if (_isRunning)
            {
                _stopwatch.Stop();
                _lapStopwatch.Stop();
                StartStopButton.Text = "Start";
                StartStopButton.TextColor = Colors.LimeGreen;
                StartStopButton.BorderColor = Colors.LimeGreen;
            }
            else
            {
                _stopwatch.Start();
                _lapStopwatch.Start();
                StartStopButton.Text = "Stop";
                StartStopButton.TextColor = Colors.Red;
                StartStopButton.BorderColor = Colors.Red;
            }
            _isRunning = !_isRunning;
        }

        private void OnLapButtonClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);

            if (_isRunning)
            {
                var lapTime = _lapStopwatch.Elapsed;
                var totalElapsedTime = _stopwatch.Elapsed;
                _lapTimes.Insert(0, new LapTimeModel
                {
                    LapNumber = $"# {_lapTimes.Count + 1}",
                    LapTime = lapTime.ToString(@"mm\:ss\.ff"),
                    LapDifference = (totalElapsedTime - _lastLapTime).ToString(@"mm\:ss\.ff")
                });
                _lastLapTime = totalElapsedTime;
                _lapStopwatch.Restart();
            }
        }

        private void OnResetButtonClicked(object sender, EventArgs e)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);

            _stopwatch.Reset();
            _lapStopwatch.Reset();
            Device.BeginInvokeOnMainThread(() =>
            {
                TimeLabel.Text = "00:00.00";
                TotalElapsedTimeLabel.Text = "00:00.00";
                StartStopButton.Text = "Start";
                StartStopButton.TextColor = Colors.LimeGreen;
                StartStopButton.BorderColor = Colors.LimeGreen;
                _lapTimes.Clear();
            });
            _isRunning = false;
            _lastLapTime = TimeSpan.Zero;
        }

        private bool UpdateTime()
        {
            if (_isRunning)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    TimeLabel.Text = _lapStopwatch.Elapsed.ToString(@"mm\:ss\.ff");
                    TotalElapsedTimeLabel.Text = _stopwatch.Elapsed.ToString(@"mm\:ss\.ff");
                });
            }
            return true; // Continue running the timer
        }
    }

    public class LapTimeModel
    {
        public string LapNumber { get; set; }
        public string LapTime { get; set; }
        public string LapDifference { get; set; }
    }
}
