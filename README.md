# DataConnector

Pour récupérer la donnée de l'Insee qui vous intéresse via leur Api il faut :
1) Ouvrir le fichier excel doc_RP.xls
2) Choisissez dans la feuille lib_tableau, le tableau qui vous intéresse en vous aidant du libellé associé à chaque tableau
3) Munissez-vous du nom du tableau et allez dans la feuille mesure_croisement
4) Dans cette feuille recherchez votre tableau (ctrl+f)
5) Récupérez le croisement et le jeu_donnees (GEO2019RP2016, RP2015, ..) correspondants à votre tableau
6) Allez sur le site de l'api de l'insee pour créer un compte https://api.insee.fr/catalogue/site/themes/wso2/subthemes/insee/pages/item-info.jag?name=DonneesLocales&version=V0.1&provider=insee
7) Récupérez votre access token sur votre compte insee
8) Dans App.config vous pouvez remplacer les champs Croisement,JeuDonnees, Modalite, AccessToken avec vos valeurs


remarque : La rate limit pour l'api de l'Insee est de 30 requêtes par minute. Un rateLimiter est implémenté dans le programme. 
Comme on ne peut demander qu'une seule commune par requête le temps nécessaire pour récupérer les 36 000 communes de la France est aux alentours de 19 heures....
