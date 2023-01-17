: thing ;

code drop # ( x -- )
    var t f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    goto *next
;


code parse # ( char "ccc<char>" -- addr u )
parse:
    var delim = $t
    var addr = $source
    var addr + $>in
    var d-stack a $addr
    var dp + 1
    var t = $addr
    .loop:
        lt $>in $source-size
        goto_if *.not-end-of-source
        goto *.end
        .not-end-of-source:
        var x = $t
        gosub *_@
        var x f return
        var >in + 1
        var t + 1
        cmp $x $delim
        goto_if *.end
        goto *.loop
    .end:
    var t - $addr
    var t - 1
    goto *next
;


code immediate # ( -- )
    var immediates s $dictionary-pointer -1
    goto *next
;


: ( ') parse drop drop ; immediate


: \ 10 parse drop drop ; immediate


( --- Debugging --- )

code memory-dump # ( -- )
memory-dump:
    print "MP: "
    println $mp
    size mem
    var x f return
    .loop:
        var x - 1
        var w f mem $x
        print $x
        print ":"
        print $w
        print " "
        gt $x 0
        goto_if *.loop
    goto *next
;


code dict-dump # ( -- )
dict-dump:
    print "DP: "
    println $dictionary-pointer
    size dict-refs
    var x f return
    .loop:
        var x - 1
        print $x
        print ": "
        string_array g dictionary $x
        var w c return
        print !w
        print " "
        var w f dict-refs $x
        print $w
        print " "
        var w f mem $w
        print $w
        var w f immediates $x
        cmp $w 1
        goto_if *.not-immediate
        print " immediate"
        .not-immediate:
        println ""
        gt $x 0
        goto_if *.loop
    goto *next
;


code .s # ( -- )
stack-dump:
    lt $dp 0
    goto_if *.empty
    var x = 1
    print "Stack: "
    .loop:
        gt $x $dp
        goto_if *.end
        var w f d-stack $x
        print $w
        print " "
        var x + 1
        goto *.loop
    .end:
        println $t
        goto *next
    .empty:
        println "Stack:"
        goto *next
;


( --- System variables --- )

code ysl-here # ( -- x )
    var d-stack a $t
    var dp + 1
    var t = $here
    goto *next
;


code here # ( -- addr )
    var d-stack a $t
    var dp + 1
    var t = $mp
    var t + 1
    goto *next
;


code enter-action # ( -- x )
    var d-stack a $t
    var dp + 1
    var t = *enter
    goto *next
;


code tco-size # ( -- addr )
    var d-stack a $t
    var dp + 1
    var t = $tco-size-offset
    goto *next
;


code dictionary-size # ( -- n )
    var d-stack a $t
    var dp + 1
    var t = $dictionary-pointer
    var t + 1
    goto *next
;


code state # ( -- ? )
    var d-stack a $t
    var dp + 1
    var t = $state-offset
    goto *next
;


( --- Stack shuffling --- )

( drop defined at the top )

code dup # ( x -- x x )
    var d-stack a $t
    var dp + 1
    goto *next
;


code swap # ( a b -- b a )
    var x f d-stack $dp
    var d-stack s $dp $t
    var t = $x
    goto *next
;


code over # ( a b -- a b a )
    var d-stack a $t
    var t f d-stack $dp
    var dp + 1
    goto *next
;


code rot # ( a b c -- b c a )
    var x f d-stack $dp
    var d-stack s $dp $t
    var dp - 1
    var t f d-stack $dp
    var d-stack s $dp $x
    var dp + 1
    goto *next
;


code >r # ( x -- ) ( R: -- x )
    var r-stack a $t
    var rp + 1
    var t f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    goto *next
;


code r> # ( -- x ) ( R: x -- )
    var d-stack a $t
    var dp + 1
    var t f r-stack $rp
    var r-stack r $rp 1
    var rp - 1
    goto *next
;


code r@ # ( -- x ) ( R: x -- x )
    var d-stack a $t
    var dp + 1
    var t f r-stack $rp
    goto *next
;


code >h # ( x -- ) ( H: -- x )
    var h-stack a $t
    var hp + 1
    var t f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    goto *next
;


code h> # ( -- x ) ( H: x -- )
    var d-stack a $t
    var dp + 1
    var t f h-stack $hp
    var h-stack r $hp 1
    var hp - 1
    goto *next
;


code h@ # ( -- x ) ( H: x -- x )
    var d-stack a $t
    var dp + 1
    var t f h-stack $hp
    goto *next
;


code depth # ( -- n )
    var d-stack a $t
    var dp + 1
    var t = $dp
    goto *next
;


( --- Memory access --- )

code @ # ( addr -- x )
    var x = $t
    gosub *_@
    var t f return
    goto *next
;


code ! # ( x addr -- )
!:
    lt $t 0
    goto_if *.mapped
    gt $t $mp
    goto_if *.out-of-bounds
    var x f d-stack $dp
    var mem s $t $x
    goto *.out-of-bounds
.mapped:
    lt $t $>in-offset
    goto_if *.tco-size
    gt $t $>in-offset
    goto_if *.out-of-bounds
    var >in f d-stack $dp
    goto *.out-of-bounds
.tco-size:
    lt $t $tco-size-offset
    goto_if *.tmp-buffers
    gt $t $tco-size-offset
    goto_if *.out-of-bounds
    var tco-size f d-stack $dp
    goto *.out-of-bounds
.tmp-buffers:
    lt $t $tmp-buffers-offset
    goto_if *.state
    var t - $tmp-buffers-offset
    gt $t $total-tmp-buffers-size
    goto_if *.out-of-bounds
    var x f d-stack $dp
    var tmp-buffers s $t $x
    goto *.out-of-bounds
.state:
    lt $t $state-offset
    goto_if *.out-of-bounds
    gt $t $state-offset
    goto_if *.out-of-bounds
    var state f d-stack $dp
    cmp $state 0
    goto_if *.out-of-bounds
    var state = -1
    goto *.out-of-bounds
.out-of-bounds:
    var dp - 1
    var t f d-stack $dp
    var d-stack r $dp 2
    var dp - 1
    goto *next
;


\ ``!+`` is defined in `Maths and logic`_


code next-tmp-buffer # ( -- addr )
    var d-stack a $t
    var dp + 1
    var t = $next-tmp-buffer
    var t * $tmp-buffer-size
    var t + $tmp-buffers-offset
    var next-tmp-buffer + 1
    var next-tmp-buffer % $tmp-buffers-num
    goto *next
;


( --- Parsing --- )

( parse is defined at the start of kernel )
    
code ' # ( "<spaces>word<space>" -- xt )
':
    gosub *parse-word
    gosub *_find
    goto_if *.found
    print "Error when executing ': word\"
    print !word
    println "\" not found."
    exit
    .found:
    var d-stack a $t
    var dp + 1
    var t = $x
    goto *next
;


code parse-string # ( "<spaces><delim>ccc<delim>" -- addr n )
parse-string:
    gosub *parse-whitespace
    var addr = $source
    var addr + $>in
    var d-stack a $t
    var dp + 1
    var x = $addr
    gosub *_@
    var t f return
    var >in + 1
    goto *parse
;


( I tried to implement $ as a Forth word, and failed. )
code $ # ( "<spaces><delim>ccc<delim>" -- | addr n )
string:
    cmp $state 0
    goto_if *.interpret-header
        var mem a $lit-action
        var mp + 7
        var mem a $mp
        var mp - 3
        var hole = $mp
        var mp + 2
        var mem a $lit-action
        var mem a 0
        var mem a $jump-action
        var mem a 0
        goto *.after-header
    .interpret-header:
        var target = $next-tmp-buffer
        var target * $tmp-buffer-size
        var start = $target
    .after-header:
    gosub *parse-whitespace
    var addr = $source
    var addr + $>in
    var x = $addr
    gosub *_@
    var delim f return
    .loop:
        var addr + 1
        var >in + 1
        lt $>in $source-size
        goto_if *.not-end
        goto *.end
        .not-end:
        var x = $addr
        gosub *_@
        var x f return
        cmp $x $delim
        goto_if *.end
        cmp $state 0
        goto_if *.interpret
            var mem a $x
            var mp + 1
            goto *.loop
        .interpret:
            var tmp-buffers s $target $x
            var target + 1
            goto *.loop
    .end:
        var >in + 1
        cmp $state 0
        goto_if *.interpret-footer
        var len = $mp
        var len - $hole
        var len - 2
        var mem s $hole $len
        var hole + 2
        var len = $mp
        var len + 1
        var mem s $hole $len
        var tco-size = 0
        goto *next
    .interpret-footer:
        var d-stack a $t
        var t = $target
        var t - $start
        var start + $tmp-buffers-offset
        var d-stack a $start
        var dp + 2
        var next-tmp-buffer + 1
        var next-tmp-buffer % $tmp-buffers-num
        goto *next
; immediate


code postpone # ( "<spaces>word<space>" -- )
postpone:
    gosub *parse-word
    gosub *_find
    var found f return
    var tco-size = 1
    cmp $found -1
    goto_if *.immediate
        var mem a $lit-action
        var mem a $x
        var mem a $comma-action
        var mp + 3
        goto *next
    .immediate:
        var mem a $x
        var mp + 1
        goto *next
; immediate


( --- Defining --- )

( : code compile ; are defined in core )

code define # ( "<spaces>word<space>" -- )
    gosub *define
    goto *next
;


code latest # ( -- xt )
    var d-stack a $t
    var dp + 1
    var t f dict-refs $dictionary-pointer
    goto *next
;


code shadow # ( -- )
    var dictionary-pointer - 1
    goto *next
;


code unshadow # ( -- )
    var dictionary-pointer + 1
    goto *next
;


code literal # ( x -- )
    var mem a $lit-action
    var mem a $t
    var mp + 2
    var t f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    var tco-size = 0
    goto *next
; immediate


code jump # ( addr -- )
    var mem a $jump-action
    var mem a $t
    var mp + 2
    var t f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    var tco-size = 0
    goto *next
; immediate


code create # ( "<spaces>word<space>" -- )
#   println "Starting create"
    gosub *define
    var mem a *dovar
    var mp + 1
#   println "Finished create"
    var tco-size = 0
    goto *next
#   println "oops, too far into create"
;


: variable ( x "<spaces>word<space>" -- )
  create 0 , ;


: ;code ( If you don't know what it does a stack effect won't help )
    ysl-here postpone literal
    postpone latest
    postpone !
    postpone ;
    compile
; immediate


: constant ( x "<spaces>word<space>" -- )
  create , ;code
    var d-stack a $t
    var dp + 1
    var t f mem $w
    goto *next
;


code nop # ( -- )
    var tco-size = 0
    goto *next
; immediate


( --- Maths and logic --- )

code + # ( n n -- n )
    var x f d-stack $dp
#   print $t
#   print " + "
#   print $x
#   print " = "
    var t + $x
#   println $t
    var d-stack r $dp 1
    var dp - 1
    goto *next
;


code - # ( n n -- n )
    var x f d-stack $dp
    var x - $t
    var d-stack r $dp 1
    var dp - 1
    var t = $x
    goto *next
;


code * # ( n n -- n )
#   println "Calling *"
    var x f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    var t * $x
    goto *next
;


: !+ ( x addr -- )
  swap over @ + swap ! ;


 0 constant false
-1 constant true


code 0= # ( n -- ? )
    cmp 0 $t
    goto_if *.true
    var t = 0
    goto *next
    .true:
    var t = -1
    goto *next
;


code = # ( a b -- ? )
    var x f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    cmp $x $t
    goto_if *.true
    var t = 0
    goto *next
    .true:
    var t = -1
    goto *next
;


code > # ( a b -- ? )
    var x f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    gt $x $t
    goto_if *.true
    var t = 0
    goto *next
    .true:
    var t = -1
    goto *next
; 


code < # ( a b -- ? )
    var x f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    lt $x $t
    goto_if *.true
    var t = 0
    goto *next
    .true:
    var t = -1
    goto *next
; 


code and # ( ? ? -- ? )
and:
    var x f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    cmp $t 0
    goto_if *next
    cmp $x 0
    goto_if *.false
    var t = -1
    goto *next
    .false:
    var t = 0
    goto *next
;


( --- Double --- )

: 2dup ( a b -- a b a b ) over over ;

: 2drop ( a b -- ) drop drop ;

: 2swap ( a b c d -- c d a b ) rot >h rot h> ;


( --- Combinators --- )

: { ( -- orig ) ( comp: -- xt )
  here 4 + postpone literal
  0 postpone jump
  here 1 -
  enter-action ,
  0 tco-size ! ; immediate


: } ( orig -- )
  postpone ; ] shadow here swap ! ; immediate


: this ( -- ) ( comp: -- xt )
  unshadow latest postpone literal shadow ; immediate

: recurse ( -- )
  unshadow latest , shadow ; immediate
  

code evaluate # ( A.. addr u -- B.. )
    var r-stack a $source
    var r-stack a $source-size
    var r-stack a $source-id
    var r-stack a $>in
    var rp + 4
    var source f d-stack $dp
    var dp - 1
    var source-size = $t
    var t f d-stack $dp
    var d-stack r $dp 2
    var dp - 1
    var source-id = -1
    var >in = 0
    var x = *quit
#   println "Evaluation setup complete"
    gosub *execute-YSL-action-from-YSL
#   println "Evaluation complete. Cleaning up."
    var >in f r-stack $rp
    var rp - 1
    var source-id f r-stack $rp
    var rp - 1
    var source-size f r-stack $rp
    var rp - 1
    var source f r-stack $rp
    var r-stack r $rp 4
    var rp - 1
    goto *next
;


code execute # ( A.. { A.. -- B.. } -- B.. )
    var w = $t
    var t f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    var x f mem $w
    var w + 1
    goto $x
;


: dip ( A.. x { A.. -- B.. } -- B.. x } )
  swap >r execute r> nop ;

: keep ( A.. x { A.. x -- B.. } -- B.. x } )
  over >r execute r> nop ;


( --- Control flow --- )

code when # ( A.. ? { A.. -- B.. } -- B.. | A.. )
    var x f d-stack $dp
    var dp - 1
    var w = $t
    var t f d-stack $dp
    var d-stack r $dp 2
    var dp - 1
    cmp $x 0
    goto_if *next
    var x f mem $w
    var w + 1
    goto $x
;


: unless ( A.. ? { A.. -- B.. } -- A.. | B.. )
  [ ' 0= ] literal dip when ;


: if ( A.. ? { A.. -- B.. } { A.. -- C.. } -- B.. | C.. )
  rot [ ' swap ] literal unless drop execute ;


: loop ( A.. { A.. -- A.. ? } -- A.. )
  dup dip swap this [ ' drop ] literal if ;


: while ( A.. { A.. -- B.. ? } { B.. -- A.. } -- B.. )
  swap dup >r dip r> rot
  { swap dup >r dip r> recurse }
  [ ' 2drop ] literal if ;


: until ( A.. { A.. -- B.. ? } { B.. -- A.. } -- B.. )
  swap dup >r dip r> rot
  [ ' 2drop ] literal
  { swap dup >r dip r> recurse } if ;


variable times.bound
variable i^

: times ( A.. n { A.. -- A.. } -- A.. )
  swap dup 1 <
  [ ' 2drop ] literal
  { times.bound @ >r i^ @ >r
    dup times.bound ! i^ !
    { dup dip i^ @ 1 - dup i^ ! } loop
    drop r> i^ ! r> times.bound ! } if ;


: i ( -- n )
  times.bound @ i^ @ - ;
    

( --- I/O --- )

code emit # ( c -- )
    print !t
    var t f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    goto *next
;


: cr ( -- ) 10 emit ;
: nl ( -- ) 10 emit ;


: type ( addr n -- )
  { dup @ emit 1 + } times drop ;


: say ( "string" -- )
  state @
  { postpone $ postpone type }
  { parse-string type } if ; immediate


( --- File access --- )

code open-file # ( addr u -- addr u ) ( F: -- f )
open-file:
    var addr f d-stack $dp
    var filename = ""
    lt $t 1
    goto_if *.end
    .loop:
        var x = $addr
        gosub *_@
        var x f return
        var filename a $x
        var t - 1
        var addr + 1
        gt $t 0
        goto_if *.loop
    .end:
    string_array a file-stack !current-file
    var file-stack-pointer + 1
    file.read !filename
    var current-file c return
    size current-file
    var current-file-size f return
    var d-stack s $dp $file-buffer-offset
    var t = $current-file-size
    goto *next
;


code drop-file # ( F: f -- )
drop-file:
    string_array g file-stack $file-stack-pointer
    var current-file f return
    string_array r file-stack $file-stack-pointer
    var file-stack-pointer - 1
    size current-file
    var current-file-size f return
    goto *next
;


: included ( x*i addr u -- x*j )
  open-file evaluate drop-file ;


( --- Confirmation message --- )

say 'Kernel loaded.' nl
