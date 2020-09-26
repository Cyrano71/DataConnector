using DataConnector.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataConnector
{
    class Program
    {
        static void Main(string[] args)
        {
            var accessToken = ConfigurationManager.AppSettings.Get("AccessToken");

            if(accessToken == null)
            {
                throw new Exception("Invalid AccessToken");
            }

            var client = new HttpInseeConnector(accessToken);

            var baseUrl = ConfigurationManager.AppSettings.Get("InseeBaseUrl");

            if(baseUrl == null)
            {
                throw new Exception("Invalid BaseUrl");
            }

            var dataFolder = ConfigurationManager.AppSettings.Get("DataFolder");

            if (dataFolder == null)
            {
                throw new Exception("Invalid DataFolder");
            }

            var inseeParameters = new InseeParametersRequest(ConfigurationManager.AppSettings.Get("JeuDonnees"),
                 ConfigurationManager.AppSettings.Get("Croisement"), ConfigurationManager.AppSettings.Get("Modalite"), ConfigurationManager.AppSettings.Get("Nivgeo"), "");

            var nbCommunes = ConfigurationManager.AppSettings.Get("NbCommunes");

            List<InseeCommune> listCommunes;
            if (nbCommunes == "*")
            {
                listCommunes = InseeUtils.GetListOfCommunes(dataFolder);
            }
            else
            {
                var nb = Convert.ToInt32(nbCommunes);
                listCommunes = InseeUtils.GetListOfCommunes(dataFolder).Take(nb).ToList();
            }

            //rate limit 30 requests per minute
            var watch = new Stopwatch();

            var nbSecondToWait = 2;

            var listData = new List<InseeData>();
            foreach (InseeCommune commune in listCommunes)
            {
                watch.Start();

                var codgeo = commune.CODGEO;
                inseeParameters.Codgeo = codgeo;

                var response = client.Get(baseUrl, InseeUtils.GetParametersUrlFormat(inseeParameters));

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Error Response StatusCode : {response.StatusCode}");
                }

                var xmlContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var json = InseeUtils.GetJsonString(xmlContent);
                var data = JsonConvert.DeserializeObject<InseeData>(json);
                listData.Add(data);
                
                watch.Stop();
                if (watch.ElapsedMilliseconds < nbSecondToWait * 1000)
                {
                    Task.Delay((int)(nbSecondToWait * 1000 - watch.ElapsedMilliseconds)).GetAwaiter().GetResult();
                }
                watch.Reset();
            }

            var rawFileName = InseeUtils.GetFileName(InseeTypeData.RawData, inseeParameters);
            InseeUtils.WriteRawData(listData, dataFolder, rawFileName);
            InseeUtils.WriteVariables(listData[0], dataFolder, InseeUtils.GetFileName(InseeTypeData.Variables, inseeParameters));

        }
    }
}
