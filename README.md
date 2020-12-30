# Rubicks-Cube
A Rubicks Cube Simulator Made Through Unity

This project is exclusively made for people who are putting their efforts in Rubik’s cube solving algorithm, for testing and debugging process of their code. The simulator provides a platform to directly access the current state of the cube over press of a button. Meanwhile it can also be used for general purposes, having good time, solving the cube on your own.<br />
Simulator is already equipped with a cube solving algorithm, that uses CFOP method of solving cube. It's an external package named Kociemba, link to it can be found under references.


### Instructions to use
Required files to run the simulator can be found in the build folder of the repository. Android users can use the apk file for the installation of the application whereas windows users can go with the other folder.

The Fetch state button is exclusively meant for testing and debugging purposes and therefore is only available for desktop users. When the fetch state button is pressed, the current state of the cube is saved in a txt file named as CurrentState.txt which can be found in the data folder of the simulator.

The simulator can be operated by the pointing devices as well as the keyboard keys.


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

Uppercase turns 90° clockwise, Lowercase turns 90° counter-clockwise


### References
[Kociemba](https://github.com/Megalomatt/Kociemba.git)
