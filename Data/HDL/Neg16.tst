load Neg16.hdl,
output-file Neg16.out,
compare-to Neg16.cmp,
output-list in%B3.16.3 out%B3.16.3;

set in %B0000000000000000,
eval,
output;

set in %B0000000000000001,
eval,
output;

set in %B1111111111111111,
eval,
output;

set in %B1010101010101010,
eval,
output;