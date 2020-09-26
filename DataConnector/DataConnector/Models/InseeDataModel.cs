using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DataConnector
{
    public partial class InseeData
    {
        [JsonProperty("?xml")]
        public Xml Xml { get; set; }

        [JsonProperty("Donnees")]
        public Donnees Donnees { get; set; }
    }

    public partial class Donnees
    {
        [JsonProperty("Croisement")]
        public Croisement Croisement { get; set; }

        [JsonProperty("Zone")]
        public DonneesZone Zone { get; set; }

        [JsonProperty("Variable")]
        public Variable[] Variable { get; set; }

        [JsonProperty("Cellule")]
        public Cellule[] Cellule { get; set; }
    }

    public partial class Cellule
    {
        [JsonProperty("Zone")]
        public CelluleZone Zone { get; set; }

        [JsonProperty("Mesure")]
        public Groupe Mesure { get; set; }

        [JsonProperty("Modalite")]
        public CelluleModalite[] Modalite { get; set; }

        [JsonProperty("Valeur")]
        public Decimal Valeur { get; set; }
    }

    public partial class Groupe
    {
        [JsonProperty("@code")]
        public string Code { get; set; }

        [JsonProperty("#text")]
        public string Text { get; set; }
    }

    public partial class CelluleModalite
    {
        [JsonProperty("@code")]
        public string Code { get; set; }

        [JsonProperty("@variable")]
        public VariableEnum Variable { get; set; }
    }

    public partial class CelluleZone
    {
        [JsonProperty("@codgeo")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Codgeo { get; set; }

        [JsonProperty("@nivgeo")]
        public Nivgeo Nivgeo { get; set; }
    }

    public partial class Croisement
    {
        [JsonProperty("Filtre")]
        public string Filtre { get; set; }

        [JsonProperty("Groupe")]
        public Groupe Groupe { get; set; }

        [JsonProperty("JeuDonnees")]
        public JeuDonnees JeuDonnees { get; set; }
    }

    public partial class JeuDonnees
    {
        [JsonProperty("@code")]
        public string Code { get; set; }

        [JsonProperty("Annee")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Annee { get; set; }

        [JsonProperty("Libelle")]
        public string Libelle { get; set; }

        [JsonProperty("Source")]
        public string Source { get; set; }
    }

    public partial class Variable
    {
        [JsonProperty("@code")]
        public VariableEnum Code { get; set; }

        [JsonProperty("Libelle")]
        public string Libelle { get; set; }

        [JsonProperty("Modalite")]
        public VariableModalite[] Modalite { get; set; }
    }

    public partial class VariableModalite
    {
        [JsonProperty("@code")]
        public String Code { get; set; }

        [JsonProperty("@variable")]
        public string Variable { get; set; }

        [JsonProperty("Libelle")]
        public string Libelle { get; set; }
    }

    public partial class DonneesZone
    {
        [JsonProperty("@codgeo")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Codgeo { get; set; }

        [JsonProperty("@nivgeo")]
        public Nivgeo Nivgeo { get; set; }

        [JsonProperty("Millesime")]
        public Millesime Millesime { get; set; }
    }

    public partial class Millesime
    {
        [JsonProperty("@annee")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Annee { get; set; }

        [JsonProperty("Nccenr")]
        public string Nccenr { get; set; }

        [JsonProperty("Tncc")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Tncc { get; set; }
    }

    public partial class Xml
    {
        [JsonProperty("@version")]
        public string Version { get; set; }

        [JsonProperty("@encoding")]
        public string Encoding { get; set; }

        [JsonProperty("@standalone")]
        public string Standalone { get; set; }
    }

    public enum VariableEnum
    {
        AGESCOL, ETUD, SEXE, ANEMR3, CS1_6, AGEACT6_B, AGEQ65, NA17, NA5, TACTR_2, TACTR99_2, CS1_8,
        AGEREMPLOI, INATC, IRANR_4, NAT13, PNAI12_A, CS2_24, IRANR, DIPL_R, EMPL , ILTDUU4, TRANS,
        ILTDUU, ILTR, ILTR2, CS2_18, IMMI, AGE4_B, AGEMEN8_B, AGEMEN8_C, IRANC, AGE5_55, IRANR_2,
        MATR, MOCO, AGEMEN8, AGEQ80_17, NPERC, AGE15_15_90, AGE3, AGE3_B, AGE4, TACTR_3,
        NATE49_B, PNAI48_A, DIPL, DIPL_15, TACTR, AGED100, AGED65, AGEFOR10, ILETUR, ILETUR2, AGEFORD, 
        AGEMEN8_A, COUPLE,  STAT_CONJ, AGEPYR10,  AGEQ100, AGEQ80_14, CATPR, STATR, TP, NA38, STOCD,
        TACTR99, TF4, TYPMR
    };

    public enum Nivgeo { Com };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                VariableEnumConverter.Singleton,
                NivgeoConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class VariableEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(VariableEnum) || t == typeof(VariableEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<VariableEnum>(reader);
            return value;

            /*
            switch (value)
            {
                case "ENTR_INDIVIDUELLE":
                    return VariableEnum.ENTR_INDIVIDUELLE;
                case "NA5_B":
                    return VariableEnum.NA5_B;
            }
            */

            throw new Exception("Cannot unmarshal type VariableEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (VariableEnum)untypedValue;
            serializer.Serialize(writer, value);

            /*
            switch (value)
            {
                case VariableEnum.ENTR_INDIVIDUELLE:
                    serializer.Serialize(writer, "ENTR_INDIVIDUELLE");
                    return;
                case VariableEnum.NA5_B:
                    serializer.Serialize(writer, "NA5_B");
                    return;
            }
            */            throw new Exception("Cannot marshal type VariableEnum");
        }

        public static readonly VariableEnumConverter Singleton = new VariableEnumConverter();
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class NivgeoConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Nivgeo) || t == typeof(Nivgeo?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "COM")
            {
                return Nivgeo.Com;
            }
            throw new Exception("Cannot unmarshal type Nivgeo");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Nivgeo)untypedValue;
            if (value == Nivgeo.Com)
            {
                serializer.Serialize(writer, "COM");
                return;
            }
            throw new Exception("Cannot marshal type Nivgeo");
        }

        public static readonly NivgeoConverter Singleton = new NivgeoConverter();
    }
}

