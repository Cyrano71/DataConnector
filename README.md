# DataConnector

Le programme récupére les données de l'Insee en se connectant à leur Api et les écrit
dans deux fichiers csv (un fichier pour les variables et un fichier pour la rawdata).

Pour lancer le programme il faut suivre la procédure suivante :
1) Ouvrir le fichier excel doc_RP.xls
2) Choisissez dans la feuille lib_tableau, le tableau qui vous intéresse en vous aidant du libellé associé à chaque tableau
3) Munissez-vous du nom du tableau et allez dans la feuille mesure_croisement
4) Dans cette feuille recherchez votre tableau (ctrl+f)
5) Récupérez le croisement et le jeu_donnees (GEO2019RP2016, RP2015, ..) correspondants à votre tableau
6) Allez sur le site de l'Api de l'Insee pour créer un compte https://api.insee.fr/catalogue/site/themes/wso2/subthemes/insee/pages/item-info.jag?name=DonneesLocales&version=V0.1&provider=insee
7) Récupérez votre access token sur votre compte Insee
8) Il faut ensuite unzip le dossier Release.zip
9) Dans le dossier Release, dans le App.config vous pouvez remplacer les champs Croisement, JeuDonnees, Modalite, AccessToken avec vos valeurs
10) Lancez l'executable DataConnector en mode administrateur.

Remarque1 : La rate limit pour l'api de l'Insee est de 30 requêtes par minute. Un rateLimiter est implémenté dans le programme. 
Comme on ne peut demander qu'une seule commune par requête le temps nécessaire pour récupérer les 36 000 communes de la France est aux alentours de 19 heures....

Remarque 2 : Deux exemples de résultats se trouvent dans le dossier InseeData : RawData_ILTR2-CS1_6-SEXE.csv et Variables_ILTR2-CS1_6-SEXE.csv
