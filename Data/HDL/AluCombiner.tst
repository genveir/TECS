load AluCombiner.hdl,
output-file AluCombiner.out,
compare-to AluCombiner.cmp,
output-list x%B3.16.3 y%B3.16.3 f%B3.1.3 out%B3.16.3;

set x %B0000000000000001,
set y %B0000000000000000,

set f 0,
eval,
output;

set f 1,
eval,
output;

set y %B1111111111111111,

set f 0,
eval,
output;

set f 1,
eval,
output;

set x %B0000000000000000,
set y %B0000000000000001,

set f 0,
eval,
output;

set f 1,
eval,
output;

set x %B1111111111111111,

set f 0,
eval,
output;

set f 1,
eval,
output;