using System;
using System.Collections.ObjectModel;
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
using System.Windows.Forms.VisualStyles;
using System.Windows.Threading;
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
        private ObservableCollection<TwicasComment> TwicasCommentList;
        private DispatcherTimer _dispatcherTimer;

        private TimeSpan oldTimeSpan;
        private TimeSpan nowTimeSpan;
        private DateTime StartTime;
        
        public MainWindow()
        {
            InitializeComponent();
            
            CommentViewerWindow.SizeChanged += new SizeChangedEventHandler(sizechanged);
            
            _dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            _dispatcherTimer.Tick += new EventHandler(timeChanged);
            _dispatcherTimer.Interval = new TimeSpan(0,0,0,0,1);
            
            
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
            nowTimeSpan = DateTime.Now.Subtract(StartTime);
            TimerLabel.Content = oldTimeSpan.Add(nowTimeSpan).ToString(@"mm\:ss\:ff");
        }

        private void sizechanged(object sender, SizeChangedEventArgs e)
        {
            ErrorTextBox.Content = CommentViewerWindow.Width + "ああ" + CommentViewerWindow.Height;
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
            CommentUtil commentUtil = null;
            TwicasCommentList = null;
            try
            {

                commentUtil = new CommentUtil(openFileDialog.FileName);
                //commentUtil = new CommentUtil(openFileDialog.FileName);
                TwicasCommentList = commentUtil.TwicasComments;
                //ResetView(CommentView);
                CommentView.ItemsSource = TwicasCommentList;


            }
            catch (ArgumentException)
            {
                ErrorTextBox.Content = "コメントファイルが読み込めませんでした";
            }
            catch (NoCommentException)
            {
                ErrorTextBox.Content = "コメントが検出されませんでした。";
            }
            
            
            MessageBox.Show(commentUtil.TwicasComments[5].Comment);
            
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

        private void Test(object sender, RoutedEventArgs e)
        {
            /*ObservableCollection<string> testcollection = new ObservableCollection<string>();
            testcollection.Add("うんこ");
            testcollection.Add("ｊｆｌｄｊ");
            testcollection.Add("ｌｆぁｓｆｊ");
            CommentView.ItemsSource = testcollection;*/
            TwicasCommentList?.Clear();
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