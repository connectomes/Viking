﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConnectomeDataModel;
using System.Collections.Generic;

namespace ConnectomeDataModelTests
{
    [TestClass]
    public class TestConnectomeDataModel
    {
        [TestMethod]
        public void TestSelectNetworkStructureIDs()
        {
            ConnectomeDataModel.ConnectomeEntities context = new ConnectomeEntities();

            List<long> IDs = new List<long>(new long[] { 476, 514 });
            SortedSet<long> networkStructureIDs = context.SelectNetworkStructureIDs(IDs, 3);
            System.Diagnostics.Trace.Write(networkStructureIDs.Count().ToString());

        }

        [TestMethod]
        public void TestSelectNetworkStructures()
        {
            ConnectomeDataModel.ConnectomeEntities context = new ConnectomeEntities();

            List<long> IDs = new List<long>(new long[] { 476, 514 });
            IQueryable<Structure> networkStructureIDs = context.SelectNetworkStructures(IDs, 3);
            System.Diagnostics.Trace.Write(networkStructureIDs.Count().ToString()); 
        }

        [TestMethod]
        public void TestSelectNetworkChildStructures()
        {
            ConnectomeDataModel.ConnectomeEntities context = new ConnectomeEntities();
            context.ConfigureAsReadOnly();

            context.Database.CommandTimeout = 120;

            List<long> IDs = new List<long>(new long[] { 476, 514 });
            IQueryable<Structure> networkStructureIDs = context.SelectNetworkChildStructures(IDs, 3);
            System.Diagnostics.Trace.Write(networkStructureIDs.Count().ToString());
        }

        [TestMethod]
        public void TestSelectNetworkStructureLinks()
        {
            ConnectomeDataModel.ConnectomeEntities context = new ConnectomeEntities();

            List<long> IDs = new List<long>(new long[] { 476, 514 });
            IQueryable<StructureLink> networkStructureLinks = context.SelectNetworkStructureLinks(IDs, 3);
            System.Diagnostics.Trace.Write(networkStructureLinks.Count().ToString());
        }

        [TestMethod]
        public void TestSelectNetworkDetails()
        {
            ConnectomeDataModel.ConnectomeEntities context = new ConnectomeEntities();

            List<long> IDs = new List<long>(new long[] { 476, 514 });
            NetworkDetails network = context.SelectNetworkDetails(IDs, 2);

            Console.WriteLine($"Nodes = {network.Nodes.Length}");
            Console.WriteLine($"Children = {network.ChildNodes.Length}");
            Console.WriteLine($"Edges = {network.Edges.Length}");
            Console.WriteLine("");

            
            Console.WriteLine($"Nodes = {network.Nodes[network.Nodes.Length-1].Label}");
            Console.WriteLine($"Children = {network.ChildNodes[network.ChildNodes.Length - 1].ID}");
            Console.WriteLine($"Edges = {network.Edges[network.Edges.Length-1].Bidirectional}"); 
        }
    }
}
