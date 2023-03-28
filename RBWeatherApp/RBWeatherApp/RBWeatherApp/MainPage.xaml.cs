using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace RBWeatherApp
{
    public partial class MainPage : ContentPage
    {
        
        // Constant variable for the API key
        const string API_KEY = "e88eb7476121c6205c85ee4793598478";
        // declare default units
        string units = "imperial";
        string unitSign = "°F";
        string speedUnits = "mph";
        public MainPage()
        {
            InitializeComponent();
            LoadWeatherDetails("Provo");
        }

        private void BtnShowWeatherDetails_Clicked(object sender, EventArgs e)
        {
            if (EntCity.Text != null)
            {
                PckCity.SelectedIndex = -1;
                LoadWeatherDetails(EntCity.Text);
            }
            else
            {
                DisplayAlert("Error", "Please enter a valid city", "Close");
            }
        }
        
        private void LoadWeatherDetails(string city)
        {
            // Instantiate the WebClient class
           
            using (WebClient wc = new WebClient())
            {
                // string to hold the JSON data
                string jsonData;
                try
                {

                    // Send a request to the OpenWeather API using our cityName and apiKey variables for the parameters. Return the units in Fahrenheit.
                    // Then, save the API JSON response text as a string and assign it to the jsonData variable.
                    jsonData = wc.DownloadString($"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={API_KEY}&units={units}");
                    // To parse the jsonData, we use the JObject class
                    JObject parsedJson = JObject.Parse(jsonData);
                    LblCity.Text = parsedJson["name"].ToString();
                    // Deserialize the json data into a custom class
                    AllWeatherData weatherData = JsonConvert.DeserializeObject<AllWeatherData>(jsonData);
                    // Using the AllWeatherDetails class, grab the current temperature and assign it to the current temp label
                    LblTemp.Text = Math.Round(double.Parse(weatherData.main.temp.ToString())) + unitSign; 
                    LblHighLow.Text = Math.Round(double.Parse(weatherData.main.temp_max.ToString())) + unitSign + "\n" + Math.Round(double.Parse(weatherData.main.temp_min.ToString())) + unitSign;
                    LblWeatherCondition.Text = weatherData.weather[0].main.ToString();
                    LblWind.Text = $"Wind speed: {parsedJson["wind"]["speed"]}" + " " + speedUnits;
                    // convert sunriseTime to human readable format
                    DateTime sunriseTime = DateTimeOffset.FromUnixTimeSeconds(weatherData.sys.sunrise).LocalDateTime;
                    DateTime sunsetTime = DateTimeOffset.FromUnixTimeSeconds(weatherData.sys.sunset).LocalDateTime;
                    // set sunrise and sunset time to labels formatted to include time only, not date
                    LblSunrise.Text = "Sunrise Time in MDT: \n" + sunriseTime.ToString("t");
                    LblSunset.Text = "Sunset Time in MDT: \n" + sunsetTime.ToString("t");
                    // set pressure and humidity
                    LblPressure.Text = "Pressure: \n" + weatherData.main.pressure +  " hPa";
                    LblHumidity.Text = "Humidity: \n" + weatherData.main.humidity + "%";
                    // set weather condition
                    LblCondition.Text = weatherData.weather[0].description;
                    if (units == "imperial")
                    {

                    
                    // change background color based on temperature, wow it's hot
                    if (weatherData.main.temp > 80)
                    {
                        WeatherGrid.BackgroundColor = Color.Red;
                    }
                    // frozen wasteland mode
                    else if (weatherData.main.temp < 40)
                    {
                        WeatherGrid.BackgroundColor = Color.LightBlue;
                    }
                    // default
                    else if (weatherData.main.temp <= 80 && weatherData.main.temp >= 40)
                    {
                        WeatherGrid.BackgroundColor = Color.Yellow;
                    }
                    // something went wrong, let's handle it anyway
                    else
                    {
                        WeatherGrid.BackgroundColor = Color.Gray;
                    }
                    }
                    if (units == "metric")
                    {


                        // change background color based on temperature, wow it's hot
                        if (weatherData.main.temp > 27)
                        {
                            WeatherGrid.BackgroundColor = Color.Red;
                        }
                        // frozen wasteland mode
                        else if (weatherData.main.temp < 4.4)
                        {
                            WeatherGrid.BackgroundColor = Color.LightBlue;
                        }
                        // default
                        else if (weatherData.main.temp <= 27 && weatherData.main.temp >= 4.4)
                        {
                            WeatherGrid.BackgroundColor = Color.Yellow;
                        }
                        // something went wrong, let's handle it anyway
                        else
                        {
                            WeatherGrid.BackgroundColor = Color.Gray;
                        }
                    }
                }
                catch (Exception ex)
                {
                    DisplayAlert("Oh no", $"{ex.Message}", "Close");
                }
            }
        }

        private void PckCity_SelectedIndexChanged(object sender, EventArgs e)
        {
            // make sure we avoid errors from triggering it when it is reset
            if (PckCity.SelectedIndex != -1)
            {
                EntCity.Text = "";
                LoadWeatherDetails(PckCity.SelectedItem.ToString());
            }
          
        }
        // flip units and unitSign
        private void BtnCelsiusFahrenheit_Clicked(object sender, EventArgs e)
        {
            if (units == "imperial")
            {
                units = "metric";
                unitSign = "°C";
                speedUnits = "m/s";
            }
            else {
                units = "imperial";
                   unitSign = "°F";
                speedUnits = "mph";
            }

            LoadWeatherDetails(LblCity.Text);


        }
    }
}
