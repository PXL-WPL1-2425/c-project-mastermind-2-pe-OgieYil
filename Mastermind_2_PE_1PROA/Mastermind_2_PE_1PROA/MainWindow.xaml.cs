using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static System.Formats.Asn1.AsnWriter;

namespace Mastermind_PE_Oguzhan_Yilmaz_1PROA
{
    public partial class MainWindow : Window
    {
        private string[] generatedCode;
        private int attemptsLeft = 10;
        private int totalPenaltyPoints = 0;

        public MainWindow()
        {
            InitializeComponent();
            GenerateRandomCode();
            OpvullenComboBoxes();
            UpdateTitle();
        }

        private void GenerateRandomCode()
        {
            Random random = new Random();
            string[] Colors = { "Rood", "Geel", "Oranje", "Wit", "Groen", "Blauw" };
            generatedCode = Enumerable.Range(0, 4).Select(_ => Colors[random.Next(Colors.Length)]).ToArray();
        }

        private void OpvullenComboBoxes()
        {
            string[] Colors = { "Rood", "Geel", "Oranje", "Wit", "Groen", "Blauw" };
            ComboBox1.ItemsSource = Colors;
            ComboBox2.ItemsSource = Colors;
            ComboBox3.ItemsSource = Colors;
            ComboBox4.ItemsSource = Colors;
        }

        private void UpdateTitle()
        {
            this.Title = $"MasterMind - Pogingen over: {attemptsLeft}";
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Label1.Background = GetBrushFromColorName(ComboBox1.SelectedItem as string ?? "default");
            Label2.Background = GetBrushFromColorName(ComboBox2.SelectedItem as string ?? "default");
            Label3.Background = GetBrushFromColorName(ComboBox3.SelectedItem as string ?? "default");
            Label4.Background = GetBrushFromColorName(ComboBox4.SelectedItem as string ?? "default");
        }

        private SolidColorBrush GetBrushFromColorName(string colorName)
        {
            return colorName switch
            {
                "Rood" => Brushes.Red,
                "Geel" => Brushes.Yellow,
                "Oranje" => Brushes.Orange,
                "Wit" => Brushes.White,
                "Groen" => Brushes.Green,
                "Blauw" => Brushes.Blue,
                _ => Brushes.Transparent
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (attemptsLeft <= 0)
            {
                ShowEndGameMessage(false);
                return;
            }

            string[] userCode = {
                ComboBox1.SelectedItem != null ? ComboBox1.SelectedItem.ToString() : "default",
                ComboBox2.SelectedItem != null ? ComboBox2.SelectedItem.ToString() : "default",
                ComboBox3.SelectedItem != null ? ComboBox3.SelectedItem.ToString() : "default",
                ComboBox4.SelectedItem != null ? ComboBox4.SelectedItem.ToString() : "default"
            };

            int score = CalculateScore(userCode);
            totalPenaltyPoints += score;
            DisplayScore(score);
            string feedback = GenerateFeedback(userCode);
            LogAttempt(userCode, feedback);

            CheckColor(Label1, userCode[0], 0);
            CheckColor(Label2, userCode[1], 1);
            CheckColor(Label3, userCode[2], 2);
            CheckColor(Label4, userCode[3], 3);

            if (userCode.SequenceEqual(generatedCode))
            {
                ShowEndGameMessage(true);
                return;
            }

            attemptsLeft--;
            UpdateTitle();

            if (attemptsLeft == 0)
            {
                ShowEndGameMessage(false);
            }
        }

        private int CalculateScore(string[] userCode)
        {
            int score = 0;

            for (int i = 0; i < 4; i++)
            {
                if (userCode[i] == generatedCode[i])
                {
                    continue;
                }
                else if (generatedCode.Contains(userCode[i]))
                {
                    score += 1;
                }
                else
                {
                    score += 2;
                }
            }

            return score;
        }

        private void DisplayScore(int score)
        {
            ScoreLabel.Content = $"Score: {score} Strafpunten | Totale strafpunten: {totalPenaltyPoints}";
        }

        private string GenerateFeedback(string[] userCode)
        {
            int correctPosition = 0;
            int correctColorWrongPosition = 0;

            for (int i = 0; i < 4; i++)
            {
                if (userCode[i] == generatedCode[i])
                {
                    correctPosition++;
                }
                else if (generatedCode.Contains(userCode[i]))
                {
                    correctColorWrongPosition++;
                }
            }

            return $"Rood: {correctPosition}, Wit: {correctColorWrongPosition}";
        }

        private void LogAttempt(string[] userCode, string feedback)
        {
            string attempt = $"Poging: {string.Join(", ", userCode)} | Feedback: {feedback}";
            AttemptsListBox.Items.Add(attempt);
        }

        private void CheckColor(Label label, string selectedColor, int position)
        {
            if (selectedColor == generatedCode[position])
            {
                label.BorderBrush = new SolidColorBrush(Colors.DarkRed);
                label.BorderThickness = new Thickness(3);
            }
            else if (generatedCode.Contains(selectedColor))
            {
                label.BorderBrush = new SolidColorBrush(Colors.Wheat);
                label.BorderThickness = new Thickness(3);
            }
            else
            {
                label.BorderBrush = Brushes.Transparent;
                label.BorderThickness = new Thickness(0);
            }
        }

        private void ShowEndGameMessage(bool isWinner)
        {
            string message = isWinner
                ? $"Gefeliciteerd! Je hebt de code gekraakt!\nTotale strafpunten: {totalPenaltyPoints}"
                : $"Game Over! De code was: {string.Join(", ", generatedCode)}\nTotale strafpunten: {totalPenaltyPoints}";

            MessageBoxResult result = MessageBox.Show(
                isWinner ? "Gewonnen!" : "Game Over"
            );
        }

        private void ResetGame()
        {
            GenerateRandomCode();
            attemptsLeft = 10;
            totalPenaltyPoints = 0;
            UpdateTitle();

            ComboBox1.SelectedItem = null;
            ComboBox2.SelectedItem = null;
            ComboBox3.SelectedItem = null;
            ComboBox4.SelectedItem = null;

            Label1.BorderBrush = Brushes.Transparent;
            Label2.BorderBrush = Brushes.Transparent;
            Label3.BorderBrush = Brushes.Transparent;
            Label4.BorderBrush = Brushes.Transparent;

            Label1.Background = Brushes.LightGray;
            Label2.Background = Brushes.LightGray;
            Label3.Background = Brushes.LightGray;
            Label4.Background = Brushes.LightGray;

            AttemptsListBox.Items.Clear();
            ScoreLabel.Content = "Score: 0 Strafpunten | Totale strafpunten: 0";
        }

        // Handle the window closing event
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            MessageBoxResult result = MessageBox.Show(
                "Wilt u het spel vroegtijdig beëindigen?",
                "Beëindigen?",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );


            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;  // Cancel the closing event if the user clicks 'No'
            }
            else
            {
                Application.Current.Shutdown();  // Allow the app to close if the user clicks 'Yes'
            }


        }
        private void MnuAfsluiten_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();  // Allow the app to close
        }

        private void nieuwspel_Click(object sender, EventArgs e)
        {
            GenerateRandomCode();
            attemptsLeft = 10;
            totalPenaltyPoints = 0;
            UpdateTitle();

            ComboBox1.SelectedItem = null;
            ComboBox2.SelectedItem = null;
            ComboBox3.SelectedItem = null;
            ComboBox4.SelectedItem = null;

            Label1.BorderBrush = Brushes.Transparent;
            Label2.BorderBrush = Brushes.Transparent;
            Label3.BorderBrush = Brushes.Transparent;
            Label4.BorderBrush = Brushes.Transparent;

            Label1.Background = Brushes.LightGray;
            Label2.Background = Brushes.LightGray;
            Label3.Background = Brushes.LightGray;
            Label4.Background = Brushes.LightGray;

            AttemptsListBox.Items.Clear();
            ScoreLabel.Content = "Score: 0 Strafpunten | Totale strafpunten: 0";

        }



        private void Name_Load(object sender, EventArgs e)
        {
           // string playerName = StartGame();


           //Toon de naam van de speler in een label
           // playerNameLabel.Text = $"Welkom, {playerName}!";
        }

        private void highscore_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
