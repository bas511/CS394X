using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Pacman.Simulator;
using Pacman.Simulator.Ghosts;


namespace PacmanAI
{
    public class fourfifty : BasePacman
    {
        bool eatEmUp = true;
        bool goGetTheGoddamnPowerPill = false;
        bool makeLikeATreeAndGetOuttaHere = false;
        bool itsAGhost = false;

        int quadrant = 0;

        // MAGIC NUMBERS
        int TIME_THRESHOLD = 400;
        double PERSONAL_SPACE = 5; // I have no idea if this is even a value
        double COMPLETION_PERCENTAGE = 0.25;


		public fourfifty() : base("fourfifty") {
		}

        public int getNumPellets(GameState gs)
        {
            int numPellets = 0;
            int leftSide = 0;
            int bottomSide = 0;

            if (gs.Pacman.Node.X < gs.Pacman.Node.CenterX)
                leftSide = 1;

            if (gs.Pacman.Node.Y < gs.Pacman.Node.CenterY)
                bottomSide = 1;

            //Make starting xpos equal to 0 if on left side and equal to the midpoint if on right
            //run until xpos equals center if on left side and right edge if on right
            for (int xpos = gs.Pacman.Node.CenterX * Math.Abs(1 - leftSide); xpos < 28 - gs.Pacman.Node.CenterX * leftSide; xpos++) {
                //Make starting ypos equal to 0 if on bottom half and equal to the midpoint if on top
                //run until ypos equals center if on bottom half and top edge if on top
                for (int ypos = gs.Pacman.Node.CenterY * Math.Abs(1 - bottomSide); ypos < 28 - gs.Pacman.Node.CenterY * bottomSide; ypos++) {
                    //Check each node and add it to the total pellet count if need be
                    Node isItAPill = null;
                    isItAPill = gs.Map.GetNode(xpos, ypos);
                    if (isItAPill.Type == Node.NodeType.Pill)
                        numPellets++;
                }
            }

            return numPellets;
        }

        public double getDistance(int x1, int x2, int y1, int y2)
        {
            return Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
        }
        
        public bool areThingsNormal(GameState gs)
        {
            bool normal = true;
            foreach (Pacman.Simulator.Ghosts.Ghost ghost in gs.Ghosts)
            {
                if (ghost.Chasing && ghost.Entered && ghost.RemainingFlee > TIME_THRESHOLD)
                {
                    normal = false;
                }
            }
            return normal;
        }

        public override Direction Think(GameState gs)
        {
            quadrant = 0;

            if (gs.Pacman.Node.X < gs.Pacman.Node.CenterX)
                quadrant += 0;
            else
                quadrant += 1;

            if (gs.Pacman.Node.Y < gs.Pacman.Node.CenterY)
                quadrant += 1;
            else
                quadrant += 3;

            //This determines if there are any ghosts within pacman's space
            foreach (Pacman.Simulator.Ghosts.Ghost ghost in gs.Ghosts)
            {
                if (getDistance(gs.Pacman.Node.X, ghost.X, gs.Pacman.Node.Y, ghost.Y) < PERSONAL_SPACE)
                {
                    if (ghost.Entered && !ghost.Chasing)
                    {
                        itsAGhost = true;
                    }
                }
            }

            if (areThingsNormal(gs))
            {
                if (!itsAGhost)
                {
                    if (true) //*numPellets() < totalPellets * COMPLETION_PERCENTAGE)
                    {
                        eatEmUp = false;
                        goGetTheGoddamnPowerPill = true;
                    }
                    if (getNumPellets(gs) == 0)
                    {
                        makeLikeATreeAndGetOuttaHere = true;
                    }
                }
            }

            if (eatEmUp)
            {
                Node.PathInfo bestPath = null;
                double closest = Double.PositiveInfinity;
                //implement greedy strat
                foreach (Node node in gs.Map.Nodes)
                {
                    if (node.Type == Node.NodeType.Pill)
                    {
                        if (getDistance(gs.Pacman.Node.X, node.X, gs.Pacman.Node.Y, node.Y) < closest)
                        {
                            bestPath = gs.Pacman.Node.ShortestPath[node.X, node.Y];
                            closest = getDistance(gs.Pacman.Node.X, node.X, gs.Pacman.Node.Y, node.Y);
                        }
                    }
                }
                return bestPath.Direction;
            }

            if (goGetTheGoddamnPowerPill)
            {
                Double closestPower = Double.PositiveInfinity;
                Node.PathInfo bestPower = null;
                //self explanatory
                foreach (Node node in gs.Map.Nodes)
                {
                    if (node.Type == Node.NodeType.PowerPill)
                    {
                        if (getDistance(gs.Pacman.Node.X, node.X, gs.Pacman.Node.Y, node.Y) < closestPower)
                        {
                            bestPower = gs.Pacman.Node.ShortestPath[node.X, node.Y];
                            closestPower = getDistance(gs.Pacman.Node.X, node.X, gs.Pacman.Node.Y, node.Y);
                        }
                    }
                }
                return bestPower.Direction;
            }

            return Direction.None;
        }
    }
}
