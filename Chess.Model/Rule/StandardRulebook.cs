//-----------------------------------------------------------------------
// <copyright file="StandardRulebook.cs">
//     Copyright (c) Michael Szvetits. All rights reserved.
// </copyright>
// <author>Michael Szvetits</author>
//-----------------------------------------------------------------------
namespace Chess.Model.Rule
{
    using Chess.Model.Command;
    using Chess.Model.Data;
    using Chess.Model.Game;
    using Chess.Model.Piece;
    using Chess.Model.Visitor;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    /// <summary>
    /// Represents the standard chess rulebook.
    /// </summary>
    public class StandardRulebook : IRulebook
    {
        /// <summary>
        /// Represents the check rule of a standard chess game.
        /// </summary>
        private readonly CheckRule checkRule;

        /// <summary>
        /// Represents the end rule of a standard chess game.
        /// </summary>
        private readonly EndRule endRule;

        /// <summary>
        /// Represents the movement rule of a standard chess game.
        /// </summary>
        private readonly MovementRule movementRule;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardRulebook"/> class.
        /// </summary>
        public StandardRulebook()
        {
            var threatAnalyzer = new ThreatAnalyzer();
            var castlingRule = new CastlingRule(threatAnalyzer);
            var enPassantRule = new EnPassantRule();
            var promotionRule = new PromotionRule();

            this.checkRule = new CheckRule(threatAnalyzer);
            this.movementRule = new MovementRule(castlingRule, enPassantRule, promotionRule, threatAnalyzer);
            this.endRule = new EndRule(this.checkRule, this.movementRule);
        }

        /// <summary>
        /// Creates a new chess game according to the standard rulebook.
        /// </summary>
        /// <returns>The newly created chess game.</returns>
        public ChessGame CreateGame()
        {
            IEnumerable<PlacedPiece> makeBaseLine(int row, Color color)
            {
                yield return new PlacedPiece(new Position(row, 0), new Rook(color));
                yield return new PlacedPiece(new Position(row, 1), new Knight(color));
                yield return new PlacedPiece(new Position(row, 2), new Bishop(color));
                yield return new PlacedPiece(new Position(row, 3), new Queen(color));
                yield return new PlacedPiece(new Position(row, 4), new King(color));
                yield return new PlacedPiece(new Position(row, 5), new Bishop(color));
                yield return new PlacedPiece(new Position(row, 6), new Knight(color));
                yield return new PlacedPiece(new Position(row, 7), new Rook(color));
            }

            IEnumerable<PlacedPiece> makePawns(int row, Color color) =>
                Enumerable.Range(0, 8).Select(
                    i => new PlacedPiece(new Position(row, i), new Pawn(color))
                );

            IImmutableDictionary<Position, ChessPiece> makePieces(int pawnRow, int baseRow, Color color)
            {
                var pawns = makePawns(pawnRow, color);
                var baseLine = makeBaseLine(baseRow, color);
                var pieces = baseLine.Union(pawns);
                var empty = ImmutableSortedDictionary.Create<Position, ChessPiece>(PositionComparer.DefaultComparer);
                return pieces.Aggregate(empty, (s, p) => s.Add(p.Position, p.Piece));
            }

            var whitePlayer = new Player(Color.White);
            var whitePieces = makePieces(1, 0, Color.White);
            var blackPlayer = new Player(Color.Black);
            var blackPieces = makePieces(6, 7, Color.Black);
            var board = new Board(whitePieces.AddRange(blackPieces));

            return new ChessGame(board, whitePlayer, blackPlayer);
        }
        public ChessGame CreateGame2()
        {
            IEnumerable<PlacedPiece> makeBaseLine(int row, Color color)
            {
                yield return new PlacedPiece(new Position(row, 1), new Knight(color));
                yield return new PlacedPiece(new Position(row, 2), new Knight(color));
                yield return new PlacedPiece(new Position(row, 4), new King(color));
                yield return new PlacedPiece(new Position(row, 5), new Knight(color));
                yield return new PlacedPiece(new Position(row, 6), new Knight(color));
            }
            IEnumerable<PlacedPiece> makeBaseLine2(int row, Color color)
            {              
                yield return new PlacedPiece(new Position(row, 4), new King(color));
            }
            IEnumerable<PlacedPiece> makePawnsBlack(int row, Color color) {
                yield return new PlacedPiece(new Position(row, 4), new Pawn(color));
            }
            IEnumerable<PlacedPiece> makePawnsWhite(int row, Color color) =>
                Enumerable.Range(0, 8).Select(
                    i => new PlacedPiece(new Position(row, i), new Pawn(color))
            );;
            IImmutableDictionary<Position, ChessPiece> makePieces(int pawnRow, int baseRow, Color color)
            {
               
                if(color == Color.White)
                {
                    var pawnsDun = makePawnsWhite(pawnRow, color);
                    var baseLineDun = makeBaseLine2(baseRow, color);
                    var piecesDun = baseLineDun.Union(pawnsDun);
                    var emptyDun = ImmutableSortedDictionary.Create<Position, ChessPiece>(PositionComparer.DefaultComparer);
                    return piecesDun.Aggregate(emptyDun, (s, p) => s.Add(p.Position, p.Piece));
                }
                var pawns = makePawnsBlack(pawnRow, color);
                var baseLine = makeBaseLine(baseRow, color);
                var pieces = baseLine.Union(pawns);
                var empty = ImmutableSortedDictionary.Create<Position, ChessPiece>(PositionComparer.DefaultComparer);
                return pieces.Aggregate(empty, (s, p) => s.Add(p.Position, p.Piece));
            }

            var whitePlayer = new Player(Color.White);
            var whitePieces = makePieces(1, 0, Color.White);
            var blackPlayer = new Player(Color.Black);
            var blackPieces = makePieces(6, 7, Color.Black);
            var board = new Board(whitePieces.AddRange(blackPieces));

            return new ChessGame(board, whitePlayer, blackPlayer);
        }
        private class MyPiece
        {
            public int Number1 { get; set; }
            public int? Number2 { get; set; }
        }
        private Dictionary<string, MyPiece> populateCondition()
        {
            Random random = new();
            ///set random positions for one rook and bishop to allow for easier process later
            ///by setting the bishop early, i can determine what other position is open for the other bishop without conflicts
            List<int> storage = new() { 4 };
            int r1 = random.Next(0, 3);
            int b1;
            int r2;
            int k1;
            int k2;
            int q1;
            //storage for number to check what numbers are left
            storage.Add(r1);
            //check if principle bishop and rook collide
            do
            {
                b1 = random.Next(0, 8);
            }
            while (storage.Contains(b1));
            storage.Add(b1);
            //rook 2
            do
            {
                r2 = random.Next(5, 8);
            }
            while (storage.Contains(r2));
            storage.Add(r2);
            //bishop 2
            int b2 = random.Next(0, 8);
            do
            {
                if ((b1 == 0 || b1 == 2 || b1 == 4 || b1 == 6) && storage.Contains(b2))
                {
                    b2 = random.Next(0, 8);
                }
                else if ((b1 == 1 || b1 == 3 || b1 == 5 || b1 == 7) && storage.Contains(b2))
                {
                    b2 = random.Next(0, 8);
                }
            }
            while (storage.Contains(b2));
            storage.Add(b2);
            ///now we can start to add the other pieces and populate the board
            ///for instance the queen, and knights dont have any hard stuck requirements so we are good to go
            do
            {
                k1 = random.Next(0, 8);
            }
            while (storage.Contains(k1));
            storage.Add(k1);
            do
            {
                k2 = random.Next(0, 8);
            }
            while (storage.Contains(k2));
            storage.Add(k2);
            do
            {
                q1 = random.Next(0, 8);
            }
            while (storage.Contains(q1));
            storage.Add(q1);

            MyPiece rook = new MyPiece
            {
                Number1 = r1,
                Number2 = r2
            };
            MyPiece Bishop = new MyPiece
            {
                Number1 = b1,
                Number2 = b2
            };
            MyPiece Knight = new MyPiece
            {
                Number1 = k1,
                Number2 = k2
            }; 
            MyPiece Queen = new MyPiece
            {
                Number1 = q1
            };
            Dictionary<string, MyPiece> numberSet = new()
            {
                { "Rook", rook },
                { "Bishop", Bishop },
                { "Knight", Knight },
                { "Queen", Queen }
            };

            return numberSet;
        }
        public ChessGame CreateGame3()
        {
            Dictionary<string, MyPiece> pairs = populateCondition();
            IEnumerable<PlacedPiece> makeBaseLine(int row, Color color,Dictionary<string,MyPiece> pairsContainer)
            {
                pairsContainer.TryGetValue("Knight",out MyPiece k);
                pairsContainer.TryGetValue("Rook",out MyPiece r);
                pairsContainer.TryGetValue("Queen",out MyPiece q);
                pairsContainer.TryGetValue("Bishop",out MyPiece b);

                yield return new PlacedPiece(new Position(row, r.Number1), new Rook(color));
                yield return new PlacedPiece(new Position(row, k.Number1), new Knight(color));
                yield return new PlacedPiece(new Position(row, b.Number1), new Bishop(color));
                yield return new PlacedPiece(new Position(row, q.Number1), new Queen(color));
                yield return new PlacedPiece(new Position(row, 4), new King(color));
                yield return new PlacedPiece(new Position(row, (int)b.Number2), new Bishop(color));
                yield return new PlacedPiece(new Position(row, (int)k.Number2), new Knight(color));
                yield return new PlacedPiece(new Position(row, (int)r.Number2), new Rook(color));
            }

            IEnumerable<PlacedPiece> makePawns(int row, Color color) =>
                Enumerable.Range(0, 8).Select(
                    i => new PlacedPiece(new Position(row, i), new Pawn(color))
                );

            IImmutableDictionary<Position, ChessPiece> makePieces(int pawnRow, int baseRow, Color color)
            {
                var pawns = makePawns(pawnRow, color);
                var baseLine = makeBaseLine(baseRow, color,pairs);
                var pieces = baseLine.Union(pawns);
                var empty = ImmutableSortedDictionary.Create<Position, ChessPiece>(PositionComparer.DefaultComparer);
                return pieces.Aggregate(empty, (s, p) => s.Add(p.Position, p.Piece));
            }

            var whitePlayer = new Player(Color.White);
            var whitePieces = makePieces(1, 0, Color.White);
            var blackPlayer = new Player(Color.Black);
            var blackPieces = makePieces(6, 7, Color.Black);
            var board = new Board(whitePieces.AddRange(blackPieces));

            return new ChessGame(board, whitePlayer, blackPlayer);
        }
        /// <summary>
        /// Gets the status of a chess game, according to the standard rulebook.
        /// </summary>
        /// <param name="game">The game state to be analyzed.</param>
        /// <returns>The current status of the game.</returns>
        public Status GetStatus(ChessGame game)
        {
            return this.endRule.GetStatus(game);
        }

        /// <summary>
        /// Gets all possible updates (i.e., future game states) for a chess piece on a specified position,
        /// according to the standard rulebook.
        /// </summary>
        /// <param name="game">The current game state.</param>
        /// <param name="position">The position to be analyzed.</param>
        /// <returns>A sequence of all possible updates for a chess piece on the specified position.</returns>
        public IEnumerable<Update> GetUpdates(ChessGame game, Position position)
        {
            var piece = game.Board.GetPiece(position, game.ActivePlayer.Color);
            var updates = piece.Map(
                p =>
                {
                    var moves = this.movementRule.GetCommands(game, p);
                    var turnEnds = moves.Select(c => new SequenceCommand(c, EndTurnCommand.Instance));
                    var records = turnEnds.Select
                    (
                        c => new SequenceCommand(c, new SetLastUpdateCommand(new Update(game, c)))
                    );
                    var futures = records.Select(c => c.Execute(game).Map(g => new Update(g, c)));
                    return futures.FilterMaybes().Where
                    (
                        e => !this.checkRule.Check(e.Game, e.Game.PassivePlayer)
                    );
                }
            );

            return updates.GetOrElse(Enumerable.Empty<Update>());
        }
    }
}