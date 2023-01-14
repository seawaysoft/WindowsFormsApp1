using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string url = "https://vpic.nhtsa.dot.gov/api/vehicles/GetAllManufacturers?ManufacturerType=Intermediate";
            //string url = "https://vpic.nhtsa.dot.gov/api/vehicles/GetModelsForMakeId/440?format=json";
            //string url = "https://vpic.nhtsa.dot.gov/api/vehicles/GetModelsForMake/" + make + "?format=json";

            
            string make = cboMake.Text.Trim().ToLower();  
            string url = "https://vpic.nhtsa.dot.gov/api/vehicles/GetModelsForMake/honda?format=json";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var tmp = client.GetAsync(url).Result;
                if (tmp.IsSuccessStatusCode)
                {
                    var readTask = tmp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var rawResponse = readTask.GetAwaiter().GetResult();

                    string carModels = rawResponse.ToString();
                    fillCarModelList(carModels);
                }
            }
            catch (Exception err)
            {
                // error handling
            }
        }

        private void fillCarModelList(string carModelStr)
        {
            JObject obj = JObject.Parse(carModelStr);

            int count = 0; 
            if (obj["Count"] != null)
                count = obj["Count"].Value<int>();

            Console.WriteLine("Count:" + count);

            string searchCriteria = "";
            if (obj["SearchCriteria"] != null)
                searchCriteria = obj["SearchCriteria"].Value<string>();

            Console.WriteLine("searchCriteria:" + searchCriteria);

            //List<Model> objCarModels = JsonConvert.DeserializeObject<List<Model>>();
            //CarModels objCarModels = Newtonsoft.Json.JsonConvert.DeserializeObject<CarModels>(carModelStr);    //ReadToObject(carModelStr);
            Model[] models = null;  //models = (string) obj["Results"][0]["Model_Name"];
            if (obj["Results"] != null)
                models = obj["Results"].ToObject<List<Model>>().ToArray();

            lstModels.Items.Clear();
            if ((models != null) && (models.Length > 0))
            {
                for (int i = 0; i < models.Length; i++)
                {
                    lstModels.Items.Add(models[i].Model_ID + " " + models[i].Model_Name);
                }
            }
        }

        private void fillCarModelList_MS(string carModelStr)
        {
            CarModels objCarModels = ReadToObject(carModelStr);
            Model[] models = objCarModels.Result;  

            lstModels.Items.Clear();
            if ((models != null) && (models.Length > 0))
            {
                for (int i = 0; i < models.Length; i++)
                {
                    lstModels.Items.Add(models[i].ToString());
                }
            }
        }

        // Deserialize a JSON stream to a User object.
        public static CarModels ReadToObject(string jsonStr)
        {
            //int startPosition = jsonStr.IndexOf("[");
            //int endPosition = jsonStr.IndexOf("]");
                        //string modelStr = jsonStr.Split('[')
            
            var models = new CarModels();
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonStr));
            var ser = new DataContractJsonSerializer(models.GetType());
            models = ser.ReadObject(ms) as CarModels;
            ms.Close();
            return models;
        }

        [DataContract]
        public class CarModels
        {
            [DataMember(Order = 0)]
            public int Count;

            [DataMember(Order = 1)]
            public string Message;

            [DataMember(Order = 2)]
            public string SearchCriteria;

            [DataMember(Order = 3)]   //Name = "Results"
            public Model[] Result; //{ get; protected set; }
        }

        [DataContract]
        public class Model
        {
            //{\"Make_ID\":474,\"Make_Name\":\"HONDA\",\"Model_ID\":1861,\"Model_Name\":\"Accord\"}
            [DataMember(Order = 0)]
            public int Make_ID;

            [DataMember(Order = 1)]
            public string Make_Name;

            [DataMember(Order = 2)]
            public string Model_ID;

            [DataMember(Order = 3)]
            public string Model_Name;   //{ get; protected set; }
        }
    }
}

