( ---------------------------------- Testing --------------------------------- )
\ A test toolset for YSL Forth.

variable test-depth
variable test-quantity

: pick ( a x*i i -- x*i a )
  dup 0 >
  { swap >h 1 - recurse h> swap }
  { drop } if ;


: push-string ( addr n -- c*n )
  { [ ' @ ] literal keep 1 + } times drop ;


: t{ ( -- ) ( test-depth: depth )
  depth test-depth ! ;


: -> ( x*i -- ) ( H: -- x*i ) ( test-quantity: i )
  depth test-depth @ -
  dup test-quantity !
  { i^ @ 1 - pick >h } times ;


: -$> ( addr n "<spaces><delim>ccc<delim>" -- ccc ) ( H: -- c*i )
      ( test-quantity: i )
  -> parse-string
  dup h@ = test-quantity @ 2 = and
  { 2 test-depth !+
    h> h> swap push-string
    -> -2 test-depth !+
    push-string } when ;


: }t ( x*i "<spaces><delim>ccc<delim>" -- ) ( H: x*i -- )
  depth test-depth @ -
  test-quantity @ =
  { true next-tmp-buffer
    test-quantity @
    { rot dup { over ! 1 + } dip
      rot swap h@ = and swap
      h> over ! 1 + } times
    swap parse-string rot
    { ( type say ': SUCCESS' nl ) 2drop drop }
    { type 1 - dup 1 -
      say ": FAIL: Items don't match." nl
      say 'EXPECTED: '
      test-quantity @ { dup @ . 2 - } times drop nl
      say 'ACTUAL  : '
      test-quantity @ { dup @ . 2 - } times drop nl nl } if }
  { parse-string type
    say ': FAIL: Wrong number of items.' nl
    say 'ACTUAL  : '
    depth test-depth @ - [ ' . ] literal times nl
    say 'EXPECTED: '
    test-quantity @ { h> . } times nl nl } if ;


: forgetter ( "word" -- ) ( word: -- )
  here create ysl-here , dictionary-size 1 - , ,
;code
forgetter:
    var here f mem $w
    var w + 1
    var x f mem $w
    var dictionary-pointer - $x
    var dictionary-pointer + 1
    var dict-refs r $x $dictionary-pointer
    var immediates r $x $dictionary-pointer
    string_array l dictionary
    var dictionary-pointer f return
    .dictionary-purge-loop:
        string_array r dictionary $x
        var dictionary-pointer - 1
        gt $dictionary-pointer $x
        goto_if *.dictionary-purge-loop
    var dictionary-pointer - 1
    var w + 1
    var x f mem $w
    var mp - $x
    var mp + 1
    var mem r $x $mp
    var mp = $x
    var mp - 1
    goto *next
;



