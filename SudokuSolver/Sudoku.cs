using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{

    delegate bool SearchStrategy(SquareRun run);

    class SudokuGame
    {
        public BoardChangeList ChangeList;
        private SudokuBoard Board;
        private int _changeStep;
        private int _debugStep;

        public SudokuGame()
        {
            Board = new SudokuBoard(new FoundHandler(this.FoundAnswer));
            ChangeList = new BoardChangeList();
        }

        public void Solve()
        {
            Board.Reset();

            _changeStep = 0;
            _debugStep = 0;

            while (FindAnswers()) ;
        }


        private bool FindAnswers()
        {
            _debugStep++;
            Debug.WriteLine("step " + _debugStep);

            //process all board state changes before searching for new changes to make

            if (ChangeList.Count > _changeStep)
            {
                Board.Process(ChangeList[_changeStep]);
                _changeStep++;
                return true;
            }
             
            // check if we're finished

            if (Board.CountPossibles() == 81)
            {
                return false;
            }

            // otherwise search out new board state changes to make

            if (Search(new SearchStrategy(SearchSinglePinInRun)))
                return true;

            if (Search(new SearchStrategy(SearchLockedPairInRun)))
                return true;

            if (SearchBlocks(new SearchStrategy(SearchPinnedRowColumnValues)))
                return true;

            return false;
        }

        private void FoundAnswer(int row, int col, int value)
        {
            // called whenever an answer is found via reduction
            ChangeList.AddKnown(row, col, value);
            Debug.WriteLine("found answer " + value.ToString() + " at (" + row.ToString() + "," + col.ToString() + ")");
        }



        private bool Search(SearchStrategy searchFunc)
        {
            foreach (SquareRun.SquareRunType runtype in Enum.GetValues(typeof(SquareRun.SquareRunType)))
            {
                // there are 9 rows, 9 columns and 9 blocks to process
                for (int i = 0; i < 9; i++)
                {
                    SquareRun run = new SquareRun(Board, runtype, i);
                    if (searchFunc(run))
                        return true;
                }
            }
            return false;
        }

        private bool SearchBlocks(SearchStrategy searchFunc)
        {
            // Process 9 blocks
            for (int i = 0; i < 9; i++)
            {
                SquareRun run = new SquareRun(Board, SquareRun.SquareRunType.BlockRun, i);
                if (searchFunc(run))
                    return true;
            }
            return false;
        }


        // ========================================================================================
        // Search Strategies: http://www.sadmansoftware.com/sudoku/techniques.htm
        // ========================================================================================


        public bool SearchSinglePinInRun(SquareRun square)
        {
            // if a number can only occur in one square in a run and that square isn't already
            // set as a known value, then queue the discovery

            int digitCount;
            PossibleSet single;

            for (int digit = 1; digit < 10; digit++)
            {
                digitCount = 0;
                single = null;

                for (int i = 0; i < 9; i++)
                {
                    if (square[i].Contains(digit))
                    {
                        digitCount++;
                        single = square[i];
                    }
                }

                if (digitCount == 1)
                {
                    if (!single.IsKnownValue())
                    {
                        Debug.WriteLine("SearchSinglePinInRun found " + single.ToDebugString());
                        ChangeList.AddKnown(single.Row, single.Column, digit);
                        //single.SetKnownValueFound(digit);
                        return true;
                    }
                }
            }
            return false;
        }


        // ========================================================================================


        private bool SearchNakedLockedPairInRun(SquareRun square)
        {
            // superceded by SearchLockedPairInRun

            bool progress = false;

            //generate all possible square pairs (i,j)
            for (int i = 0; i < 8; i++)
                for (int j = i + 1; j < 9; j++)
                {
                    //if the two squares have the same pair of numbers
                    if (square[i].Count == 2 && square[j].Count == 2)
                    {
                        if (square[i] == square[j])
                        {
                            // squares i,j are a pair
                            // remove the pair of numbers from the other squares
                            for (int k = 0; k < 9; k++)
                            {
                                if (k != i && k != j)
                                    if (square[k].Remove(square[i]))
                                        progress = true;
                            }
                        }
                    }
                }
            return progress;
        }

        private bool SearchLockedPairInRun(SquareRun square)
        {
            // search for any naked or hidden pairs

            bool progress = false;
            int[] tally = new int[10];
            int[,] offset = new int[10, 9];

            for (int digit = 1; digit < 10; digit++)
            {
                // count the number of times a digit shows up
                // and track which square they're in

                tally[digit] = 0;
                for (int i = 0; i < 9; i++)
                {
                    if (square[i].Contains(digit))
                    {
                        int foundSquare = tally[digit];
                        tally[digit]++;
                        offset[digit, foundSquare] = i;
                    }
                }
            }

            //generate all possible digit pairs (i,j)
            for (int i = 1; i < 9; i++)
                for (int j = i + 1; j < 10; j++)
                {
                    //check whether each digit shows up only twice
                    if (tally[i] == 2 && tally[j] == 2)
                    {
                        // and they show up in the same squares
                        if (offset[i, 0] == offset[j, 0] && offset[i, 1] == offset[j, 1])
                        {
                            // then we have found two locked pairs
                            int sq1 = offset[i, 0];
                            int sq2 = offset[i, 1];

                            // reduce hidden pairs to naked pairs
                            if (square[sq1].Count > 2)
                            {
                                square[sq1].SetPair(i, j);
                                Debug.WriteLine("Hidden Pair found " + square[sq1].ToDebugString());
                                progress = true;
                            }
                            if (square[sq2].Count > 2)
                            {
                                square[sq2].SetPair(i, j);
                                Debug.WriteLine("Hidden Pair found " + square[sq2].ToDebugString());
                                progress = true;
                            }

                            // reduce other squares in run with the naked square values
                            for (int k = 0; k < 9; k++)
                            {
                                if (k != sq1 && k != sq2)
                                    if (square[k].Remove(square[sq1]))
                                    {
                                        progress = true;
                                        Debug.WriteLine("Pair reduction " + square[k].ToDebugString() + " reduced by " + square[1].ToDebugString());
                                    }
                            }

                            if (progress)
                                return progress;
                        }
                    }
                }
            return progress;
        }

        // ========================================================================================


        public bool SearchPinnedRowColumnValues(SquareRun block)
        {
            // a subrun is a row or column within a single block
            // for each subrun in a block, union the possible sets
            // subtract the remaining possible sets that are outside the subrun but inside the block
            // anything remaining is unique to that subrun and can be removed from the run outside the block

            // calculation datastructure: first 3 members are the subrun squares to union, next 6 are remaining squares to subtract
            int[][] set = new int[6][] { new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                                            new int[] { 3, 4, 5, 0, 1, 2, 6, 7, 8 },
                                            new int[] { 6, 7, 8, 0, 1, 2, 3, 4, 5 },
                                            new int[] { 0, 3, 6, 1, 4, 7, 2, 5, 8 },
                                            new int[] { 1, 4, 7, 0, 3, 6, 2, 5, 8 },
                                            new int[] { 2, 5, 8, 0, 3, 6, 1, 4, 7 }};


            bool progress = false;

            for (int subrun = 0; subrun < 6; subrun++)
            {
                // calculate the unique union
                PossibleSet unique = new PossibleSet(0, 0, null);
                PossibleSet sq;

                for (int offset = 0; offset < 9; offset++)
                {
                    if (offset < 3)
                    {
                        sq = block[set[subrun][offset]];
                        unique.Add(sq);
                    }
                    else
                    {
                        sq = block[set[subrun][offset]];
                        unique.Remove(sq);
                    }
                }
                // now remove this set (if not empty) from the run outside of this block
                if (unique.Count > 0)
                {
                    PossibleSet square;
                    int[] include = new int[3];
                    include[0] = set[subrun][0];
                    include[1] = set[subrun][1];
                    include[2] = set[subrun][2];
                    SquareRun run = block.GetRun(include);
                    run.ExcludeBlock(block);
                    for (int j = 0; j < 9; j++)
                    {
                        square = run[j];
                        if (square != null)
                            if (square.Remove(unique))
                            {
                                Debug.WriteLine("Run " + run.Position + " Pinned Row/Column reduction " + square.ToDebugString() + " removed " + unique.ToString());
                                progress = true;
                            }
                    }
                }

                if (progress)
                    return progress;
            }

            return progress;
        }
    }
}
