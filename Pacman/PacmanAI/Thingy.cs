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
    public class Thingy : BasePacman
    {
        public Thingy()
            : base("Thingy")
        {
        }

        public int getNumPellets(GameState gs)
        {
            int numPellets = 0;
            int quadrant;
            bool isRight = (gs.Pacman.Node.X >= gs.Pacman.Node.CenterX);
            bool isOver = (gs.Pacman.Node.Y >= gs.Pacman.Node.CenterY);

            if (isRight && isOver) { quadrant = 1; }
            else if (!isRight && isOver) { quadrant = 2; }
            else if (!isRight && isOver) { quadrant = 3; }
            else { quadrant = 4; } // isRight && !isOver

            for (int xpos = gs.Pacman.Node.CenterX * Math.Abs(1 - leftSide); xpos < 28 - gs.Pacman.Node.CenterX * leftSide; xpos++)
            {
                for (int ypos = gs.Pacman.Node.CenterY * Math.Abs(1 - topSide); ypos < 28 - gs.Pacman.Node.CenterY * topSide; ypos++)
                {
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
            bool eatEmUp = true;
            bool goGetTheGoddamnPowerPill = false;

            foreach (Pacman.Simulator.Ghosts.Ghost ghost in gs.Ghosts)
            {
                if (getDistance(gs.Pacman.Node.X, ghost.X, gs.Pacman.Node.Y, ghost.Y) > 10/*this number is probably wrong, fyi*/)
                {
                    if (ghost.Chasing)
                        eatEmUp = false;
                }
                if (true /*numPellets() < totalPellets (idr the number) * 0.3*/)
                {
                    eatEmUp = false;
                    goGetTheGoddamnPowerPill = true;
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
