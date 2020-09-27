using DataConnector.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DataConnector
{
    class Program
    {
        static void Main(string[] args)
        {
            var logFolder = ConfigurationManager.AppSettings.Get("LogFolder");

            if (logFolder == null)
            {
                throw new Exception("Invalid LogFolder");
            }

            var logService = new SimpleLogService(logFolder);

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

            var jeuDonnees = ConfigurationManager.AppSettings.Get("JeuDonnees");

            if (jeuDonnees == null)
            {
                throw new Exception("Invalid jeuDonnees");
            }

            var croisement = ConfigurationManager.AppSettings.Get("Croisement");

            if (croisement == null)
            {
                throw new Exception("Invalid croisement");
            }

            var modalite = ConfigurationManager.AppSettings.Get("Modalite");

            if (modalite == null)
            {
                throw new Exception("Invalid modalite");
            }

            var nivgeo = ConfigurationManager.AppSettings.Get("Nivgeo");

            if (nivgeo == null)
            {
                throw new Exception("Invalid nivgeo");
            }

            var inseeParameters = new InseeParametersRequest(jeuDonnees, croisement, modalite, nivgeo, "");
            var rawFileName = InseeUtils.GetFileName(InseeTypeData.RawData, inseeParameters);

            Boolean smartContinue = true;
            List<InseeCommune> listCommunes = InseeUtils.GetListOfCommunes(dataFolder);
            var lastCodgeoProcessed = InseeUtils.GetLastCodgeo(dataFolder, rawFileName);
            if (lastCodgeoProcessed != null)
            {
                var lastCommuneProcessed = listCommunes.FirstOrDefault(x => x.CODGEO == lastCodgeoProcessed);
                if(lastCommuneProcessed == null)
                {
                    smartContinue = false;
                }
                else
                {
                    listCommunes = listCommunes.Where(x => x.Index > lastCommuneProcessed.Index).ToList();
                }
            }
            else
            {
                smartContinue = false;
            }

            //rate limit 30 requests per minute
            var watch = new Stopwatch();
            var nbSecondToWait = 2;

            //Writing settings
            var firstWrite = true;

            var limit = ConfigurationManager.AppSettings.Get("limitNbDataItems");
            var limitNbDataItems = 50;
            if (limit != null)
            {
                limitNbDataItems = Convert.ToInt32(limit);
            }
         
            var listData = new List<InseeData>();
            foreach (InseeCommune commune in listCommunes)
            {
                watch.Start();

                var codgeo = commune.CODGEO;
                inseeParameters.Codgeo = codgeo;

                var response = client.Get(baseUrl, InseeUtils.GetParametersUrlFormat(inseeParameters));

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    logService.Error(response.ToString());
                    return;
                }

                var xmlContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var json = InseeUtils.GetJsonString(xmlContent);
                var data = JsonConvert.DeserializeObject<InseeData>(json);
                listData.Add(data);
                
                if(listData.Count >= limitNbDataItems)
                {
                    if (firstWrite && !smartContinue)
                    {
                        InseeUtils.WriteRawData(listData, dataFolder, rawFileName);
                        InseeUtils.WriteVariables(listData[0], dataFolder, InseeUtils.GetFileName(InseeTypeData.Variables, inseeParameters));
                        firstWrite = false;
                    }
                    else
                    {
                        InseeUtils.WriteRawData(listData, dataFolder, rawFileName, false);
                    }

                    listData.Clear();
                }

                watch.Stop();
                if (watch.ElapsedMilliseconds < nbSecondToWait * 1000)
                {
                    Task.Delay((int)(nbSecondToWait * 1000 - watch.ElapsedMilliseconds)).GetAwaiter().GetResult();
                }
                watch.Reset();
            }

            InseeUtils.WriteRawData(listData, dataFolder, rawFileName, false);

        }
    }
}
