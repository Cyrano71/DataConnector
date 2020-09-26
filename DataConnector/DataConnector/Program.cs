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

            /*    
            var croisement = "NA5_B-ENTR_INDIVIDUELLE";
            var jeu_donnees = "GEO2017REE2017";
            var nivgeo = "COM";
            var codgeo = "51108";
            var modalite = "all.all";

            var croisement = "SEXE-AGEACT6_B-TACTR_2";
            var jeu_donnees = "GEO2019RP2016";
            var nivgeo = "COM";
            var codgeo = "44109";
            var modalite = "all.all.all";
             */

            var croisement = "ILTR2-CS1_6-SEXE";
            var jeu_donnees = "GEO2019RP2016";
            var nivgeo = "COM";
            var modalite = "all.all.all";
            var inseeParameters = new InseeParametersRequest(jeu_donnees, croisement, modalite, nivgeo, "");

            var watch = new Stopwatch();
            var listCommunes = InseeUtils.GetListOfCommunes(dataFolder).Take(10).ToList();
            
            //rate limit 30 requests per minute
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
