// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Fill.asm

// Runs an infinite loop that listens to the keyboard input.
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel;
// the screen should remain fully black as long as the key is pressed. 
// When no key is pressed, the program clears the screen, i.e. writes
// "white" in every pixel;
// the screen should remain fully clear as long as no key is pressed.


//SCREENLOC = 0
//SCREENSIZE = 8192
//(PRESS)
//	INPUT = KBD
//	IF INPUT > 0 GOTO COLOR
//	GOTO PRESS

@screenloc
M = 0
@8192
D = A
@screensize
M = D

(PRESS)
@KBD
D = M
@COLOR
D, JGT
@PRESS
0, JMP

//(COLOR)
//	IF SCREENSIZE == SCREENLOC THEN GOTO DEPRESS
//		SCREEN + SCREENLOC = -1
//		SCREENLOC = SCREENLOC + 1
//		GOTO COLOR

(COLOR)
@screensize
D = M
@screenloc
D = D - M
@DEPRESS
D, JEQ

@screenloc
D = M
@SCREEN
A = D + A
M = -1

@screenloc
M = M + 1

@COLOR
0, JMP

//(DEPRESS)
//	INPUT = KBD
//	IF INPUT = 0 GOTO CLEAR ELSE GOTO DEPRESS

(DEPRESS)
@KBD
D = M
@CLEAR
D, JEQ
@DEPRESS
0, JMP

//(CLEAR)
//	IF SCREENLOC == 0 THEN GOTO PRESS
//		SCREEN + SCREENLOC = 0
//		SCREENLOC = SCREENLOC - 1
//		GOTO CLEAR

(CLEAR)
@screenloc
D = M
@PRESS
D, JLT

@screenloc
D = M
@SCREEN
A = D + A
M = 0

@screenloc
M = M - 1

@CLEAR
0, JMP