# Bulbapedia scraper [![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)
Scraper to get data from [Bulbapedia](https://bulbapedia.bulbagarden.net/) and convert to a graph database.

The script generated in this project is in the project [pokemon-graph](https://github.com/IgorRozani/pokemon-graph).

## Pages utilized
This scraper get the data from the pages:
* [Pokémon list by number](https://bulbapedia.bulbagarden.net/w/index.php?title=List_of_Pok%C3%A9mon_by_National_Pok%C3%A9dex_number) - to read the pokémon and regional variant data;
* [Evolution list](https://bulbapedia.bulbagarden.net/w/index.php?title=List_of_Pok%C3%A9mon_by_evolution_family) - to read the evolutions and the Unown forms;
* [Forms list](https://bulbapedia.bulbagarden.net/w/index.php?title=List_of_Pok%C3%A9mon_with_form_differences) - to read the forms from the pokémons;
* [Mega evolution list](https://bulbapedia.bulbagarden.net/w/index.php?title=Mega_Evolution) - to read the mega evolutions.

## Project architecture
The scraper is a console application in C# and .Net Core.

### Configuration
To run the project it's necessary two sections in the appsetting.json: bulbapediaConfiguration and fileExportConfiguration.

#### bulbapediaConfiguration
This configuration has the bulbapedia urls and paths, necessary to read the data. It's mapped in the class BulbapediaConfiguration, inside the configurations.
The properties in the configurations are:
- baseUrl: to inform the base url from all the paths from bulbapedia;
- baseImageUrl: the base image url from the pictures in the site;
- pokemonListPath: the path to the pokémons list;
- evolutionListPath: the path to the pokémons evolution list;
- megaEvolutionListPath: the path to the pokémons mega evolution list;
- formsListPath: the path to the pokémons forms list.

#### fileExportConfiguration
This configuration has the property fileFullPath, that is used the informe the file path and file name to the script generated. It's mapped in the class FileExportConfiguration, inside the configurations.

#### Example:
```
{
  "bulbapediaConfiguration": {
    "baseUrl": "https://bulbapedia.bulbagarden.net/w/index.php?title=",
    "baseImageUrl": "https://",
    "pokemonListPath": "List_of_Pok%C3%A9mon_by_National_Pok%C3%A9dex_number",
    "evolutionListPath": "List_of_Pok%C3%A9mon_by_evolution_family",
    "megaEvolutionListPath": "Mega_Evolution",
    "formsListPath": "List_of_Pok%C3%A9mon_with_form_differences"
  },
  "fileExportConfiguration": {
    "fileFullPath": "C:\\temp\\pokemon.cypher"
  }
}
```

### Architecture
The project has three main folders, that sepate the context from the project: Configurations, Models and Services.

#### Configurations
This folder has the map from the configurations, utilized to read the configurations from the file exporation and bulbapedia urls, as explained in the last section.

#### Models
This folder contains the models from the domain, it's mapped all the data from the database. The main class is Pokemon, inside of it has all lists of evolutions, mega evolutions, types, forms.
Inside this folder has a subfolder named Comparers, inside of it has the TypeEqualityComparer utilized in the project to compare the types.

#### Services
This folder is contains the logic from the project, separeted in three contexts: FileExport, Scrapers and ScriptGenerator.

#### FileExport
This service is responsible for exporting the script to a file, in the place configured.

#### Scrapers
This service is responsible for reading the data from the bulbapedia and convert it to the model objects. It has one scraper for each path in the configuration and each scraper has a specific logic for the page, because the lists share the same layout but has different structures. The scrapers are:
- EvolutionList: responsible for read the evolutions and the Unown forms;
- FormsList: responsible for read the forms from the pokémons;
- MegaEvolutionList: responsible for read the mega evolutions;
- PokemonList: responsible for read the pokémons and the regional variant (Alola region).

The PokemonList scraper needs to be runned first, it's the one that create the pokemon list, utilized by the others scrapers.

#### ScriptGenerator
This service is responsible for converting the pokémon list to a cypher script, it passes by all pokémon properties and create the nodes and relationships from the script.

### Upcoming features
Create the scraper and the structure to read the data from the [pokémon moves](https://bulbapedia.bulbagarden.net/wiki/List_of_moves).
