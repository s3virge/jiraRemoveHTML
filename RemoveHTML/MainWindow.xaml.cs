using MySQLibrary;
using MySQLlibrary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RemoveHTML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            dataGrid.ItemsSource = DataBase.SelectIssueDescription();
            LongTextConverter LongTextConverter = new LongTextConverter();
            dataGrid.AutoGeneratingColumn += (ss, ee) =>
            {
                DataGridTextColumn column = ee.Column as DataGridTextColumn;
                column.Binding = new Binding(ee.PropertyName) { Converter = LongTextConverter };
            };
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = dataGrid?.SelectedItem as DescriptionModel;
            if (selectedItem == null) { return; }

            tbSelected.Text = selectedItem.Description;
            tbConverted.Text = Converter.ToJiraMarkup(tbSelected.Text);
            //tbConverted.Text = "";
        }

        private void ReadDataBaseData_click(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = DataBase.SelectIssueDescription();
        }

        private void ConvertSelected_click(object sender, RoutedEventArgs e)
        {
            tbConverted.Text = Converter.ToJiraMarkup(tbSelected.Text);
        }
    }
}