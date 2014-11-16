﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnnotationVizLib.AnnotationService;

namespace AnnotationVizLib
{
    public class MotifDOTView : GraphVizEngine<string>
    {
        static MotifDOTView()
        {
            
        }

        public static MotifDOTView ToDOT(MotifGraph graph, bool IncludeUnlabeled = false)
        {
            string DOT = "";

            MotifDOTView DotGraph = new MotifDOTView();

            DotGraph.AddAttributes(AttributeMaps.StandardGraphDOTAttributes); 
            
            foreach(MotifNode node in graph.Nodes.Values)
            {
                if(node.Key == "Unlabeled" && !IncludeUnlabeled)
                    continue; 

                GraphViewNode<string> DOTNode = DotGraph.addNode(node.Key);
                DOTNode = GraphVizNodeFromMotifNode(DOTNode, node);  
            }

            DotGraph.Attributes.Add("nslimit", Math.Ceiling(Math.Sqrt(graph.Nodes.Count)).ToString());
            DotGraph.Attributes.Add("mclimit", Math.Ceiling(Math.Sqrt(graph.Nodes.Count)).ToString());
            /*
            List<MotifEdge> UniqueEdges = new List<MotifEdge>();
            foreach (MotifEdge edge in graph.Edges)
            {
                bool AddEdge = true; 
                foreach (MotifEdge existingEdge in UniqueEdges)
                {
                    if (existingEdge == edge)
                    {
                        AddEdge = false;
                        existingEdge.Weight = existingEdge.Weight + 1;
                        break;
                    }
                }

                if (AddEdge)
                {
                    UniqueEdges.Add(edge); 
                }
            }
             */

            foreach (MotifEdge edge in graph.Edges.Values)
            {
                if(edge.TargetNodeKey == "Unlabeled" && !IncludeUnlabeled)
                    continue;

                if(edge.SourceNodeKey == "Unlabeled" && !IncludeUnlabeled)
                    continue; 

                GraphViewEdge<string> DOTEdge = GraphVizEdgeFromMotifEdge(DotGraph, graph, edge); 
            }

            DotGraph.createDirectedGraph("Motif"); 
              
            return DotGraph;
        }

        private static GraphViewNode<string> GraphVizNodeFromMotifNode(GraphViewNode<string> DotNode, MotifNode node)
        {
            string label = node.Key;

            DotNode.AddAttributes(AttributeMaps.StandardNodeDOTAttributes);

            DotNode.Attributes.Add("label", node.Key);

            IDictionary<string, string> AttribsForLabel = AttributeMaps.AttribsForLabel(DotNode.label, AttributeMaps.StandardLabelToNodeDOTAppearance);

            if (AttribsForLabel == null)
            {
                if (label.Length > 0)
                    DotNode.Attributes.Add("fillcolor", "grey");
                else
                    DotNode.Attributes.Add("fillcolor", "white");

                DotNode.Attributes.Add("shape", "ellipse");
            }
            else
            {
                DotNode.AddAttributes(AttribsForLabel); 
            }

            string ToolTipStr = node.Structures.Count.ToString() + " " + node.Key + " instances: ";

            bool firstentry = true;
            foreach(Structure s in node.Structures)
            {
                if (!firstentry)
                    ToolTipStr = ToolTipStr + ", ";
                
                firstentry = false; 
                ToolTipStr = ToolTipStr + s.ID.ToString(); 
            } 

            DotNode.Attributes.Add("tooltip", ToolTipStr); 

            return DotNode;
        }

        public static  GraphViewEdge<string> GraphVizEdgeFromMotifEdge(GraphViewEngine<string> DotEngine, MotifGraph graph, MotifEdge edge)
        {
            GraphViewEdge<string> DotEdge = new GraphViewEdge<string>();
            float additionFactor = 1f;
            float mulFactor = 0.5f; 
            //Set the arrow properties
            string color = "black";
            string arrowhead = "";
            string arrowtail = "";
            string tooltip = "";
            string dir = "";
            float arrowsize = additionFactor;
            float pensize = additionFactor;

            dir = "";
            string StoredToolTip = "";

            DotEdge.from = edge.SourceNodeKey;
            DotEdge.to = edge.TargetNodeKey;

            IDictionary<string, string> EdgeAttribs = AttributeMaps.AttribsForLabel(edge.SynapseType.ToUpper(),
                                                                                     AttributeMaps.StandardEdgeLabelToDOTAppearance);

            if (EdgeAttribs == null)
            {
                return null;
            }
            else
            {
                DotEdge.AddAttributes(EdgeAttribs); 
            }
            
            arrowsize = arrowsize * (float)(Math.Sqrt(edge.Weight) * mulFactor);
            if (arrowsize < 1)
                arrowsize = 1;

            pensize = pensize * (float)Math.Sqrt(edge.Weight);

            DotEdge.Attributes.Add("tailclip", "true");
            //DotEdge.Attributes.Add("color", color);
            //DotEdge.edgeAttributes.Add("URL", edgeSections.Substring(0, edgeSections.Length - 1));
            //DotEdge.Attributes.Add("dir", dir);
            //tempEdge.edgeAttributes.Add("samehead", TypeName); 
            //DotEdge.Attributes.Add("arrowhead", arrowhead);
            //DotEdge.Attributes.Add("arrowtail", arrowtail);
            DotEdge.Attributes.Add("arrowsize", arrowsize.ToString());
            
            //tempEdge.edgeAttributes.Add("weight", edge.Strength.ToString());
            //DotEdge.Attributes.Add("penwidth", pensize.ToString());
            
             
            MotifNode SourceNode = graph.Nodes[edge.SourceNodeKey];
            MotifNode TargetNode = graph.Nodes[edge.SourceNodeKey];

            double SourceCoverage = (double)edge.SourceStructIDs.Count / (double)SourceNode.Structures.Count;
            double TargetCoverage = (double)edge.TargetStructIDs.Count / (double)TargetNode.Structures.Count;

            //double Weight = (SourceCoverage * 10.0) + (TargetCoverage * 10.0);
            double Weight = ((SourceCoverage + TargetCoverage) / 2) * 10.0;

            if (Weight < 1)
            {
                Weight = 1; 
            }

            //Give small weights a boost in appearance. 
            if (Weight > 1 && Weight < 2)
            {
                Weight = 2; 
            }

            if (edge.SourceStructIDs.Count == 1 ||
               edge.TargetStructIDs.Count == 1)
            {
                Weight = 1;
            }

            SourceCoverage *= 100;
            TargetCoverage *= 100; 

            string toolTipStr = edge.SourceNodeKey + " connects: " + SourceCoverage.ToString("F0") + "%    " + edge.TargetNodeKey + " contacted: " + TargetCoverage.ToString("F0") + "%";
            DotEdge.Attributes.Add("tooltip", toolTipStr);


            DotEdge.Attributes.Add("penwidth", Weight.ToString());
            DotEdge.Attributes.Add("weight", (Weight * Weight).ToString());

            //If the edge is bidirectional clone it, reverse the direction, and make it invisible to help directional layout algorithms.
            if (DotEdge.Attributes["dir"] == "both")
            {
                GraphViewEdge<string> reverseTempEdge = DotEdge.Clone() as GraphViewEdge<string>;
                reverseTempEdge.to = DotEdge.from;
                reverseTempEdge.from = DotEdge.to;
                reverseTempEdge.Attributes.Add("style", "invis"); //invisible
                  
                DotEngine.addEdge(reverseTempEdge);
            }

            DotEngine.addEdge(DotEdge);  
            return DotEdge;
        }
    }
}
