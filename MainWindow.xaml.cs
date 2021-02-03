using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using TwitcastingCommentViewer;
using ListView = System.Windows.Controls.ListView;
using MessageBox = System.Windows.MessageBox;
using MouseEventHandler = System.Windows.Input.MouseEventHandler;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Threading;
using TwicasRecCommentLoader;
using TwitcastingCommentViewer.Annotations;
using WpfColorFontDialog;
using FontFamily = System.Windows.Media.FontFamily;
using Timer = System.Threading.Timer;

namespace test
{
    
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private OpenFileDialog openFileDialog;
        private ObservableCollection<TwicasComment> DisplayTwicasCommentsList;
        private DispatcherTimer _dispatcherTimer;

        private TimeSpan oldTimeSpan;
        private TimeSpan nowTimeSpan;
        private TimeSpan totalTimeSpan;
        private DateTime StartTime;
        CommentUtil commentUtil;
        private int displayIndex;
        public MainWindow()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)this.CommentView.Items).CollectionChanged += this.ListBoxCollectionChanged;
            CommentViewerWindow.SizeChanged += new SizeChangedEventHandler(sizechanged);
            
            _dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _dispatcherTimer.Tick += new EventHandler(timeChanged);
            _dispatcherTimer.Interval = new TimeSpan(0,0,0,2 );
            
            
        }

        private void ListBoxCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.CommentView.ScrollIntoView(this.CommentView.Items[e.NewStartingIndex]);
                    break;
            }
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            _dispatcherTimer.Start();
            StartTime = DateTime.Now;
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            oldTimeSpan = oldTimeSpan.Add(nowTimeSpan);
            _dispatcherTimer.Stop();
        }

        private void timeChanged(object sender, EventArgs e)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            displayIndex = 0;
            nowTimeSpan = DateTime.Now.Subtract(StartTime);
            totalTimeSpan = oldTimeSpan.Add(nowTimeSpan);
            int time = totalTimeSpan.Hours * 60 * 60 + totalTimeSpan.Minutes * 60 + totalTimeSpan.Seconds;
            TimerLabel.Content = time;

            var list = new Collection<int>();
            /*foreach (var variable in commentUtil.SortedTwicasComments)
            {
                if (variable.Time <= time)
                {
                    DisplayTwicasCommentsList.Add(variable);
                    list.Add(displayIndex);
                    displayIndex = displayIndex + 1;
                }
                else
                {
                    break;
                }
            }
            foreach (var i in list)
            {
                commentUtil.SortedTwicasComments.RemoveAt(i);
            }
            */
            /*
            for (int i = 0; i < commentUtil.SortedTwicasComments.Count; i++)
            {
                if (commentUtil.SortedTwicasComments[0].Time <= time)
                {
                    DisplayTwicasCommentsList.Add(commentUtil.SortedTwicasComments[0]);
                    commentUtil.SortedTwicasComments.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }*/
            while (commentUtil.SortedTwicasComments[0].Time <= time)
            {
                DisplayTwicasCommentsList.Add(commentUtil.SortedTwicasComments[0]);
                commentUtil.SortedTwicasComments.RemoveAt(0);
            }
            {
                
            }

            stopwatch.Stop();
            DetailLabel.Content = (stopwatch.Elapsed);
            
        }

        private void sizechanged(object sender, SizeChangedEventArgs e)
        {
            DetailLabel.Content = CommentViewerWindow.Width + "ああ" + CommentViewerWindow.Height;
        }


        /*private void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ErrorTextBox.Content = commentUtil.TwicasComments[Int32.Parse(textbox.Text)].ToString();
            }
     PlayButton_OnClickmentOutOfRangeException)
            {
         fontSetting_OnClick.Content = "コメント数の上限を超えています";
            }
            catch (FormatException)
            {
                ErrorTextStopButton_OnClick外の文字が入力されました";
            }
            
        }*/

        private void ButtonLoad_OnClicked(object sender, RoutedEventArgs e)
        {
            try
            {

                DisplayTwicasCommentsList = new ObservableCollection<TwicasComment>();
                commentUtil = new CommentUtil(openFileDialog.FileName);
                //commentUtil = new CommentUtil(openFileDialog.FileName);
                //ResetView(CommentView);
                //CommentView.ItemsSource = DisplayTwicasCommentsList;
                CommentView.ItemsSource = DisplayTwicasCommentsList;
                //CommentView.ItemsSource = commentUtil.twicasCommentsOrderBy;
                DetailLabel.Content = "ロードが完了しました。";
                oldTimeSpan = new TimeSpan(0, 0, commentUtil.SortedTwicasComments[0].Time);

            }
            catch (ArgumentException)
            {
                DetailLabel.Content = "コメントファイルが読み込めませんでした";
            }
            catch (NoCommentException)
            {
                DetailLabel.Content = "コメントが検出されませんでした。";
            }

        }


        private void ResetView(ListView listView)
        {
            for (int i = 0; i < listView.Items.Count; i++)
            {
                listView.Items.RemoveAt(i);
            }
        }

        /// <summary>
        /// object sender にはイベントが発生したコントロールが入っている
        /// </summary>
        /// <param name="seTest"></param>
        /// <param name="e"></param>
        private void ButtonOpenFileDialog_OnClicked(object seTest, RoutedEventArgs e)
        {
            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ツイキャスコメントファイル (*.txt)|*.txt";
            openFileDialog.ShowDialog();
            PathBox.Text = openFileDialog.FileName;
        }

        private void fontSetting_OnClick(object sender, RoutedEventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.ShowDialog();
            Font font = fontDialog.Font;
            CommentView.FontFamily = new FontFamily(font.Name);
            CommentView.FontSize = font.Size;
        }

        private void fontDialog_Apply(object sender, EventArgs e)
        {
            MessageBox.Show("こんにちは");
        }
    }

    public class ListViewLiner : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}