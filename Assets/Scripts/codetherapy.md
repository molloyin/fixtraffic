### This is Matt's understanding of the code

##### TLDR 
TrafficManager is the holy grail, connecting vehicles and waypoints to each other, and to the game world.
Graph and path creator creates an adjacency matrix which describes how intersections are connected(very cool), 
and contains distance data for vehicle pathing. TurnMaker automatically generates smooth pathing between 
intersection entrances and exits(very cool). Vehicles are what you'd imagine; they have a start/end, a 
continually updated path from their current position to the end (very cool), and methods for adjusting their
orientation and speed. Waypoints seem to do a lot of shit, see below.


##### GraphCreator:
Creates an adjacency matrix for all intersection waypoints(entrances
and exits), not only describing their connections(edges), but also their distances?
Presumably used by the relevant traffic script to animate pathing, and by dijkstras to decide pathing.

Also contains a GraphToString method which prints the adj-matrix for misc uses?

##### PathCreator 
Is passed an adjacency matrix describing all vertex connections, an origin, and a destination, and 
finds the shortest path between--adding it to a list. List then used as pathing for vehicle objects?

    Important note: if using an adj-matrix to describe the entire sims road network, makes 
    sense for it to be a global variable. Passing it through a function for every vehicle 
    (copying it onto the stack) is unnecessarily compute heavy... chatGPT agrees, but will
    have to be carefully maintained when adding interections.

Confused about pathlist--appears to only add vertexes to first element, how does it create a complete 
pathing list? does it outsource that job to vertex predecessors?

##### TrafficManager + TrafficManagerEditor
These are a little too monolithic and multipurpose for me to describe here, I think I have a good general idea
of what they do: "Manage the simulation environment"...  connect vehicles and waypoints with the simulation 

##### TurnMaker
Very cool wee script, automatically creates a turn/curve between intersection entrances and exits. You can 
adjust how many points there are along the curve, increasing turn smoothness. There is "middle" variable "B" 
which connects entrance and exit, used in building the curve.

##### Vehicle
Vehicles are created with a start and end point, spawn at start, uses adj-matrix to find best next waypoint
when it reaches a new waypoint (and it's not the end point)... current is updated, it finds next waypoint, 
adjusts orientation/points to next waypoint, and adjusts speed.

##### Waypoint
Contains lists of waypoints used by vehicles. I think PathToNext describes how to change lanes? There are also
Gizmos, which visualize in unity how waypoints are connected to one another, and maybe drawgizmoarrow is used
to create new waypoints?

##### WaypointEditor
Fucked if I know LOL


##### Questions/Issues/Ideas

    Are waypoints and vertices the same things?

    Does the "speed" attribute in Waypoint.cs control the speed of vehicles passing through that waypoint?
    If so, makes me nervous, as will be tricky to create variability in behaviour. Makes more sense to have
    speed tied to Vehicle? (Note: interesting, in Vehicle.update() vehicle speed is dictated by a combination of
    waypoint speed and Time.deltatime... please explain what this does)

    Class structure isn't very decoupled... TrafficManager, Waypoint, and Vehicle all instantiate one another.
    Unlikely to be a problem for us, but would get messy at scale? (solution is an interface, so if we change
    one class, we don't have to change the others.)

    FindNextWaypoint implements a depth-first algorithm, if the waypoints are local/close together, will backtracking
    cause weird vehicle behaviour?

    In turnmaker, is it possible in more complex intersections there will have to be more than one point in between 
    entrance and exit, or is B/Mid enough?

    Could we use Turns instead of Gizmos to visualize connections between waypoints in unity?