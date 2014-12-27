using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Windows.Input;
using ResolutionsTracker.Commands;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ResolutionsTracker.DataModel
{
    public class Resolution : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ObservableCollection<DateTime> Dates { get; set; }

        // Tells serializer to ignore this data member
        [IgnoreDataMember]
        public ICommand CompletedCommand { get; set; }

        public Resolution()
        {
            CompletedCommand = new CompletedButtonClick();
            Dates = new ObservableCollection<DateTime>();
        }

        public void AddDate()
        {
            Dates.Add(DateTime.Today);
            NotifyPropertyChanged("Dates"); 
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


    }

    public class DataSource
    {
        // Observable collection for Resolutions used throughout application
        private ObservableCollection<Resolution> _resolutions;

        // Data Source JSON file
        const string fileName = "resolutions.json";

        public DataSource()
        {
            _resolutions = new ObservableCollection<Resolution>();
        }

        // Gets the _resolutions observable collection 
        public async Task<ObservableCollection<Resolution>> GetResolutions()
        {
            await ensureDataLoaded();
            return _resolutions;
        }

        // Makes sure data is loaded
        private async Task ensureDataLoaded()
        {
            if (_resolutions.Count == 0)
                await getResolutionDataAsync();

            return;
        }

        // Gets resolutions data
        private async Task getResolutionDataAsync()
        {
            // If resolutions is already in memory we dont need to run this method
            if (_resolutions.Count != 0)
                return;

            // If resolutions is not in memory we will create a JSON serializer 
            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Resolution>));

            try
            {
                // Open file and read data into the stream
                using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(fileName))
                {
                    // serialize stream into observable collection of resolutions
                    _resolutions = (ObservableCollection<Resolution>)jsonSerializer.ReadObject(stream);
                }
            }
            catch
            {
                // If there was no file - create a file
                _resolutions = new ObservableCollection<Resolution>();
            }
        }

        // Method for adding resolutions/ creating new instances of resolutions
        public async void AddResolution(string name, string description)
        {
            var resolution = new Resolution();
            resolution.Name = name;
            resolution.Description = description;
            resolution.Dates = new ObservableCollection<DateTime>();

            // Saves new resolution to Observable collection and to disk
            _resolutions.Add(resolution);
            await saveResolutionDataAsync();
        }

        // Saves resolutions data
        private async Task saveResolutionDataAsync()
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof(ObservableCollection<Resolution>));

            // Overides existing file if it exists. 
            // "Using" because we want to close up the file and links to file when out of scope of using statement.
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.ReplaceExisting))
            {
                jsonSerializer.WriteObject(stream, _resolutions);
            }
        }

        public async void CompleteResolutionToday(Resolution resolution)
        {
            // gets index of resolution paramater from observable dictionary
            int index = _resolutions.IndexOf(resolution);

            // calls the AddDate() method to add todays date to the Dates observabele dictionary for the current object
            _resolutions[index].AddDate();

            // saves resolutions to disk
            await saveResolutionDataAsync();
        }


    }

}
