using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Int_Node : ActionNode
{
    public override Node GetNodeRepresentation()
    {
        return new Node
        {
            title = "Int",
 


        };
    }

}
