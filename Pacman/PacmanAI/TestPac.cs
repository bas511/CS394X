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
    public class TestPac : BasePacman
    {
        bool eatEmUp = true;
        bool goGetTheGoddamnPowerPill = false;
        bool makeLikeATreeAndGetOuttaHere = false;
        bool itsAGhost = false;

        int quadrant = 0;
        int q1TotalPellets, q2TotalPellets, q3TotalPellets, q4TotalPellets;

        // MAGIC NUMBERS
        int TIME_THRESHOLD = 400;
        double PERSONAL_SPACE = 200; // I have no idea if this is even a value
        double COMPLETION_PERCENTAGE = 5;

        private const bool debug = false;

        public TestPac()
            : base("TestPac")
        {
        }

        public bool isInDeadZone(Node spot)
        {
            bool dead = false;
            if (spot.Y < 23 && spot.Y > 8)
            {
                if (spot.Y < 18 && spot.Y > 9)
                {
                    if (spot.X > 8 && spot.X < 19) { return true; }
                }
                else
                {
                    if (spot.X > 3 && spot.X < 24) { return true; }
                }
            }

            return dead;
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
            for (int xpos = gs.Pacman.Node.CenterX * Math.Abs(1 - leftSide); xpos < 28 - gs.Pacman.Node.CenterX * leftSide; xpos++)
            {
                //Make starting ypos equal to 0 if on bottom half and equal to the midpoint if on top
                //run until ypos equals center if on bottom half and top edge if on top
                for (int ypos = gs.Pacman.Node.CenterY * Math.Abs(1 - bottomSide); ypos < 28 - gs.Pacman.Node.CenterY * bottomSide; ypos++)
                {
                    //Check each node and add it to the total pellet count if need be
                    Node isItAPill = null;
                    isItAPill = gs.Map.GetNode(xpos, ypos);
                    if (isItAPill.Type == Node.NodeType.Pill)
                        numPellets++;
                }
            }
            Console.WriteLine(numPellets);
            return numPellets;

        }

        public void initTotalPellets(GameState gs)
        {
            int numPellets = 0;
            // debugging I found some weird values for centerX and centerY. I'm hard-coding
            // the values into the loop now, but we can change the values later

            // using quadrant assignments: 1-lowerleft, 2-lowerright, 3-upperleft, 4-upperright
            // coordinates are RIGHT-DOWN

            // quadrant 1
            for (int x = 0; x < gs.Pacman.Node.CenterX; x++)
            {
                for (int y = gs.Pacman.Node.CenterY; y < 31; y++)
                {
                    Node isItAPill = null;
                    isItAPill = gs.Map.GetNode(x, y);
                    if (isItAPill.Type == Node.NodeType.Pill)
                        numPellets++;
                }
            }
            q1TotalPellets = numPellets;
            numPellets = 0;

            // quadrant 2
            for (int x = gs.Pacman.Node.CenterX; x < 28; x++)
            {
                for (int y = gs.Pacman.Node.CenterY; y < 31; y++)
                {
                    Node isItAPill = null;
                    isItAPill = gs.Map.GetNode(x, y);
                    if (isItAPill.Type == Node.NodeType.Pill)
                        numPellets++;
                }
            }
            q2TotalPellets = numPellets;
            numPellets = 0;

            // quadrant 3
            for (int x = 0; x < gs.Pacman.Node.CenterX; x++)
            {
                for (int y = 0; y < gs.Pacman.Node.CenterY; y++)
                {
                    Node isItAPill = null;
                    isItAPill = gs.Map.GetNode(x, y);
                    if (isItAPill.Type == Node.NodeType.Pill)
                        numPellets++;
                }
            }
            q3TotalPellets = numPellets;
            numPellets = 0;

            // quadrant 4
            for (int x = gs.Pacman.Node.CenterX; x < 28; x++)
            {
                for (int y = 0; y < gs.Pacman.Node.CenterY; y++)
                {
                    Node isItAPill = null;
                    isItAPill = gs.Map.GetNode(x, y);
                    if (isItAPill.Type == Node.NodeType.Pill)
                        numPellets++;
                }
            }
            q4TotalPellets = numPellets;
            numPellets = 0;

        }

        public int getAreaTotalPellets()
        {
            if (quadrant == 1) { return q1TotalPellets; }
            else if (quadrant == 2) { return q2TotalPellets; }
            else if (quadrant == 3) { return q3TotalPellets; }
            else { return q4TotalPellets; }

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

            if (gs.Timer < 100) // may have to change this if the function can't work in time
            {
                initTotalPellets(gs);
            }

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
                    if (ghost.Entered && ghost.Chasing)
                    {
                        itsAGhost = true;
                    }
                }
            }

            if (areThingsNormal(gs))
            {
                if (!itsAGhost)
                {
                    if (getNumPellets(gs) < (getAreaTotalPellets() * (1 - COMPLETION_PERCENTAGE))) // completion threshold reached
                    {
                        eatEmUp = false;
                        goGetTheGoddamnPowerPill = true;
                    }
                    if (getNumPellets(gs) == 0)
                    {
                        makeLikeATreeAndGetOuttaHere = true;
                    }
                }
                else
                    makeLikeATreeAndGetOuttaHere = true;
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
