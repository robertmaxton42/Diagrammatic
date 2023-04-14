namespace Diagrammatic.Core

open FSharp.FGL
open Aether
open Aether.Operators

(*  
    A Diagram<'label> is either a relation between terms, or a bare term, with labels of 
    type 'label provided by the user.
    A Term<'label> is a Graph<Node, 'label, Edge> 
    A Node is pair of an id and either an OpNode (a graph-level operation of some kind, which may 
    be something fancy like a derivative circle or a bang box, or just a ∑ in parentheses or the
    like) or a SimpleNode (which is just a bare node with a string saying what kind it is, say 
    "red" or "three-vector"). 

    Note that OpNodes are asymmetric; an OpNode with more than one leg will in general not be 
    equivalent if you attach to a different leg. But then, that's true of tensors in general...

    Alternatively, we could have OpNodes just contain other nodes directly, rather than containing
    entire subgraphs that need to be correctly wired up. The problem with this is that sometimes
    OpNodes should interfere with graphical identites -- derivatives, for example, but even just
    "+" can do this. If I have c . (a + b) and an identity for c . x, x has to match both a and
    b to use that identity. The latter case has the doubled problem that it essentially introduces
    multiedges *anyway*.

    ...It seems like we'll have to have logical-nodes contain vertices in general, unless there's a
    better way to implement asymmetric legs than "groups of disconnected subnodes".

    An Edge is always a SimpleEdge for now, until and unless something fancy comes up.

    ... For now, we'll settle for just handling "terms" 
    Equality specifically will be handled by a Derivation class, that batches the successive steps
     of a derivation
*)



type EdgeData = SimpleEdge of string
type DiagramEdge<'label when 'label: equality> = 
    struct
        val data: EdgeData
        val label: 'label ref

        new (data, label) = {data = data; label = label}
    end


[<CustomEquality>]
[<CustomComparison>]
type Node<'p, 'e when 'e: equality> = 
    struct
        val id: int
        val kind: NodeKind<'p, 'e>
        val ports: PortGroup<'p, 'e> seq
        new (id, kind, pempty) = {id = id; kind = kind; ports = pempty}
        //pempty should be a collection of collections, each representing a group of ports
        //fixed-size collections represent groups with an undetermined number of ports 
        //variable-size collections represent groups with a fixed number of ports
        //for example, a Z-spider has a single variable-size collection because it has arbitrary arity
        //a derivative operator w.r.t. a vector v would have one variable-size collection -- arbitrary 
        //number of connections into whatever it's operating on -- and one fixed-size collection 
        //of size one -- connecting to v

        //Two nodes are the same if their ids are the same.
        override this.Equals(other) = 
            match other with
            | :? Node<'p, 'e> as n -> n.id = this.id
            | _ -> false
        
        member this.CompareTo(other: obj) =
            match other with
            | :? Node<'p, 'e> as n -> this.id.CompareTo(n.id)
            | _ -> raise (System.ArgumentException("Object must be of type Node"))

        override this.GetHashCode() = this.id.GetHashCode()

        interface System.IComparable with
            member this.CompareTo(other) = this.CompareTo(other)
    end
and NodeKind<'p, 'e when 'e: equality> = OpNode of Op<'p, 'e> | SimpleNode of string
and Op<'p, 'e when 'e: equality> = {op: string; subg: Graph<Port<'p, 'e>, 'p, DiagramEdge<'e>>}
and Port<'p, 'e when 'e: equality> = 
    { parent: Node<'p, 'e> option; group: int; id: int }
    static member parent_: Prism<Port<'p, 'e>, _> =
        (fun p -> p.parent), (fun n p -> {p with parent = Some n})
and LabeledPort<'p, 'e when 'e: equality> = LVertex<Port<'p, 'e>, 'p ref>
and PortGroup<'p, 'e when 'e: equality> =
    | FixedGroup of LabeledPort<'p, 'e> array 
    | MutableGroup of LabeledPort<'p, 'e> list
    
    static member map f = function
        | FixedGroup arr -> FixedGroup (Array.map f arr)
        | MutableGroup l -> MutableGroup (List.map f l)

    interface System.Collections.Generic.IEnumerable<LabeledPort<'p,'e>> with
        member this.GetEnumerator() =
            match this with
            | FixedGroup arr -> (arr :> System.Collections.Generic.IEnumerable<_>).GetEnumerator()
            | MutableGroup l -> (List.toSeq l).GetEnumerator()

    interface System.Collections.IEnumerable with
        member this.GetEnumerator() =
            (this :> System.Collections.Generic.IEnumerable<LabeledPort<'p,'e>>).GetEnumerator()

//port_structure should be a collection of groups -- either an array of ports of the intended 
// fixed size or a list with the default/starting number of ports. `parent` should be set to None.
type NodeTemplate<'p, 'e when 'e: equality>(kind: NodeKind<'p, 'e>, port_structure: PortGroup<'p, 'e> seq) =
    member this.kind = kind 
    member this.port_structure = port_structure

    member this.coinNode id = 
        let parentLens = fst_ >-> Port.parent_
        let rec out = 
            Node<'p, 'e>(id, kind, 
                seq {
                    for group in port_structure ->
                        PortGroup<_,_>.map (out ^= parentLens) group
                }
            )
        out

[<CustomEquality>]
[<CustomComparison>]
type LabeledNode<'n, 'p, 'e when 'e: equality> = 
    | LabeledNode of node: Node<'p, 'e> * label: 'n ref

    //Labeled nodes are the same if the nodes (not the labels) are the same.
    override this.Equals(other) =
        let (LabeledNode(thisn, _)) = this
        match other with
        | :? LabeledNode<'n, 'p, 'e> as ln -> 
            let (LabeledNode(thatn, _)) = ln
            thisn = thatn
        | _ -> false

    member this.CompareTo(other: obj) =
        let (LabeledNode(thisn, _)) = this
        match other with
        | :? LabeledNode<'n, 'p, 'e> as ln -> 
            let (LabeledNode(thatn, _)) = ln
            thisn.CompareTo(thatn)
        | _ -> raise (System.ArgumentException("Object must be of type LabeledNode"))

    override this.GetHashCode() = 
        let (LabeledNode(thisn, _)) = this
        thisn.GetHashCode()

    interface System.IComparable with
        member this.CompareTo(other) = this.CompareTo(other)


type Term<'n, 'p, 'e when 'e: equality> (nodes: LabeledNode<'n, 'p, 'e> list, graph: Graph<Port<'p, 'e>, 'p ref, DiagramEdge<'e>>)= 
    member this.Nodes = nodes
        //and private set n = nodes <- n 
    member this.Graph = graph
        //and private set g = graph <- g

    member this.addNode (node: LabeledNode<'n, 'p, 'e>) =
        let (LabeledNode (n, _)) = node
        new Term<'n, 'p, 'e>(
            node::nodes, 
            Vertices.addMany (List.ofSeq <| Seq.concat n.ports) graph
        )

    member this.removeNode (node: LabeledNode<'n, 'p, 'e>) =
        let (LabeledNode (n, _)) = node
        new Term<'n, 'p, 'e>( 
            List.filter ((<>) node) nodes,
            Vertices.removeMany (
                List.ofSeq <| seq {
                        for port, _ in Seq.concat n.ports -> port
                }) graph
        )

(*
[<CustomEquality>]
[<CustomComparison>] 
type Node<'n, 'e> =
    struct 
        val id: int
        val data: NodeData<'n, 'e>
        new (id, data) = {id = id; data= data}

        //This is super nonintuitive, but it's not clear what FGL uses its comparability for, so
        override this.Equals(other) = 
            match other with
            | :? Node<'n, 'e> as n -> n.id = this.id
            | _ -> false
        
        member this.CompareTo(other: obj) =
            match other with
            | :? Node<'n, 'e> as n -> this.id.CompareTo(n.id)
            | _ -> raise (System.ArgumentException("Object must be of type Node"))

        override this.GetHashCode() = this.id.GetHashCode()

        interface System.IComparable with
            member this.CompareTo(other) = this.CompareTo(other)
    end
and NodeData<'n, 'e> = OpNode of Op<'n, 'e> | SimpleNode of string
and Op<'n, 'e> = {op: string; subg: Graph<Node<'n, 'e>, 'n, Edge<'e>>}
type Term<'n, 'e> = Graph<Node<'n, 'e>, 'n, Edge<'e>>
//type Diagram<'op, 'gop, 'label> = Relation of string * Term * Term | Term of Term




//The following is the old, erroneous definition
//It is kept in case it is someday useful

// <term>       ::= <infix_op> <term list> | <ext_op> <graph list> | <graph>
// <rel>        ::= "=" | "<" | "<=" | ...
// <infix_op>         ::= "∑" | "Π" | "⊕" | ...
// <ext_op>   ::= (derivative circle) | (bang box)  | (parentheses) | ...
// <graph>      ::= Empty | <context> & <graph>
// <context>    ::= (<adj>, <node_id: int>, <vlabel>, <adj>)
// <adj>        ::= (<elabel>, <node_id: int>) list
// <vlabel>     ::= (<node> | <term>)
//*)