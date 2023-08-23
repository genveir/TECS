load AluPreset.hdl,
output-file AluPreset.out,
compare-to AluPreset.cmp,
output-list in%B3.16.3 zin%B3.1.3 nin%B3.1.3 out%B3.16.3;

set in %B1010101010101010,

set zin 0,
set nin 0,
eval,
output;

set zin 1,
set nin 0,
eval,
output;

set zin 0,
set nin 1,
eval,
output;

set zin 1,
set nin 1,
eval,
output;