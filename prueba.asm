;Archivo: prueba.asm
;Fecha: 08/11/2022 07:50:13 a. m.
#make_COM#
include emu8086.inc
ORG 100h
;Variables:
	area DW ? 
	radio DW ? 
	pi DW ? 
	resultado DW ? 
	a DW ? 
	d DW ? 
	altura DW ? 
	cinco DW ? 
	x DW ? 
	y DW ? 
	i DW ? 
	j DW ? 
	k DW ? 
PRINTN "Introduce la altura de la piramide: "
CALL SCAN_NUM
MOV altura, CX
PUSH AX
MOV AX,2
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE if 1
InicioFor0:
PUSH AX
POP AX
MOV i, AX
MOV i, AX
PUSH AX
MOV AX,0
PUSH AX
POP BX
POP AX
CMP AX, BX
JLE 
MOV AX,1
PUSH AX
MOV AX,0
PUSH AX
POP AX
MOV j, AX
MOV j, AX
PUSH AX
PUSH AX
PUSH AX
POP BX
POP AX
SUB AX, BX
PUSH AX
POP BX
POP AX
CMP AX, BX
JGE 
PUSH AX
MOV AX,2
PUSH AX
POP BX
POP AX
DIV BX
PUSH DX
MOV AX,0
PUSH AX
POP BX
POP AX
CMP AX, BX
JNE if 3
PRINTN ""*""
JMP else 4
if 3:
PRINTN ""-""
else 4:
