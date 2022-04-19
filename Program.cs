using System;

class Dungeon
{
    private static int seed = Environment.TickCount;
    private static System.Random random = new Random(seed);

    private static float splitVariance = .5f;    //used to determine variance in bsp splits
    private static float sizeVariance = .5f;     //used to determine variance in room size
    private static float doorVariance = .5f;     //used to determine variance in door location

    private static int minSize = 4;             //minimum size for rooms in any dimension

    private static int depth = 4;               //depth of recursion for bsp algorithm

    private static int width = 80;
    private static int height = 80;

    private static List<Room> rooms = new List<Room>();

    private static char currID = 'A';         //used to assign unique id to each room

    static void Main(string[] args)
    {
        //parse any command line inputs, if there are any
        if(args.Length > 0)
        {
            ParseInput(args);
        }

        Console.WriteLine("GENERATING NEW DUNGEON...");
        Console.WriteLine("Parameters (copy these parameters to save your dungeon):");
        Console.WriteLine("Size:\t\t\t" + width + "x" + height);
        Console.WriteLine("Depth:\t\t\t" + depth);
        Console.WriteLine("Seed:\t\t\t" + seed);
        Console.WriteLine("Minsize:\t\t" + minSize);
        Console.WriteLine("Split variance:\t" + splitVariance);
        Console.WriteLine("Size variance:\t" + sizeVariance);
        Console.WriteLine("Door variance:\t" + doorVariance);

        //this is absolutely the stupidest way to do it, but i must've screwed up somewhere and
        //gotten width and height mixed up (sigh). so basically, we're just gonna swap 'em and
        //act like nothing's wrong.
        int temp = width;
        width = height;
        height = temp;

        //we start with a room that's the size of the entire area. it will be subdivided
        Room startRoom = new Room(0, width - 1, 0, height - 1);

        BinarySpacePartition(startRoom, depth);

        Shrink();

        Draw();
    }

    static void ParseInput(string[] args)
    {
        if(args[0] == "-help")
        {
            Console.WriteLine("Dungeon Generator v1.0 Help:");
            Console.WriteLine("");
            Console.WriteLine("Arg          Params  : Description");
            Console.WriteLine("");
            Console.WriteLine("-size        x y     : sets width to x and height to y (default 80 80)");
            Console.WriteLine("-depth       d       : sets recursive depth to d (default 4)");
            Console.WriteLine("                       depth determines how many rooms are generated. there will");
            Console.WriteLine("                       be 2^depth rooms");
            Console.WriteLine("-seed        s       : sets seed to s (defaults to a random value)");
            Console.WriteLine("-minsize     ms      : sets minimum room size (on either axis) to ms (default 4)");
            Console.WriteLine("");
            Console.WriteLine("For the following commands, both decimal and string values are accepted.");
            Console.WriteLine("If a decimal value between 0 and 1 is provided, it will be used. Otherwise,");
            Console.WriteLine("you can use 'none', 'low', 'med', 'high', and 'max'.");
            Console.WriteLine("The default for all of these is 'med'.");
            Console.WriteLine("");
            Console.WriteLine("-splitvar    spv     : sets the split variance to spv");
            Console.WriteLine("-sizevar     szv     : sets the size variance to szv");
            Console.WriteLine("-doorvar     dv      : sets the door variance to dv");
            Console.WriteLine("");
            Console.WriteLine("Note: It is best to redirect your output to a file!");
            Console.WriteLine("Use the '>' operator after the executable, and enter a filename to write to.");
            return;
        }

        int index = 0;
        while(index < args.Length)
        {
            switch(args[index])
            {
                case "-size":
                    try
                    {
                        width = Int32.Parse(args[index + 1]);
                        height = Int32.Parse(args[index + 2]);
                        index++; //must increment index one additional amount
                    }
                    catch(FormatException)
                    {
                        InvalidInput(args[index], "", false);
                        return;
                    }
                    break;
                case "-depth":
                    try
                    {
                        depth = Int32.Parse(args[index + 1]);
                    }
                    catch(FormatException)
                    {
                        InvalidInput(args[index], args[index + 1], false);
                        return;
                    }
                    break;
                case "-seed":
                    try
                    {
                        seed = Int32.Parse(args[index + 1]);
                        random = new Random(seed);
                    }
                    catch(FormatException)
                    {
                        InvalidInput(args[index], args[index + 1], false);
                        return;
                    }
                    break;
                case "-minsize":
                    try
                    {
                        minSize = Int32.Parse(args[index + 1]);
                    }
                    catch(FormatException)
                    {
                        InvalidInput(args[index], args[index + 1], false);
                        return;
                    }
                    break;
                case "-splitvar":
                    if(args[index + 1] == "none")
                    {
                        splitVariance = 0f;
                    }
                    else if(args[index + 1] == "low")
                    {
                        splitVariance = .25f;
                    }
                    else if(args[index + 1] == "med")
                    {
                        splitVariance = .5f;
                    }
                    else if(args[index + 1] == "high")
                    {
                        splitVariance = .75f;
                    }
                    else if(args[index + 1] == "max")
                    {
                        splitVariance = 1f;
                    }
                    else
                    {
                        try
                        {
                            splitVariance = float.Parse(args[index + 1]);
                            if(splitVariance < 0 || splitVariance > 1)
                            {
                                throw new FormatException();
                            }
                        }
                        catch(FormatException)
                        {
                            InvalidInput(args[index], args[index + 1], false);
                            return;
                        }
                    }
                    break;
                case "-sizevar":
                    if(args[index + 1] == "none")
                    {
                        sizeVariance = 0f;
                    }
                    else if(args[index + 1] == "low")
                    {
                        sizeVariance = .25f;
                    }
                    else if(args[index + 1] == "med")
                    {
                        sizeVariance = .5f;
                    }
                    else if(args[index + 1] == "high")
                    {
                        sizeVariance = .75f;
                    }
                    else if(args[index + 1] == "max")
                    {
                        sizeVariance = 1f;
                    }
                    else
                    {
                        try
                        {
                            sizeVariance = float.Parse(args[index + 1]);
                            if(sizeVariance < 0 || sizeVariance > 1)
                            {
                                throw new FormatException();
                            }
                        }
                        catch(FormatException)
                        {
                            InvalidInput(args[index], args[index + 1], false);
                            return;
                        }
                    }
                    break;
                case "-doorvar":
                    if(args[index + 1] == "none")
                    {
                        doorVariance = 0f;
                    }
                    else if(args[index + 1] == "low")
                    {
                        doorVariance = .25f;
                    }
                    else if(args[index + 1] == "med")
                    {
                        doorVariance = .5f;
                    }
                    else if(args[index + 1] == "high")
                    {
                        doorVariance = .75f;
                    }
                    else if(args[index + 1] == "max")
                    {
                        doorVariance = 1f;
                    }
                    else
                    {
                        try
                        {
                            doorVariance = float.Parse(args[index + 1]);
                            if(doorVariance < 0 || doorVariance > 1)
                            {
                                throw new FormatException();
                            }
                        }
                        catch(FormatException)
                        {
                            InvalidInput(args[index], args[index + 1], false);
                            return;
                        }
                    }
                    break;
                default:
                    InvalidInput(args[index], "", true);
                    return;
            }

            index += 2;
        }
    }

    static void InvalidInput(String arg, String value, bool noVal)
    {
        TextWriter errorWriter = Console.Error;

        if(noVal)
        {
            errorWriter.WriteLine("ERROR: Invalid argument '" + arg + "'");
        }
        //slightly different error message for -size
        else if(arg == "-size")
        {
            errorWriter.WriteLine("ERROR: Invalid parameters for '-size'");
        }
        else
        {
            errorWriter.WriteLine("ERROR: Parameter '" + value + "' is not valid for arg '" + arg + "'");
        }
        
        return;
    }

    static void BinarySpacePartition(Room room, int levelsToGo)
    {
        //check base cases first
        //stop when we've reached the desired depth, or if the current room reached the min size
        if(levelsToGo == 0 || room.rightWall - room.leftWall < minSize || room.topWall - room.bottomWall < minSize)
        {
            room.id = currID++;
            rooms.Add(room);
            return;
        }

        levelsToGo--;

        //determine the direction upon which we are going to split. whichever wall is longer is
        //the one being split
        if(room.rightWall - room.leftWall > room.topWall - room.bottomWall)
        {
            int splitPoint = Split(room.leftWall, room.rightWall, splitVariance);

            //using the split point, make two new rooms split down the line along that point
            Room leftRoom = new Room(room.leftWall, splitPoint, room.bottomWall, room.topWall);
            Room rightRoom = new Room(splitPoint, room.rightWall, room.bottomWall, room.topWall);

            //since this room started as one big room, we need to make sure that doors end up in
            //the appropriate room. any doors that were on the left side of the split should be a
            //part of the left room, and vice versa
            foreach(Door doorAt in room.doors)
            {
                Room toGetDoor;

                if(doorAt.x < splitPoint)
                {
                    toGetDoor = leftRoom;
                }
                else
                {
                    toGetDoor = rightRoom;
                }

                toGetDoor.AddDoor(doorAt);

                //since we're splitting vertically, and there is a specific allowed variance for
                //door locations, we want to relocate a door so it is within the allowed variance.
                //for example, with 0 variance, doors are always in the center of a room. vertical
                //doors, however, would no longer be because we've split along the vertical axis.
                //fortunately, we can use the same split function in order to move the door within
                //the appropriate bounds
                if(!doorAt.horizontal)
                {
                    doorAt.x = Split(toGetDoor.leftWall, toGetDoor.rightWall, doorVariance);
                }
            }

            //after any necessary doors have been added and relocated, we need to add a new set of
            //doors between the two rooms we've just created. we've split vertically, so the doors
            //are going to be horizontal. in both cases, their location along the y axis will be
            //based on the door variance using the split function
            Door leftDoor = new Door(splitPoint, Split(room.bottomWall, room.topWall, doorVariance), true);
            Door rightDoor = new Door(splitPoint, Split(room.bottomWall, room.topWall, doorVariance), true);

            leftDoor.AssignOtherDoor(rightDoor);
            rightDoor.AssignOtherDoor(leftDoor);

            leftRoom.doors.Add(leftDoor);
            rightRoom.doors.Add(rightDoor);

            //now recursively split these subrooms
            BinarySpacePartition(leftRoom, levelsToGo);
            BinarySpacePartition(rightRoom, levelsToGo);
        }
        else
        {
            //split across the opposite dimension. the code here is largely the same as above, just
            //doing things in the x direction instead of y
            int splitPoint = Split(room.bottomWall, room.topWall, splitVariance);

            Room bottomRoom = new Room(room.leftWall, room.rightWall, room.bottomWall, splitPoint);
            Room topRoom = new Room(room.leftWall, room.rightWall, splitPoint, room.topWall);

            foreach(Door doorAt in room.doors)
            {
                Room toGetDoor;

                if(doorAt.y < splitPoint)
                {
                    toGetDoor = bottomRoom;
                }
                else
                {
                    toGetDoor = topRoom;
                }

                toGetDoor.AddDoor(doorAt);

                if(doorAt.horizontal)
                {
                    doorAt.y = Split(toGetDoor.bottomWall, toGetDoor.topWall, doorVariance);
                }
            }

            Door bottomDoor = new Door(Split(room.leftWall, room.rightWall, doorVariance), splitPoint, false);
            Door topDoor = new Door(Split(room.leftWall, room.rightWall, doorVariance), splitPoint, false);

            bottomDoor.AssignOtherDoor(topDoor);
            topDoor.AssignOtherDoor(bottomDoor);

            bottomRoom.AddDoor(bottomDoor);
            topRoom.AddDoor(topDoor);

            BinarySpacePartition(bottomRoom, levelsToGo);
            BinarySpacePartition(topRoom, levelsToGo);
        }
    }

    static int Split(int min, int max, float variance)
    {
        //get our random variance value, the amount from which we're deviating from the center
        //with no variance, we will always split right down the middle of a room.
        //the variance value will always be between -1 and 1. -1 inclusive, 1 exclusive
        float randomVariance = (float) (random.NextDouble() * 2 - 1);

        float midPoint = (min + max) / 2;

        //we're using the variance that was randomly generated as well as the given varaince
        //value, which will depend on the purposes for which the function is being used
        midPoint += randomVariance * variance * (max - min) / 2;

        return (int) midPoint;
    }

    //function for drawing the map to the console
    //
    //'.' = void space
    //' ' = open space
    //'+' = room corner
    //'O' = tunnel
    //'H' = vertical door
    //'I' = horizontal door
    //'|' = vertical wall
    //'-' = horizontal wall
    static void Draw()
    {
        char[,] map = new char[width, height];

        //begin by filling the entire space with void.
        //the requisite parts will be drawn over later
        for(int col = 0; col < width; ++col)
        {
            for(int row = 0; row < height; ++row)
            {
                map[col, row] = '.';
            }
        }

        foreach(Room room in rooms)
        {
            int left = room.leftWall;
            int right = room.rightWall;
            int bottom = room.bottomWall;
            int top = room.topWall;

            //place horizontal lines for tops and bottoms of rooms, ignoring corners
            for(int col = left + 1; col < right; ++col)
            {
                map[col, bottom] = '|';
                map[col, top] = '|';
            }

            //place vertical lines for sides of rooms, ignoring corners
            for(int row = bottom + 1; row < top; ++row)
            {
                map[left, row] = '-';
                map[right, row] = '-';
            }

            //fill rooms with empty space
            for(int col = left + 1; col < right; ++col)
            {
                for(int row = bottom + 1; row < top; ++row)
                {
                    map[col, row] = ' ';
                }
            }
        }

        //apply corner tile for each room, as well as room name in the middle
        foreach(Room room in rooms)
        {
            int left = room.leftWall;
            int right = room.rightWall;
            int bottom = room.bottomWall;
            int top = room.topWall;

            map[left, bottom] = '+';
            map[right, bottom] = '+';
            map[left, top] = '+';
            map[right, top] = '+';

            map[(left + right) / 2, (bottom + top) / 2] = room.id;
        }

        foreach(Room room in rooms)
        {
            foreach(Door door in room.doors)
            {
                int col = door.x;
                int row = door.y;

                //system for fixing rounding errors by checking adjacent tiles
                if(col > 0 && col < width - 1)
                {
                    if(map[col, row - 1] != '-' || map[col, row + 1] != '-')
                    {
                        if(map[col - 1, row] == '-')
                        {
                            col--;
                        }
                        else
                        {
                            col++;
                        }
                    }
                }

                if(row > 0 && row < height - 1)
                {
                    if(map[col - 1, row] != '|' || map[col + 1, row] != '|')
                    {
                        if(map[col, row - 1] == '|')
                        {
                            row--;
                        }
                        else
                        {
                            row++;
                        }
                    }
                }

                if(door.horizontal)
                {
                    map[col, row] = 'I';
                }
                else
                {
                    map[col, row] = 'H';
                }

                ConnectDoors(map, door);
            }
        }

        //now to actually print it to the console
        for(int col = 0; col < width; ++col)
        {
            for(int row = 0; row < height; ++row)
            {
                Console.Write(map[col, row]);
            }

            Console.WriteLine();
        }
    }

    static void ConnectDoors(char[,] map, Door door)
    {
        if(door.horizontal)
        {
            int midPoint = door.divDim;

            int fromCol = door.x;
            int toCol = door.GetOtherDoor().x;

            int fromRow = door.y;
            int toRow = door.GetOtherDoor().y;

            if(fromCol > toCol)
            {
                int temp = fromCol;
                fromCol = toCol;
                toCol = temp;

                temp = fromRow;
                fromRow = toRow;
                toRow = temp;
            }
            
            fromRow++;
            toRow++;

            for(int col = fromCol + 1; col <= midPoint; ++col)
            {
                map[col, fromRow] = 'O';
            }

            for(int col = midPoint; col < toCol; ++col)
            {
                map[col, toRow] = 'O';
            }

            if(fromRow > toRow)
            {
                int temp = fromRow;
                fromRow = toRow;
                toRow = temp;
            }

            for(int row = fromRow + 1; row < toRow; ++row)
            {
                map[midPoint, row] = 'O';
            }
        }
        else
        {
            int mid = door.divDim;

            int fromCol = door.x;
            int toCol = door.GetOtherDoor().x;

            int fromRow = door.y;
            int toRow = door.GetOtherDoor().y;

            if (fromRow > toRow)
            {
                int temp = fromCol;
                fromCol = toCol;
                toCol = temp;

                temp = fromRow;
                fromRow = toRow;
                toRow = temp;
            }

            fromCol++;
            toCol++;

            for (int row = fromRow + 1; row <= mid; ++row)
            {
                map[fromCol, row] = 'O';
            }

            for (int row = mid; row < toRow; ++row)
            {
                map[toCol, row] = 'O';
            }

            if (fromCol > toCol)
            {
                int temp = fromCol;
                fromCol = toCol;
                toCol = temp;
            }

            for (int col = fromCol + 1; col < toCol; ++col)
            {
                map[col, mid] = 'O';
            }
        }
    }

    static void Shrink()
    {
        //for each room, shrink it horizontally and vertically by some amount, based on
        //sizeVariance
        foreach(Room room in rooms)
        {    
            //shrink horizontally first
            float horizScale = (float) (1 - sizeVariance * random.NextDouble());
            float width = room.rightWall - room.leftWall;
            float newWidth = horizScale * width;

            //at minimum, each room will be made 2 units smaller, to create minimum space between
            //each room
            if (width - newWidth < 2)
            {
                newWidth = width - 2;
            }

            //but at the same time, rooms must conform to the minimum size
            if (newWidth < minSize)
            {
                newWidth = minSize;
            }

            float centerWidth = (room.rightWall + room.leftWall) / 2;

            room.rightWall = (int) (centerWidth + newWidth / 2);
            room.leftWall = (int) (centerWidth - newWidth / 2);

            //now do the same process, but vertically
            float vertScale = (float)(1 - sizeVariance * random.NextDouble());
            float height = room.topWall - room.bottomWall;
            float newHeight = vertScale * height;

            if (height - newHeight < 2)
            {
                newHeight = height - 2;
            }

            if (newHeight < minSize)
            {
                newHeight = minSize;
            }

            float centerHeight = (room.topWall + room.bottomWall) / 2;

            room.topWall = (int) (centerHeight + newHeight / 2);
            room.bottomWall = (int) (centerHeight - newHeight / 2);

            horizScale = newWidth / width;
            vertScale = newHeight / height;

            //need to also adjust the doors so they're always inside a wall
            foreach (Door door in room.doors)
            {
                float doorOffsetX = door.x - centerWidth;
                door.x = (int) (horizScale * doorOffsetX + centerWidth);

                float doorOffsetY = door.y - centerHeight;
                door.y = (int) (vertScale * doorOffsetY + centerHeight);
            }
        }
    }
}

class Room
{
    public int leftWall;
    public int rightWall;
    public int topWall;
    public int bottomWall;

    public char id;

    public List<Door> doors { get; }

    public Room(int leftWall, int rightWall, int bottomWall, int topWall)
    {
        this.leftWall = leftWall;
        this.rightWall = rightWall;
        this.topWall = topWall;
        this.bottomWall = bottomWall;

        doors = new List<Door>();
    }

    public void AddDoor(Door door)
    {
        doors.Add(door);
    }
}

class Door
{
    //door variables are completely public because they made need to be changed
    public int x;
    public int y;
    public int divDim;

    public bool horizontal { get; }

    private Door? other; //a door always must connect to another door

    public Door(int x, int y, bool horizontal)
    {
        this.x = x;
        this.y = y;
        this.horizontal = horizontal;

        if(horizontal)
        {
            divDim = x;
        }
        else
        {
            divDim = y;
        }
    }

    public void AssignOtherDoor(Door other)
    {
        this.other = other;
    }

    public Door GetOtherDoor()
    {
        return other;
    }
}