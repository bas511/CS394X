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
    public class ThingyNew : BasePacman
    {
        bool eatEmUp = true;
        bool goGetTheGoddamnPowerPill = false;
        bool makeLikeATreeAndGetOuttaHere = false;
        bool itsAGhost = false;

        int quadrant = 0;

		public ThingyNew() : base("ThingyNew") {
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
                if (getDistance(gs.Pacman.Node.X, ghost.X, gs.Pacman.Node.Y, ghost.Y) < 5/*this number is probably wrong, fyi*/)
                {
                    if (ghost.Entered && !ghost.Chasing)
                    {
                        itsAGhost = true;
                    }
                }
            }

            if (true/*normal or ghosts are about to turn back*/)
            {
                if (!itsAGhost)
                {
                    if (true /*numPellets() < totalPellets (idr the number) * 0.3*/)
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
                //implement greedy strat
                foreach (Node node in gs.Map.Nodes)
                {
                    if (node.Type == Node.NodeType.Pill)
                    {
                        Node.PathInfo path = gs.Pacman.Node.ShortestPath[node.X, node.Y];
                        if (path != null)
                        {
                            return path.Direction;
                        }
                    }
                }
            }

            if (goGetTheGoddamnPowerPill)
            {
                //self explanatory
                foreach (Node node in gs.Map.Nodes)
                {
                    if (node.Type == Node.NodeType.PowerPill)
                    {
                        Node.PathInfo path = gs.Pacman.Node.ShortestPath[node.X, node.Y];
                        if (path != null)
                        {
                            return path.Direction;
                        }
                    }
                }
            }

            return Direction.None;
        }
    }
}
