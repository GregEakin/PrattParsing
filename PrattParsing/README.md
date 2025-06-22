# Top Down Operator Precedence

[Original paper by Vaughan R. Pratt](https://dl.acm.org/doi/pdf/10.1145/512927.512931)
[Pratt Parsing video](https://www.youtube.com/watch?v=2l1Si4gSb9A)
[Wikipedia article](https://en.wikipedia.org/wiki/Operator-precedence_parser#Pratt_parsing)]

```
    let lex str =
      tokens := [];
      let lbuf = Lexing.from_string str in
      let rec go () =
        let tok = lexer.tokenise lbuf in
        tokens := tok :: !tokens;
        match tok with
        | EOF -> ()
        | _ -> go () in
      in
      go ();
      tokens := List.ref !tokens

    let current () = List.hd !tokens
    let advance () = tokens := List.tl !tokens

    // open Expr

    let lbp : Token.t -> int = function
      | IDENT _ | LPAR | INT _ | RPAR -> 0
      | ADD | SUB -> 2
      | MUL | DIV -> 3
      | EXP -> 4
      | EOF -> (-1)

    let rec expr limit =
      let first = current () in 
      advance ();
      let left = ref (nud first) in 
      while lbp (current ()) > limit do
        let next = current () in
        advance ();
        left := led !left next
      done;
      !left
    and nud = function
      | IDENT x -> Var x 
      | INT i => Int i
      | LPAR ->
         let e = expr 0 in
         (match current() with
          | RPAR -> advance (); e
          | _ -> failwith "Expected closing parenthesis")
      | t -> failwith (Printf.sprintf "No nud for %s" (Token.show t))
    and led = function
      | ADD -> Bop (Add, left, expr 2)
      | SUB -> Bop (Sub, left, expr 2)
      | MUL -> Bop (Mul, left, expr 3)
      | DIV -> Bop (Div, left, expr 3)
      | EXP -> Bop (Exp, left, expr 5)
      | t -> failwith (Printf.sprintf "No led for %s" (Token.show t))

    let rec parse_expr str = 
      lex str;
      print_endline (String.concat "," (List.map Token.show !tokens));
      expr 0
```