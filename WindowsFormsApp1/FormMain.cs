using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows.Forms;

/*  
 *  A demo to leverage a public API: 
 *  
 *      Get the car models of the defined manufacturer
 * 
 *  Gary Xia   01/13/2023
 */

namespace WindowsFormsApp1
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            cboMake.SelectedIndex = 0;
        }

        //public API to get car models
        const string API_URL = "https://vpic.nhtsa.dot.gov/api/vehicles/GetModelsForMake/make?format=json";
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                RefreshModelList();
            } catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error");
            }
        }
        private void RefreshModelList()
        {
            string make = cboMake.Text.Trim().ToLower();
            string url = API_URL.Replace("make", make);

            //get data from web 
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var tmp = client.GetAsync(url).Result;
            if (tmp.IsSuccessStatusCode)
            {
                var readTask = tmp.Content.ReadAsStringAsync().ConfigureAwait(false);
                var rawResponse = readTask.GetAwaiter().GetResult();

                //fillCarModelList with web result 
                string webResult = rawResponse.ToString();
                fillCarModelList(webResult);
            }
        }

        private void fillCarModelList(string carModelStr)
        {
            //carModelStr is JSON string of car make and models 
            JObject obj = JObject.Parse(carModelStr);
            Model[] models = null;
            if (obj["Results"] != null)
            {
                models = obj["Results"].ToObject<List<Model>>().ToArray();
            }

            lstModels.Items.Clear();
            if ((models != null) && (models.Length > 0))
            {
                for (int i = 0; i < models.Length; i++)
                {
                    lstModels.Items.Add(models[i].Model_ID + " " + models[i].Model_Name);
                }
            }
        }
    }

    //car model data structure of the public API 
    public class Model
    {
        public int Make_ID;
        public string Make_Name;
        public string Model_ID;
        public string Model_Name;
    }
}

