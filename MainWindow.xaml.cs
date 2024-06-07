using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace logik
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Color> _availableColors = new List<Color>
        {
            Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Orange, Colors.Purple
        };

        private List<Color> _secretCode = new List<Color>();
        private List<List<Color>> _guesses = new List<List<Color>>();

        public MainWindow()
        {

            InitializeComponent();
        }

        private void InitializeGame()
        {
            List<Color> SecretCode = new List<Color>();
            for (int i = 0; i < _availableColors.Count; i++)
            {
                SecretCode.Add(_availableColors[i]);
            }
            Random rnd = new Random();
            _secretCode.Clear();

            for (int i = 0; i < 4; i++)
            {
                int count = rnd.Next(SecretCode.Count);
                _secretCode.Add(SecretCode[count]);
                SecretCode.RemoveAt(count);
            }

            _guesses.Clear();
            GuessesControl.Items.Clear();
            GuessesCorrection.Items.Clear();
            StatusLabel.Content = string.Empty;
            InitializeColorSelection();
        }

        private void InitializeColorSelection()
        {
            ColorSelectionPanel.Children.Clear();

            foreach (var color in _availableColors)
            {
                Button colorButton = new Button
                {
                    Width = 30,
                    Height = 30,
                    Background = new SolidColorBrush(color),
                    Margin = new Thickness(5)
                };

                colorButton.Click += ColorButton_Click;
                ColorSelectionPanel.Children.Add(colorButton);
            }

            Button removeButton = new Button
            {
                Width = 30,
                Height = 30,
                Background = Brushes.LightGray,
                Margin = new Thickness(5),
                Content = "<--"
            };
            removeButton.Click += RemoveButton_Click;
            ColorSelectionPanel.Children.Add(removeButton);
        }

        private List<Color> _currentGuess = new List<Color>();

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGuess.Count < 4)
            {
                Button button = sender as Button;
                _currentGuess.Add(((SolidColorBrush)button.Background).Color);
                UpdateGuessDisplay();
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGuess.Count != 0)
            {
            _currentGuess.RemoveAt(_currentGuess.Count - 1);
            UpdateGuessDisplay();
            }
           
        }

        private void UpdateGuessDisplay()
        {
            if (GuessesControl.Items.Count == 0)
            {
                GuessesControl.Items.Add(new StackPanel { Orientation = Orientation.Horizontal });
            }

            var currentGuessPanel = GuessesControl.Items[GuessesControl.Items.Count - 1] as StackPanel;
            currentGuessPanel.Children.Clear();

            currentGuessPanel.Children.Add(new TextBox 
            { 
                Width = 30, 
                Height = 30, 
                Text = $"{(_guesses.Count + 1).ToString()}.", 
                TextAlignment = TextAlignment.Center,
                IsEnabled = false

            });

            foreach (var color in _currentGuess)
            {
                currentGuessPanel.Children.Add(new Border
                {
                    Width = 30,
                    Height = 30,
                    Background = new SolidColorBrush(color),
                    Margin = new Thickness(5)
                });
            }
        }

        private void SubmitGuess_Click(object sender, RoutedEventArgs e)
        {
            if (_currentGuess.Count == 4)
            {
                _guesses.Add(new List<Color>(_currentGuess));
                CheckGuess();
                _currentGuess.Clear();
                GuessesControl.Items.Add(new StackPanel { Orientation = Orientation.Horizontal });
                GuessesCorrection.Items.Add(new StackPanel { Orientation = Orientation.Horizontal });
            }
        }

        private void CheckGuess()
        {
            var lastGuess = _guesses.Last();
            int correctPositionAndColor = 0;
            int correctColorWrongPosition = 0;

            var tempSecret = new List<Color>(_secretCode);
            var tempGuess = new List<Color>(lastGuess);

            for (int i = 0; i < lastGuess.Count; i++)
            {
                if (tempSecret[i] == tempGuess[i])
                {
                    correctPositionAndColor++;
                    tempSecret[i] = Colors.Transparent;
                    tempGuess[i] = Colors.Transparent;
                }
            }

            foreach (var color in tempGuess.Where(c => c != Colors.Transparent))
            {
                if (tempSecret.Contains(color))
                {
                    correctColorWrongPosition++;
                    tempSecret[tempSecret.IndexOf(color)] = Colors.Transparent;
                }
            }

            if (correctPositionAndColor == 4)
            {
                Win();
            }
            else if (_guesses.Count == 10)
            {
                Loss();
            }
            else
            {
                //StatusLabel.Content = $"Correct Position and Color: {correctPositionAndColor}, Correct Color Wrong Position: {correctColorWrongPosition}";
                UpdateGuessPanel(correctPositionAndColor, correctColorWrongPosition);
            }
        }
        public void UpdateGuessPanel(int correctPositionAndColor, int correctColorWrongPosition)
        {
            if (GuessesCorrection.Items.Count == 0)
            {
                GuessesCorrection.Items.Add(new StackPanel { Orientation = Orientation.Horizontal });
            }

            var currentCorrectionPanel = GuessesCorrection.Items[GuessesCorrection.Items.Count - 1] as StackPanel;
            currentCorrectionPanel.Children.Clear();

            for(int i = 0; i < correctPositionAndColor; i++)
            {
                currentCorrectionPanel.Children.Add(new Border
                {
                    Width = 30,
                    Height = 30,
                    Background = Brushes.Black,
                    Margin = new Thickness(5)
                });
            }
            for (int i = 0; i < correctColorWrongPosition; i++)
            {
                currentCorrectionPanel.Children.Add(new Border
                {
                    Width = 30,
                    Height = 30,
                    Background = Brushes.Gray,
                    Margin = new Thickness(5)
                });
            }

            int TheRest = correctColorWrongPosition + correctPositionAndColor;
            if (TheRest != 4)
            {
                for (int i = 0; i < 4 - TheRest; i++)
                {
                    currentCorrectionPanel.Children.Add(new Border
                    {
                        Width = 30,
                        Height = 30,
                        Background = Brushes.White,
                        Margin = new Thickness(5)
                    });
                }
            }
        }
        public void Win()
        {
            CorrectColorPanel.Children.Clear();
            GamePanel.Visibility = Visibility.Hidden;
            GameOverPanel.Visibility = Visibility.Visible;
            EndGameText.Text = "You Win!";

            foreach (var color in _secretCode)
            {
                Border colorButton = new Border
                {
                    Width = 30,
                    Height = 30,
                    Background = new SolidColorBrush(color),
                    Margin = new Thickness(5)
                };

                CorrectColorPanel.Children.Add(colorButton);
            }
        }

        public void Loss()
        {
            CorrectColorPanel.Children.Clear();
            GamePanel.Visibility = Visibility.Hidden;
            GameOverPanel.Visibility = Visibility.Visible;
            EndGameText.Text = "Game Over";

            foreach (var color in _secretCode)
            {
                Border colorButton = new Border
                {
                    Width = 30,
                    Height = 30,
                    Background = new SolidColorBrush(color),
                    Margin = new Thickness(5)
                };

                CorrectColorPanel.Children.Add(colorButton);
            }
        }

        private void GameStart_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame();
            MenuPanel.Visibility = Visibility.Hidden;
            GameOverPanel.Visibility = Visibility.Hidden;
            GamePanel.Visibility = Visibility.Visible;
        }

        private void GameExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
