using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;

            string symbol = button.Content.ToString();
            string input = calculationInput.Text ?? string.Empty;
            string[] operators = { "+", "-", "*", "/" };

            // Wenn Eingabe leer und Symbol ein Operator ist -> ignorieren
            if (string.IsNullOrEmpty(input))
            {
                if (operators.Contains(symbol))
                    return;

                calculationInput.Text += symbol;
                return;
            }

            char lastChar = input[input.Length - 1];

            if (operators.Contains(symbol) && operators.Contains(lastChar.ToString()))
            {
                // Ersetze den letzten Operator durch den neuen
                calculationInput.Text = input.Substring(0, input.Length - 1) + symbol;
                return;
            }

            calculationInput.Text += symbol;
        }
        
        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string input = calculationInput.Text.Replace(" ", "").Replace(',', '.');
                
                var parts = Regex.Split(input, @"([+\-])").Where(x => x.Length > 0).ToArray();

                double total = 0.0;
                char sign = '+';

                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i] == "+" || parts[i] == "-")
                    {
                        sign = parts[i][0];
                        continue;
                    }

                    // Punkt-Ebene: ["3", "*", "4", "/", "2"]
                    var tok = Regex.Split(parts[i], @"([*/])").Where(x => x.Length > 0).ToArray();

                    double term = double.Parse(tok[0], CultureInfo.InvariantCulture);
                    for (int j = 1; j < tok.Length; j += 2)
                    {
                        double right = double.Parse(tok[j + 1], CultureInfo.InvariantCulture);
                        if (tok[j] == "*") term *= right;
                        else
                        {
                            if (right == 0) throw new DivideByZeroException();
                            term /= right;
                        }
                    }

                    total = (sign == '+') ? total + term : total - term;
                    sign = '+'; // reset
                }

                calculationInput.Text = total.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            calculationInput.Text = string.Empty;
        }

        private void BackspaceButton_Click(object sender, RoutedEventArgs e)
        {
            if (calculationInput.Text.Length > 0)
            {
                calculationInput.Text = calculationInput.Text.Substring(0, calculationInput.Text.Length - 1);
            }
        }

        private void dbTest_Click(object sender, RoutedEventArgs e)
        {
            string sql = "Select * From Professor Where PersNr = @MeineNummer";
            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                {"@MeineNummer", Convert.ToInt32(calculationInput.Text)}
            };

            Dapper dapper = new Dapper();
            List<Professor> profs = new List<Professor>();
            profs = dapper.readData<Professor>(sql, dict);

            MessageBox.Show($"Hallo Herr {profs[0].Name}");
        }
    }
}