# Rubik's-Cube
A Rubik's Cube Simulator Made Through Unity

I exclusively did this project for people who put their efforts into Rubik's cube solving algorithm to test and debug their code. The simulator provides a platform to directly access the cube's current state over a press of a button. Meanwhile, it can also be used for general purposes, having a good time and solving the cube independently.<br />

The simulator is already equipped with a cube solving algorithm, named The Two-Phase algorithm by [Herbert Kociemba](https://www.speedsolving.com/wiki/index.php/Herbert_Kociemba). It's an external package called Kociemba, link to it can be found under references.<br />


### Instructions To Use
One can find the required files to run the simulator in the build folder of the repository. Android users can use the apk file to install the application, whereas windows users can go with the other folder.<br />

The Fetch state button is exclusively meant for testing and debugging purposes and is only available for desktop users. The fetch state button saves cube's current state into a text file named "CurrentState.txt" which is available in the simulator's data folder.<br />

You can use the pointing devices as well as the keyboard keys to use the simulator.<br />


### Key-Bindings
**F** - Front Face<br />
**L** - Left Face<br />
**B** - Back Face<br />
**R** - Right Face<br />
**U** - Up Face<br />
**D** - Down Face<br />

**M** - Centre Right Vertical<br />
**S** - Centre Left Vertical<br />
**E** - Centre Horizontal<br />

**X** - Rotates Cube on R<br />
**Y** - Rotates Cube on U<br />
**Z** - Rotates Cube on F<br />

**P** - Shuffle<br />
**Q** - Solve<br />
**C** - Get Current State<br />

Uppercase turns 90° clockwise, and Lowercase turns 90° counter-clockwise<br />


### References
1. [Kociemba](https://github.com/Megalomatt/Kociemba.git)
2. [Output Video](https://drive.google.com/file/d/1API5Szd4HY0s9dsJtHZZTSCmi3uhnHcj/view?usp=sharing)
