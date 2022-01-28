# Génération procédurale de Bordeaux sur Unity

Projet semestriel dans le cadre de l'option IA commune à l'ENSEIRB et l'ENSC.

L'objectif est de générer une ville ressemblante à Bordeaux avec des méthodes de génération procédurales. Ces méthodes peuvent notamment être utilisées pour la création de map dans les jeux vidéos. Nous utilisons le logiciel [Unity](https://unity.com/fr) pour générer la ville.

## Membres du projet
- Victor Leroy
- Aymeric Bled

## Etapes du projet

### Génération de routes

Nous avons repris les grands axes de Bordeaux auxquels nous avons ajouté des routes aléatoires générées à l'aide de la méthode de Voronoï.

### Génération de bâtiments

Nous avons placé les bâtiments à côté des routes. Les dimensions des bâtiments (hauteur, largeur, profondeur) sont choisies aléatoirement.

### Ajout de trafic

A l'aide des [NavMesh](https://docs.unity3d.com/ScriptReference/AI.NavMesh.html) de Unity, nous avons défini les routes comme surfaces navigables pour nos voitures.

### Génération de façades

Nous avons utilisé le réseau de neurone [pix2pix](https://www.tensorflow.org/tutorials/generative/pix2pix) qui est un réseau générateur de façacades (GAN). Il est en effet possible avec ce réseau de construire des façades à partir d'images segmentées qui définissent la position d'objets comme des portes et des fenêtres.

## Démo

### Vue de dessus

<img src="./img/vue de dessus.PNG" width="1000">

### Vue des bâtiments

<img src="./img/vue des bâtiments.gif" width="1000">

### Vue depuis une voiture

<img src="./img/vue depuis une voiture.gif" width="1000">