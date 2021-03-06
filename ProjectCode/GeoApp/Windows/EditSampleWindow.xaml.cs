using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GeoApp
{
    /// <summary>
    /// Interaction logic for EditSampleWindow.xaml
    /// </summary>
    public partial class EditSampleWindow : Window
    {
        private IController _controller;
        private Sample _sample;

        /// <summary>
        /// Initialize EditSampleWindow
        /// </summary>
        /// <param name="repository"></param>
        public EditSampleWindow(Sample sample, Controller controller)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            _sample = sample;
            _controller = controller;
            FillProperties();
        }

        /// <summary>
        /// Populate the sample properties with current data
        /// </summary>
        private void FillProperties()
        {
            Sample sample = _sample;
            SampleID.Text = sample.SampleId.ToString();
            Name.Text = sample.Name;
            SampleType.Text = sample.SampleType;
            GeologicAge.Text = sample.GeologicAge;
            LocationDescription.Text = sample.LocationDescription;
            City.Text = sample.City;
            State.Text = sample.State;
            Country.Text = sample.Country;
            Latitude.Text = sample.Latitude.ToString();
            Longitude.Text = sample.Longitude.ToString();
            if (sample.Image != null)
            {
                SampleImage.Source = ToImage(sample.Image);
            }
            else
            {
                NoImageText.Visibility = Visibility.Visible;
            }
        }

        private BitmapImage ToImage(byte[] array)
        {
            using (var ms = new MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }

        /// <summary>
        /// Submit the updated sample
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit(object sender, RoutedEventArgs e)
        {
            List<string> sampleInfo = new List<string>();
            sampleInfo.Add(_sample.DbId.ToString());
            sampleInfo.Add(SampleID.Text);
            sampleInfo.Add(Name.Text);
            sampleInfo.Add(SampleType.Text);
            sampleInfo.Add(GeologicAge.Text);
            sampleInfo.Add(LocationDescription.Text);
            sampleInfo.Add(City.Text);
            sampleInfo.Add(State.Text);
            sampleInfo.Add(Country.Text);
            sampleInfo.Add(Latitude.Text);
            sampleInfo.Add(Longitude.Text);

            //image conversion
            if (!string.IsNullOrEmpty(PathSampleImage.Text))
            {
                FileStream fs;
                BinaryReader br;
                string fileName = PathSampleImage.Text;
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);
                _sample.Image = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
            }

            try
            {
                if (_controller.UpdateSample(sampleInfo, _sample.Image))
                {

                    int itemLocation = MainWindow.Samples.IndexOf(_sample);
                    MainWindow.Samples.RemoveAt(itemLocation);
                    int.TryParse(sampleInfo[1], out Int32 newSampleId);
                    _sample.SampleId = newSampleId;
                    _sample.Name = sampleInfo[2];
                    _sample.SampleType = sampleInfo[3];
                    _sample.GeologicAge = sampleInfo[4];
                    _sample.LocationDescription = sampleInfo[5];
                    _sample.City = sampleInfo[6];
                    _sample.State = sampleInfo[7];
                    _sample.Country = sampleInfo[8];
                    double.TryParse(sampleInfo[9], out Double newLatitude);
                    _sample.Latitude = newLatitude;
                    double.TryParse(sampleInfo[10], out Double newLongitude);
                    _sample.Longitude = newLongitude;

                    MainWindow.Samples.Insert(itemLocation, _sample);

                    SuccessfulEditWindow confirmation = new();
                    confirmation.Show();
                    Close();
                }
                else
                {
                    UnsuccessfulEditWindow error = new();
                    error.setErrorMessage(_controller.getErrorMessage());
                    error.Show();
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 0)
                {
                    MessageBox.Show("Couldn't connect to the server while updating entry. Please" +
                        " try again or contact your administrator.", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("There was a problem in the database while updating entry. " +
                        "Please try again, check your connection, or contact your administrator.", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// Edit an image
        /// </summary>
        /// @author: Demetrios Green, Ben Pink, Clayton Rath, David Vegter
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditImage(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Image files | *.jpg";
                openFileDialog1.ShowDialog();
                if (!string.IsNullOrEmpty(openFileDialog1.FileName))
                {
                    PathSampleImage.Text = openFileDialog1.FileName;
                    SampleImage.Source = new BitmapImage(new Uri(openFileDialog1.FileName));
                    NoImageText.Visibility = Visibility.Hidden;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Cancel edit, return to main view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel(object sender, RoutedEventArgs e) => this.Close();

    }
}
