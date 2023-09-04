// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/01/And16.tst

load And16.hdl,
output-file And16.out,
compare-to And16.cmp,
output-list a%B1.16.1 b%B1.16.1 out%B1.16.1;

set a 0,
set b 0,
eval,
output;

set a 0,
set b -1,
eval,
output;

set a -1,
set b -1,
eval,
output;

set a -21846,
set b 21845,
eval,
output;

set a 15555,
set b 4080,
eval,
output;

set a 4660,
set b -26506,
eval,
output;