load DFF.hdl,
compare-to DFF.cmp,
output-list time%S1.4.1 in%B2.1.2 clock%B2.1.2 out%B2.1.2;

set in 0,
tick,
output;

tock,
output;

set in 1,
tick,
output;

tock,
output;

set in 0,
tick,
output;

tock,
output;

set in 1,
tick,
output;

tock,
output;

// if you do not increment the clock, a DFF can update immediately
set in 0,
eval,
output;