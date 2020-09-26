using System;

namespace DataConnector.Models
{
    public class InseeParametersRequest
    {
        public String JeuDonnees { get; }
        public String Croisement { get; }
        public String Modalite { get; }
        public String Nivgeo { get; }
        public String Codgeo { get; set; }

        public InseeParametersRequest(String jeu_donnees, String croisement, String modalite, String nivgeo, String codgeo)
        {
            JeuDonnees = jeu_donnees;
            Croisement = croisement;
            Modalite = modalite;
            Nivgeo = nivgeo;
            Codgeo = codgeo;
        }

    }
}
