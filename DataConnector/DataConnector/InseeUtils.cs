using DataConnector.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace DataConnector
{
    public static class InseeUtils
    {
        public static String GetJsonString(String xmlContent)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            string json = JsonConvert.SerializeXmlNode(doc);
            return json;
        }

        public static List<InseeCommune> GetListOfCommunes(String folder)
        {
            var filePath = Path.Combine(folder, "CommunesFrance.csv");
            var listOfCommunes = new List<InseeCommune>();
            using (var sr = new StreamReader(filePath))
            {
                // Read the stream as a string, and write the string to the console.
                var i = 1;
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var data = line.Split(';');
                    var commune = new InseeCommune()
                    {
                        Index = i,
                        CODGEO = data[0],
                        LIBGEO = data[1],
                    };

                    listOfCommunes.Add(commune);

                    i++;
                }
            }

            return listOfCommunes;
        }

        public static InseeData ReadFromJsonFile(String folder, String fileName)
        {
            var jsonFilePath = Path.Combine(folder, fileName + ".json");
            String json = File.ReadAllText(jsonFilePath);
            var data = JsonConvert.DeserializeObject<InseeData>(json);
            return data;
        }

        public static String? GetLastCodgeo(String folder, String fileName)
        {
            var filePath = Path.Combine(folder, fileName + ".csv");

            if (!File.Exists(filePath))
            {
                return null;
            }

            using (var sr = new StreamReader(filePath))
            {
                // Read the stream as a string, and write the string to the console.
                String line = null;
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();                 
                }

                if (line == null)
                    return null;

                var data = line.Split(';');
                
                return data[0];
            }
        }

        public static String GetParametersUrlFormat(InseeParametersRequest inseeParameters)
        {
            return inseeParameters.Croisement + "@" + inseeParameters.JeuDonnees + "/" + inseeParameters.Nivgeo + "-" + inseeParameters.Codgeo + "." + inseeParameters.Modalite;
        }

        public static String GetFileName(InseeTypeData typedata, InseeParametersRequest inseeParameters)
        {
            //var modalite = inseeParameters.Modalite.Replace('.', '_');
            //return $"{typedata}" + "_" + inseeParameters.Croisement + "@" + inseeParameters.JeuDonnees + "_" + inseeParameters.Nivgeo + "-" + inseeParameters.Codgeo + "_" + modalite;
            //+ "_" + inseeParameters.Codgeo
            return $"{typedata}" + "_" + inseeParameters.Croisement;
        }

        public static void WriteRawData(List<InseeData> listInseedata, String folder, String fileName, Boolean withHeader = true)
        {
            var fileMode = FileMode.Append;
            if (withHeader)
            {
                fileMode = FileMode.Create;
            }

            var filepath = Path.Combine(folder, fileName + ".csv");
            using (StreamWriter file = new StreamWriter(new FileStream(filepath,
                                  fileMode, FileAccess.Write), Encoding.GetEncoding("UTF-8")))
            {
                if (withHeader)
                {
                    var header = "CODGEO;LIBGEO;";
                    var firstDataNotNull = listInseedata.FirstOrDefault(x => x.Donnees.Cellule != null);
                    if (firstDataNotNull == null)
                    {
                        throw new Exception($"No entry in the list of data with non null cellule");
                    }

                    foreach (Cellule cellule in firstDataNotNull.Donnees.Cellule)
                    {
                        var length = cellule.Modalite.Length;
                        for (int i = 0; i < length; i++)
                        {
                            var modalite = cellule.Modalite[i];
                            header += $"{modalite.Variable}{modalite.Code}";

                            if (i != length - 1)
                            {
                                header += "_";
                            }
                        }

                        header += ";";
                    }

                    file.WriteLine(header);
                }        

                foreach (InseeData inseedata in listInseedata)
                {
                    var line = $"{inseedata.Donnees.Zone.Codgeo};{inseedata.Donnees.Zone.Millesime.Nccenr};";

                    if(inseedata.Donnees.Cellule != null)
                    {
                        foreach (Cellule cellule in inseedata.Donnees.Cellule)
                        {
                            line += $"{cellule.Valeur};";
                        }
                    }

                    file.WriteLine(line);
                }
            }
        }

        public static void WriteVariables(InseeData inseedata, String folder, String fileName)
        {
            var filepath = Path.Combine(folder, fileName + ".csv");
            using (StreamWriter file = new StreamWriter(new FileStream(filepath,
                                  FileMode.Create, FileAccess.Write), Encoding.GetEncoding("UTF-8")))
            {
                var header = $"Variables utilisées\n";
                file.WriteLine(header);

                foreach (Variable variable in inseedata.Donnees.Variable)
                {
                    var line = $"{variable.Code} : {variable.Libelle}\n";

                    foreach (VariableModalite modalite in variable.Modalite)
                    {
                        line += $"{modalite.Code} : {modalite.Libelle}\n";
                    }

                    file.WriteLine(line);
                }
            }
        }
    }
}
