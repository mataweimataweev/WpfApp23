using Microsoft.WindowsAPICodePack.Dialogs;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WPFAudioPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private List<string> filenames = new List<string>();
        private int currentTrackIndex = -1;
        private DispatcherTimer timer = new DispatcherTimer();
        private bool isReplayOn = false;
        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer.MediaEnded += Media_MediaEnded;
            mediaPlayer.MediaOpened += Media_MediaOpened;

            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;
        }

        private void Media_MediaOpened(object? sender, EventArgs e)
        {
            if (mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                SliderMusic.Minimum = 0;
                SliderMusic.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
                SliderMusic.TickFrequency = 1;
                SliderMusic.IsSnapToTickEnabled = true;
                SliderMusic.ValueChanged -= SliderMusic_ValueChanged;
                SliderMusic.ValueChanged += SliderMusic_ValueChanged;

                timer.Start();

                TimeSpan totalDuration = mediaPlayer.NaturalDuration.TimeSpan;
                musicTime.Text = $"{totalDuration.Hours:D2}:{totalDuration.Minutes:D2}:{totalDuration.Seconds:D2}";
            }
        }

        private void Media_MediaEnded(object? sender, EventArgs e)
        {
            if (isReplayOn)
            {
                mediaPlayer.Position = TimeSpan.Zero;
            }
            else if (currentTrackIndex < filenames.Count - 1)
            {
                currentTrackIndex++;
                PlayTrack();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mediaPlayer.Source != null && mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                SliderMusic.Value = mediaPlayer.Position.TotalMilliseconds;

                TimeSpan currentPosition = mediaPlayer.Position;
                timerNow.Text = $"{currentPosition.Hours:D2}:{currentPosition.Minutes:D2}:{currentPosition.Seconds:D2}";
            }
        }


        private void OpenGovno_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog
            {
                Multiselect = true,
                DefaultExtension = ".mp3"
            };
            fileDialog.Filters.Add(new CommonFileDialogFilter("MP3 files", "*.mp3"));
            var result = fileDialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                foreach (string filename in fileDialog.FileNames)
                {
                    FileXyu1.Items.Add(System.IO.Path.GetFileName(filename));
                    filenames.Add(filename);
                }

                currentTrackIndex = 0;
                PlayTrack();
            }
        }

        private void BackGovno_Click(object sender, RoutedEventArgs e)
        {
            int index = currentTrackIndex;

            if (index == 0)
            {
                index = filenames.Count - 1;
            }
            else
            {
                index--;
            }

            currentTrackIndex = index;
            PlayTrack();
        }

        private void PlayGovno_Click(object sender, RoutedEventArgs e)
        {
            int index = FileXyu1.SelectedIndex;
            if (index >= 0)
            {
                currentTrackIndex = index;
                PlayTrack();
            }
        }

        private void PauseGovno_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        private void RepeatGovno_Click(object sender, RoutedEventArgs e)
        {
            isReplayOn = !isReplayOn;
            if (isReplayOn)
            {
                RandomGovno.Label = "Replay On";
            }
            else
            {
                RandomGovno.Label = "Replay Off";
            }
        }

        private void NextGovno_Click(object sender, RoutedEventArgs e)
        {
            int index = currentTrackIndex;

            if (index == filenames.Count - 1)
            {
                index = 0;
            }
            else
            {
                index++;
            }

            currentTrackIndex = index;
            PlayTrack();
        }

        private void SliderMusic_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Position = TimeSpan.FromMilliseconds(SliderMusic.Value);
        }

        private void PlayTrack()
        {
            if (currentTrackIndex >= 0 && currentTrackIndex < filenames.Count)
            {
                FileXyu1.SelectedIndex = currentTrackIndex;
                if (isReplayOn && mediaPlayer.Position == TimeSpan.Zero && currentTrackIndex != 0)
                {
                    currentTrackIndex--;
                }
                else
                {
                    mediaPlayer.Open(new Uri(filenames[currentTrackIndex]));
                    mediaPlayer.Play();
                    SliderMusic.Value = 0;
                }
            }
        }
    }
}