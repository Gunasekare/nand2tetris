// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Mult.asm

// Multiplies R0 and R1 and stores the result in R2.
// (R0, R1, R2 refer to RAM[0], RAM[1], and RAM[2], respectively.)
// Assumes that R0 >= 0, R1 >= 0, and R0 * R1 < 32768 (as per the project website)

// VAL0 = R0
// VAL1 = R1
// SUM = 0
@R0
D = M
@value0
M = D

@R1
D = M
@value1
M = D

@sum
M = 0

// (LOOP)
// IF VAL1 = 0 GOTO FINISH
// SUM = SUM + VAL0
// VAL1 = VAL1 - 1
// GOTO LOOP

(LOOP)
@value1
D = M
@FINISH
D, JEQ

@value0
D = M
@sum
M = D + M

@value1
M = M -1

@LOOP
0, JMP

// (FINISH)
// R2 = SUM

(FINISH)
@sum
D = M
@R2
M = D

// (END)
// GOTO END

(END)
@END
0, JMP