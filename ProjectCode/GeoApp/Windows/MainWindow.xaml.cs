using GeoApp.Windows;
using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GeoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ObservableCollection<Sample> Samples { get; set; }
        public static ObservableCollection<Issue> Issues { get; set; }
        public string _searchText;
        private Controller _controller;
        private Repository _repo;
        private MySqlConnection _conn;
        private string _connectionStringToDB = ConfigurationManager.ConnectionStrings["TeamProjectDB"].ConnectionString;

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            try
            {
                _conn = new MySqlConnection(_connectionStringToDB);
                _repo = new(_conn);
                _controller = new Controller(_repo);

                Samples = LoadCollectionData();
                SampleTable.ItemsSource = Samples;
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 0)
                {
                    MessageBox.Show("Couldn't connect to the server, please restart, and " +
                        "try again or contact your administrator.", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("There was a problem in the database, please restart, check your connection and " +
                        "try again or contact your administrator.", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }

        /// <summary>
        /// Method for loading initial table data of all samples
        /// @author: Demetrios Green, Ben Pink, Clayton Rath, David Vegter
        /// </summary>
        /// <returns>A list of all samples</returns>
        private ObservableCollection<Sample> LoadCollectionData()
        {
            ObservableCollection<Sample> collection = new();

            try
            {
                collection = new ObservableCollection<Sample>(_controller.GetAllSamples());
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 0)
                {
                    MessageBox.Show("Couldn't connect to the server. Please restart and try again " +
                        "or contact your administrator.", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("There was a problem in the database, please restart again, check your connection, " +
                        "or contact your administrator.", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            return collection;

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddSampleWindow addEntryWindow = new(_controller);
            addEntryWindow.Show();

        }

        private void ReportIssueButton_Click(object sender, RoutedEventArgs e)
        {
            ReportIssueWindow reportIssueWindow = new(_controller);
            reportIssueWindow.Show();
        }

        private void ViewIssuesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewIssuesWindow issuesWindow = new(_controller);
                issuesWindow.Show();
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 0)
                {
                    MessageBox.Show("Couldn't connect to the server. Please try again to view issues " +
                        "or contact your administrator.", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("There was a problem in the database, please check your connection, try again to view issues " +
                        "or contact your administrator.", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// Method for performing a search using a ListCollectionView filter.
        /// </summary>
        /// <returns>A collection of the filtered list</returns>
        private ListCollectionView PerformSearch()
        {
            ListCollectionView collectionView = new(Samples);
            collectionView.Filter = (x) =>
            {
                Sample sample = x as Sample;
                string normalizedSearchTerm = _searchText.ToLower();
                return sample.SampleId.ToString().ToLower().Contains(normalizedSearchTerm) ||
                sample.Name.ToLower().Contains(normalizedSearchTerm) ||
                sample.SampleType.ToLower().Contains(normalizedSearchTerm) ||
                sample.LocationDescription.ToLower().Contains(normalizedSearchTerm) ||
                sample.GeologicAge.ToLower().Contains(normalizedSearchTerm);
            };

            return collectionView;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (_searchText == "Search")
            {
                _searchText = string.Empty;
                SearchBox.Text = string.Empty;
            }

            SampleTable.ItemsSource = PerformSearch();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SampleTable.ItemsSource = PerformSearch();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchText = SearchBox.Text;
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text.Equals("Search"))
            {
                SearchBox.Text = string.Empty;
            }
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            var sampleData = (Button)e.OriginalSource;
            var sample = (Sample)sampleData.DataContext;
            SampleViewWindow window = new(sample);
            window.Show();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var sampleData = (Button)e.OriginalSource;
            var sample = (Sample)sampleData.DataContext;
            EditSampleWindow window = new(sample, _controller);
            window.Show();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var sampleData = (Button)e.OriginalSource;
            var sample = (Sample)sampleData.DataContext;
            DeleteSampleConfirmationWindow window = new(sample, _controller);
            window.Owner = this;
            window.Show();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "Search";
            SampleTable.ItemsSource = Samples;
        }
    }
}
