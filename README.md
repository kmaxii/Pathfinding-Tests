# Pathfinding-test Repository

Welcome to the Pathfinding-test repository, which includes the code for mine and Demian Skoglövs bachelor thesis titled "Opportunities for improvement in Dijkstra's pathfinding algorithm." This study, written in Swedish, compares the efficiency of various graph search algorithms in a two-dimensional grid-based environment. Below, you'll find a brief summary of the thesis, along with visualizations and descriptions of the pathfinding algorithms explored.

## Thesis Summary

This study compares the efficiency of the following graph search algorithms:

- Dijkstra
- Bidirectional Dijkstra
- A*
- Bidirectional A*
- Jump Point Search (JPS)

By analyzing their performance based on execution time and the number of expanded nodes, the study aims to identify the efficiency improvements these algorithms offer. The results show significant performance differences between the algorithms, with JPS being 7235% faster than Dijkstra on a 1000x1000 grid map. This highlights the relevance of these improvements in gaming, where milliseconds matter. The bidirectional versions of Dijkstra and A* are also more efficient than their unidirectional counterparts, improving execution time by approximately 50%, validating the value of these improvements. Future research may include exploring additional algorithms and improvements, as well as their application and testing in real-time scenarios to further validate and develop these results.

## Visualizations

Below are visual representations of the five pathfinding algorithms used in the study. The first image shows all five algorithms at once in the order mentioned above, and the subsequent GIFs illustrate how each algorithm explores the area.

### All Pathfinding Algorithms

<img src="https://cdn.discordapp.com/attachments/885157374036439042/1214576429321424956/kart_visaulisering_50x50.png?ex=66801672&is=667ec4f2&hm=13919fdc626b7b00937b6377bd44f17d4a975b40b71ed0e3f3332a46a7a3d978&" alt="All Pathfinding Algorithms" width="100%">

### Dijkstra's Algorithm

<img src="https://cdn.discordapp.com/attachments/885157374036439042/1217476005178904616/Dijkstra-ezgif.com-video-to-gif-converter.gif?ex=668016e3&is=667ec563&hm=7fe1b4d2451cc996c502d9dd13d7a67b22d11b4bc6ae6c734c0a532a181a7020&f" alt="Dijkstra's Algorithm" width="300">

### Bidirectional Dijkstra's Algorithm

<img src="https://cdn.discordapp.com/attachments/885157374036439042/1217476674375909406/BiDirDijkstra-ezgif.com-video-to-gif-converter.gif?ex=66801782&is=667ec602&hm=956f6f81f10211e8beece5e30bc670c695916fd016fbb2ea05e160d8b4d8c816&" alt="Bidirectional Dijkstra's Algorithm" width="300">

### A* Algorithm

<img src="https://cdn.discordapp.com/attachments/885157374036439042/1217476673407029369/Astar-ezgif.com-video-to-gif-converter.gif?ex=66801782&is=667ec602&hm=6494327b62ee3b82a4b2f82685c55876fe6a7ecaa1e9408e259077498011a5e8&" alt="A* Algorithm" width="300">

### Bidirectional A* Algorithm

<img src="https://cdn.discordapp.com/attachments/885157374036439042/1217476674799538186/BidirAstar-ezgif.com-video-to-gif-converter.gif?ex=66801782&is=667ec602&hm=3bf6148b5c95ce25a53a7348fbca95aabc394e84f3072627dbc92445a3ae0722&" alt="Bidirectional A* Algorithm" width="300">

### Jump Point Search (JPS) Algorithm

<img src="https://cdn.discordapp.com/attachments/885157374036439042/1217476673935769730/jPS-ezgif.com-video-to-gif-converter.gif?ex=66801782&is=667ec602&hm=9a6a6ad12558a8da0ad3eb38de4c77d237ed663b402115196e7ea86ffaa811e8&" alt="Jump Point Search (JPS) Algorithm" width="300">

## Repository Contents

This repository contains the following:

- Source code for implementing and testing the pathfinding algorithms.
- Scripts for generating and comparing execution times and node expansions.
- Visualization tools for displaying the pathfinding process.

## Getting Started

To get started with this repository, follow these steps:

1. **Download Unity:**
    Download Unity version 2021.3.4f1
    

2. **Clone the repository:**
    ```sh
    git clone https://github.com/kmaxii/Pathfinding-Tests.git
    ```

3. **Run the project**
    You can run the tests to compare the algorithms by clicking play in the Unity editor. The different pathfinding algorithms are put in scriptable object which can be activated or deactivated.



## License

This project is licensed under the Apache-2.0 license License. See the [LICENSE](LICENSE) file for details.

## Acknowledgements

This repository is part of my bachelor thesis at Högskolan i Skövde. Special thanks to our advisor, Andreas Jonasson, for their guidance and support.

For more details, you can read the full thesis [here](https://www.diva-portal.org/smash/record.jsf?dswid=-5593&pid=diva2%3A1850457&c=1&searchType=SIMPLE&language=en&query=Opportunities+for+improvement+in+Dijkstra%27s+pathfinding+algorithm&af=%5B%5D&aq=%5B%5D&aq2=%5B%5D&aqe=%5B%5D&noOfRows=50&sortOrder=author_sort_asc&sortOrder2=title_sort_asc&onlyFullText=false&sf=all).

Thank you for visiting the Pathfinding-test repository!
