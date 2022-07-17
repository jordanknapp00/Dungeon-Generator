# Dungeon Generator

Dungeon Generator is a program which uses an algorithm similar to that used by [Rogue](https://en.wikipedia.org/wiki/Rogue_(video_game)) (1980). A dungeon made up of simple square rooms, with connections between certain rooms, will be generated. Many parameters can be customized in order to add variance to the result.

One main draw of Dungeon Generator is that if you run the program multiple times with the exact same parameters (including the seed, of course), you will get the same dungeon each time. This makes sharing dungeons relatively simple.

Dungeon Generator is a simple, no-frills program that can give you a basic layout for, say, a D&D dungeon. It gives you a simple starting point from which you can begin to add details however you wish. The only limit is your *imagination*!

## Running Dungeon Generator

**Note:** In order to run Dungeon Generator, you must install the Microsoft .NET Framework.

Dungeon Generator is meant to be run from the command line. It will produce text output to your console, which can be redirected into a file to view it later.

If you run Dungeon Generator with the ```-help``` argument, it will print out information about how to modify the parameters. This is also detailed in a section below.

### Compile and Run With .NET

From the project directory, you can use ```dotnet run``` to compile and run the program. Any desired arguments and parameters can be placed after ```dotnet run```. For example:

```dotnet run -size 96 54 -depth 3 -minsize 3 -splitvar max -sizevar high -doorvar .33``` will run the program with those parameters. The parameters are described in detail in a section below.

### Run the .exe

If you have an executable version of the program (which is available on [my website](https://www.jordanknapp.net)), then simply run the ```dungen.exe``` file on the command line as you would any other. Arguments and parameters can be placed after the name of the executable. For example:

```dungen -size 96 54 -depth 3 -minsize 3 -splitvar max -sizevar high -doorvar .33```

## Customizing Parameters

There are seven parameters that determine how a dungeon is generated. Note that all of these parameters are printed above the dungeon itself.

### Basic Parameters

The following parameters all take a number (or multiple numbers) as input. Entering invalid inputs will cause the program to throw an error, or in some potential cases, crash.

- **Size**: The width and height of a dungeon. You can modify this by running the program with the argument ```-size x y```, where x and y are both numbers. The default dungeon size is 80x80.
- **Recursive depth**: The dungeon is created by repeatedly subdividing areas of the map, much in the same way the original Rogue did. You can modify how many times it is subdivided by running the program with the argument ```-depth d```, where d is a number. Larger map sizes allow for more recursive depth. If the depth is too high, the program will likely crash. The default value is 4. In general, it's best to increase or decrease this value by 1 for each time the map size is doubled or halved, respectively.
- **Seed**: The seed for the random number generator. If none is specified, then the system time is used. You can modify this by running the program with the argument ```-seed s```, where s is a number.
- **Minimum size**: The minimum size that a room can be, in either dimension. The default value is 4, meaning rooms cannot be smaller than 4 units in either the x or y axis. You can modify this by running the program with the argument ```-minsize ms```, where ms is a number.

### Complex Parameters

For the following arguments, both decimal and string values are accepted. If a decimal value between 0 and 1 is provided, it will be used. Otherwise, you can use ```none```, ```low```, ```med```, ```high```, or ```max```.

- **Split variance**: Determines the variance in *where* along an axis the rooms may be subdivided. In other words, higher values can produce more interestingly-shaped rooms. You can modify this parameter by running the program with the argument ```-splitvar spv```, where spv conforms to the values described above.
- **Size variance**: Determines the variance in the size of rooms. You can modify this parameter by running the program with the argument ```-sizevar szv```, where szv conforms to the values described above.
- **Door variance**: Determines the variance in the positions of doors. You can modify this parameter by running the program with the arugment ```-doorvar dv```, where dv conforms to the values described above.

Each string parameter corresponds to a particular decimal value:

- ```none``` corresponds to a value of 0.0
- ```low``` corresponds to a value 0.25
- ```med``` corresponds to a value of 0.5 (this is the default)
- ```high``` corresponds to a value of 0.75
- ```max``` corresponds to a value of 1.0

Generally speaking, using ```max``` for any of the parameters has the potential to cause minor visual issues. The most common issues is two doors not being fully connected by a pathway. Sometimes a pathway tile might be drawn atop a door tile. These are simply visual errors, and are easily ignorable or correctable by hand.

## The Dungeons

When you run the program, the dungeon will be printed to your console, or redirected to a file by using the ```>``` redirection operator when running the program. An example of a dungeon can be found in the [example file](example.txt).

You will see that each room is labeled with a letter. The letters simply ASCII values which are incremented for each room. So it'll run through the capital and lowercase letters, and then start using various symbols if your dungeon is partiuclarly large. Maybe this isn't ideal, but it does succeed in producing a unique identifier for each room. You can see an example of this phenomenon in the [big example file](big.txt).

Each room will have one or more doors. Along vertical axes, doors are represented by an ```H``` character, while along horizontal axes, they're an ```I```. This is meant to somewhat mimic the look of the original Rogue. There also is a pathway or corridor connecting pairs of doors. These are represented by a sequence of ```O```'s.
