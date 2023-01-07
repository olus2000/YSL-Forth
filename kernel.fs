: thing ;

code drop # ( x -- )
    var t f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    goto *next
;


code parse # ( c "ccc<char>" -- addr u )
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


( --- Stack shuffling --- )

( drop defined at the top )

code dup # ( x -- x x )
    var d-stack a $t
    var dp + 1
    goto *next
;


code swap # ( x1 x2 -- x2 x1 )
    var x f d-stack $dp
    var d-stack s $dp $t
    var t = $x
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
    var dp - 1
    var mem s $t $x
    var t f d-stack $dp
    var d-stack r $dp 2
    var dp - 1
    goto *next
.mapped:
    lt $t $input-size-offset
    goto_if *.out-of-bounds
    gt $t $input-size-offset
    goto_if *.out-of-bounds
    var t = $>in
    var dp - 1
    var t f d-stack $dp
    var d-stack r $dp 2
    var dp - 1
    goto *next
.out-of-bounds:
    var dp - 1
    var t f d-stack $dp
    var d-stack r $dp 2
    var dp - 1
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


( I tried to implement $ as a Forth word, and failed. )
code $ # ( "<spaces><delim>ccc<delim>" -- )
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
        var mem a $x
        var mp + 1
        goto *.loop
    .end:
        var >in + 1
        var len = $mp
        var len - $hole
        var len - 2
        var mem s $hole $len
        var hole + 2
        var len = $mp
        var len + 1
        var mem s $hole $len
        goto *next
; immediate


code postpone # ( "<spaces>word<space>" -- )
postpone:
    gosub *parse-word
    gosub *_find
    var found f return
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


code literal # ( x -- )
    var mem a $lit-action
    var mem a $t
    var mp + 2
    var t f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    goto *next
; immediate


code jump # ( addr -- )
    var mem a $jump-action
    var mem a $t
    var mp + 2
    var t f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    goto *next
; immediate


code create # ( "<spaces>word<space>" -- )
#   println "Starting create"
    gosub *define
    var mem a *dovar
    var mp + 1
#   println "Finished create"
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


( --- Maths --- )

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


( --- Combinators --- )

: { ( -- orig ) ( comp: -- xt )
  here 4 + postpone literal
  0 postpone jump
  here 1 -
  enter-action , ; immediate


: } ( orig -- )
  postpone exit here swap ! ; immediate
  

code evaluate # ( x*i addr u -- x*j )
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


code execute # ( x*i xt -- x*j )
    var w = $t
    var t f d-stack $dp
    var d-stack r $dp 1
    var dp - 1
    var x f mem $w
    var w + 1
    goto $x
;


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

2137 .
