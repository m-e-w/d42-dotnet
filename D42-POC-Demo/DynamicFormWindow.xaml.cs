using D42_POC_Demo.Classes;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace D42_POC_Demo
{
    /// <summary>
    /// Interaction logic for DynamicFormWindow.xaml
    /// </summary>
    public partial class DynamicFormWindow : Window
    {
        private object _sourceObject;
        private Dictionary<string, TextBox> returnPayload = new Dictionary<string, TextBox>();

        public bool Saved { get; set; } = false;

        public DynamicFormWindow(object source, string title)
        {
            InitializeComponent();
            Title = title;
            _sourceObject = source;
        }
        public Dictionary<string, string> GetSavedObject()
        {
            Dictionary<string, string> payload = new Dictionary<string, string>();

            foreach(var item in returnPayload)
            {
                payload.Add(item.Key, item.Value.Text);
            }
            return payload;
        }
        public void CreateControls<T>(Dictionary<string, int> ui_Order)
        {
            Grid rootGrid = new Grid();
            rootGrid.Margin = new Thickness(10,0,10,0);

            rootGrid.ColumnDefinitions.Add(
               new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            rootGrid.ColumnDefinitions.Add(
                 new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            rootGrid.ColumnDefinitions.Add(
                new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            int j = ui_Order.Count;

            this.Height = 40 + (30 * (propertyInfos.Length + 2));
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.PropertyType.Name == "String")
                {

                    rootGrid.RowDefinitions.Add(CreateRowDefinition(30));

                    TextBlock label;
                    TextBox textbox;
                    if(ui_Order.ContainsKey(propertyInfo.Name))
                    {
                        ui_Order.TryGetValue(propertyInfo.Name, out int row);
                        label = CreateTextBlock(propertyInfo.Name,row, 0);
                        textbox = CreateTextBox((string)propertyInfo.GetValue(_sourceObject), row, 2);
                        textbox.TabIndex = row;
                    }
                    else
                    {
                        label = CreateTextBlock(propertyInfo.Name, j, 0);
                        textbox = CreateTextBox((string)propertyInfo.GetValue(_sourceObject), j, 2);
                        j++;
                    }
                    
                    rootGrid.Children.Add(label);
                    rootGrid.Children.Add(textbox);
                    returnPayload.Add(propertyInfo.Name, textbox);
                }
                if (propertyInfo.PropertyType.Name == "Boolean")
                {
                    rootGrid.RowDefinitions.Add(CreateRowDefinition(30));

                    var Label = CreateTextBlock(propertyInfo.Name, j, 0);
                    rootGrid.Children.Add(Label);

                    var Textbox = CreateCheckBox(j, 2);
                    rootGrid.Children.Add(Textbox);
                    j++;
                }
            }
            rootGrid.RowDefinitions.Add(CreateRowDefinition(10));
            rootGrid.RowDefinitions.Add(CreateRowDefinition(30));
            var saveButton = CreateButton("Save Job", j + 1, 0);
            var exitButton = CreateButton("Exit", j + 1 , 1);
            saveButton.Click += new RoutedEventHandler(Button_Click);
            exitButton.Click += ExitBtton_Click;

            rootGrid.Children.Add(saveButton);
            rootGrid.Children.Add(exitButton);
            layoutGrid.Children.Add(rootGrid);
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void ExitBtton_Click(object sender, RoutedEventArgs e)
        {
            Saved = false;
            this.Close();
        }

        private CheckBox CreateCheckBox(int row, int column)
        {
            CheckBox cb = new CheckBox();
            cb.Margin = new Thickness(5);
            cb.Height = 22;
            cb.MinWidth = 50;

            Grid.SetColumn(cb, column);
            Grid.SetRow(cb, row);

            return cb;
        }

        private RowDefinition CreateRowDefinition(int minHeight)
        {
            RowDefinition RowDefinition = new RowDefinition();
            RowDefinition.Height = GridLength.Auto;
            RowDefinition.MinHeight = minHeight;
            return RowDefinition;
        }
        private TextBlock CreateTextBlock(string text, int row, int column)
        {
            string[] aa = BreakUpperCB(text);
            string prop = "";
            for (int i = 0; i < aa.Length; i++)
            {
                prop = prop + " " + aa[i];
            }
            prop = prop.Replace("_", " ");
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            prop = textInfo.ToTitleCase(prop);
            TextBlock tb = new TextBlock() { Text = prop };
            tb.MinWidth = 90;
            //tb.FontWeight = FontWeights.Bold;
            tb.FontSize = 14;
            tb.Margin = new Thickness(0,5,0,5);
            tb.Height = 20;
            Grid.SetColumnSpan(tb, 2);
            //var bc = new BrushConverter();
            //tb.Foreground = (Brush)bc.ConvertFrom("#FF2D72BC");
            Grid.SetColumn(tb, column);
            Grid.SetRow(tb, row);
            return tb;
        }
        private TextBox CreateTextBox(string text, int row, int column)
        {
            TextBox tb = new TextBox();
            tb.Text = text;
            tb.Margin = new Thickness(0,5,0,5);
            tb.Height = 20;
            Grid.SetColumn(tb, column);
            Grid.SetRow(tb, row);
            return tb;
        }

        // this will break the property text by upper
        public string[] BreakUpperCB(string sInput)
        {
            StringBuilder[] sReturn = new StringBuilder[1];
            sReturn[0] = new StringBuilder(sInput.Length);
            const string CUPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int iArrayCount = 0;
            for (int iIndex = 0; iIndex < sInput.Length; iIndex++)
            {
                string sChar = sInput.Substring(iIndex, 1); // get a char
                if ((CUPPER.Contains(sChar)) && (iIndex > 0))
                {
                    iArrayCount++;
                    System.Text.StringBuilder[] sTemp =
                       new System.Text.StringBuilder[iArrayCount + 1];
                    Array.Copy(sReturn, 0, sTemp, 0, iArrayCount);
                    sTemp[iArrayCount] = new StringBuilder(sInput.Length);
                    sReturn = sTemp;
                }
                sReturn[iArrayCount].Append(sChar);
            }
            string[] sReturnString = new string[iArrayCount + 1];
            for (int iIndex = 0; iIndex < sReturn.Length; iIndex++)
            {
                sReturnString[iIndex] = sReturn[iIndex].ToString();
            }
            return sReturnString;
        }

         void Button_Click(object sender, RoutedEventArgs e)
        {
            Saved = true;
            this.Close();
        }

        private Button CreateButton(string text, int row, int column)
        {
            Button tb = new Button()
            {
                Content = text,
                Margin = new Thickness(5)
            };
            Grid.SetColumn(tb, column);
            Grid.SetRow(tb, row);
            return tb;
        }
    }
}
