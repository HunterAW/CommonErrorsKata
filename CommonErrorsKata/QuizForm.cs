using CommonErrorsKata.Shared;
using System.IO;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonErrorsKata
{
    public partial class CommonErrorsForm : Form
    {
        private readonly AnswerQueue<TrueFalseAnswer> answerQueue;
        private readonly string[] files;
        private readonly SynchronizationContext synchronizationContext;
        private int time = 100;
        private string currentBaseName = null;
        private readonly string[] possibleAnswers = null;

        public CommonErrorsForm()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
            files = Directory.GetFiles(Environment.CurrentDirectory + @"..\..\..\ErrorPics");
            possibleAnswers = files.Select(f => Path.GetFileName(f)?.Replace(".png", "")).ToArray();
            lstAnswers.DataSource = possibleAnswers;
            answerQueue = new AnswerQueue<TrueFalseAnswer>(possibleAnswers.Length);
            Next();
            lstAnswers.Click += LstAnswers_Click;
            StartTimer();
        }
        private async void StartTimer()
        {
            await Task.Run(() =>
            {
                for (time = 100; time > 0; time--)
                {
                    UpdateProgress(time);
                    Thread.Sleep(50);
                }

                if (answerQueue.Count >= possibleAnswers.Length && answerQueue.Grade >= 98)
                {
                    Application.Exit();
                }
                else
                {
                    Message("Need to be quicker on your feet next time!  Try again...");
                }

            });
        }

        private void LstAnswers_Click(object sender, EventArgs e)
        {
            //TODO:  Figure out what is a valid answer.
            time = 100;

            var selected = possibleAnswers[lstAnswers.SelectedIndex];
            if (selected == currentBaseName)
            {
                
                answerQueue.Enqueue(new TrueFalseAnswer(true));
            }
            else
            {
                answerQueue.Enqueue(new TrueFalseAnswer(false));
            }

            Next();

        }

        private void Next()
        {

            if (answerQueue.Count >= possibleAnswers.Length && answerQueue.Grade >= 98)
            {
                MessageBox.Show("Congratulations you've defeated me!");
                Application.Exit();
                return;
            }



            label1.Text = answerQueue.Grade.ToString() + "%";
            var file = files.GetRandom();
            currentBaseName = Path.GetFileName(file)?.Replace(".png", "");
            pbImage.ImageLocation = file;
        }

        public void UpdateProgress(int value)
        {
            synchronizationContext.Post(new SendOrPostCallback(x =>
            {
                progress.Value = value;
            }), value);
        }
        public void Message(string value)
        {
            synchronizationContext.Post(new SendOrPostCallback(x =>
            {
                MessageBox.Show(value);
            }), value);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
