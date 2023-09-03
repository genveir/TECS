load Latch.hdl,
compare-to Latch.cmp,
output-list s%B1.1.1 r%B1.1.1 out%B2.1.2;

set s 0,
set r 1,
eval,
output;

set s 1,
set r 1,
eval,
output;

set s 1,
set r 0,
eval,
output;

set s 1,
set r 1,
eval,
output;