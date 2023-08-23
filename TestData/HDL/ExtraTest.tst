// Extra test, tests Not with a different filename,
// different formatting, and inverted order of testing

load Not.hdl,
output-file Not.out,
compare-to Not.cmp,
output-list 
    in%B3.1.3 
    out%B3.1.3;

set in 1,
eval,
output;

set in 0,
eval,
output;
