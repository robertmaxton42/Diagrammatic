namespace Diagrammatic

open FSharp.FGL

//A Diagram is defined by the following grammar:
// <diagram>    ::= <rel> <lhs: term> <rhs: term> | <term>
// <term>       ::= <op> <term list> | <graph_op> <graph list> | <graph>
// <rel>        ::= "=" | "<" | "<=" | ...
// <infix_op>         ::= "∑" | "Π" | "⊕" | ...
// <ext_op>   ::= (derivative circle) | (bang box)  | (parentheses) | ...
// <graph>      ::= Empty | <context> & <graph>
// <context>    ::= (<adj>, <node_id: int>, <vlabel>, <adj>)
// <adj>        ::= (<elabel>, <node_id: int>) list
// <vlabel>     ::= (<node> | <term>)
//
//where <node> and <elabel> are descriptors of atomic nodes and edges respectively
//e.g. red v. green spiders. (Rendering rules?)
//
//We might also call external operators "container operators".

//... For now, we'll settle for just handling "terms" 
//Equality specifically will be handled by a Derivation class
//, that batches the successive steps of a derivation

type InfixOp = InfixOp of string
type ExtOp = ExtOp of string
type Term<'label> = 
    | InfixOp of InfixOp * Term<'label> list 
    | ExtOp of ExtOp * Graph<int, 'label, float> list 
    | Graph of Graph<int, 'label, float>
//type Diagram<'op, 'gop, 'label> = Relation of string * Term * Term | Term of Term