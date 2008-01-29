using System;
using System.Collections.Generic;
using System.Drawing;

namespace Tivo.Hme.Samples
{
    class TicTacToe : HmeApplicationHandler
    {
        Application _app;
        View _piecesView;
        TextStyle _tokenStyle = new TextStyle("default", FontStyle.Bold, 72);
        TextView[,] _pieces = new TextView[3, 3];
        int _gridX;
        int _gridY;

        int movesElapsed = 0;

        static TicTacToe()
        {
            Application.Images.Add("background", Properties.Resources.bg, Properties.Resources.bg.RawFormat);
            Application.Images.Add("grid", Properties.Resources.grid, Properties.Resources.grid.RawFormat);
        }

        public override void OnApplicationStart(HmeApplicationStartArgs e)
        {
            _app = e.Application;

            _app.Root = new ImageView(_app.GetImageResource("background"));

            _piecesView = new View();
            _piecesView.Bounds = _app.Root.Bounds;
            _app.Root.Children.Add(_piecesView);

            _gridX = (_app.Root.Bounds.Width - 300) / 2;
            _gridY = 130;
            ImageView grid = new ImageView(_app.GetImageResource("grid"));
            grid.Bounds = new Rectangle(_gridX - 5, _gridY - 5, 310, 310);
            _app.Root.Children.Add(grid);

            _app.CreateTextStyle(_tokenStyle);

            _app.KeyPress += new EventHandler<KeyEventArgs>(_app_KeyPress);
        }

        public override void OnApplicationEnd()
        {
        }


        void _app_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= KeyCode.Num1 && e.KeyCode <= KeyCode.Num9)
            {
                int movePosition = (int)e.KeyCode - (int)KeyCode.Num1;
                // convert pos to x/y and make a move
                MakeMove(movePosition % 3, movePosition / 3);
                e.Handled = true;
            }
            else if (e.KeyCode == KeyCode.Left)
            {
                e.Handled = true;
                _app.Close();
            }
            else
            {
                _app.GetSound("bonk").Play();
            }
        }

        private void MakeMove(int x, int y)
        {
            // is this a valid move?
            if (_pieces[x, y] != null)
            {
                _app.GetSound("bonk").Play();
                return;
            }

            int player = (movesElapsed++) % 2;
            // create the piece
            _pieces[x, y] = CreatePiece(player, x, y);

            bool victory = IsVictory();
            bool draw = !victory && movesElapsed == 9;
            if (victory || draw)
            {
                _app.GetSound(victory ? "tivo" : "thumbsdown").Play();

                System.Threading.Thread.Sleep(2000);

                TimeSpan sec1 = TimeSpan.FromSeconds(1);
                for (int i = 0; i < 3; ++i)
                {
                    for (int j = 0; j < 3; ++j)
                    {
                        View piece = _pieces[i, j];
                        if (piece != null)
                        {
                            Animation animation = new Animation();
                            if (victory)
                                animation.AddMove(piece.Location + new Size((i - 1) * 400, (j - 1) * 300), 0, sec1);
                            animation.AddFade(1, 0, sec1);
                            piece.Animate(animation);
                            _piecesView.Children.Remove(piece, sec1);
                            _pieces[i, j] = null;
                        }
                    }
                }

                movesElapsed = 0;
            }
        }

        private bool IsVictory()
        {
            for (int i = 0; i < 3; ++i)
            {
                if (IsVictoryRun(0, i, 1, 0) || IsVictoryRun(i, 0, 0, 1))
                    return true;
            }
            return IsVictoryRun(0, 0, 1, 1) || IsVictoryRun(0, 2, 1, -1);
        }

        private bool IsVictoryRun(int ox, int oy, int dx, int dy)
        {
            int x = ox;
            int y = oy;
            for (int i = 0; i < 3; ++i)
            {
                if (_pieces[x, y] == null)
                    return false;
                if (i > 0 && _pieces[x, y].Text != _pieces[x - dx, y - dy].Text)
                    return false;
                x += dx;
                y += dy;
            }

            // win - highlight pieces
            x = ox;
            y = oy;
            for (int i = 0; i < 3; ++i)
            {
                _pieces[x, y].Update(_pieces[x, y].Text, _tokenStyle, Color.FromArgb(0xff, 0xa0, 0x00));
                x += dx;
                y += dy;
            }

            return true;
        }

        private TextView CreatePiece(int player, int x, int y)
        {
            string token = (player == 0) ? "X" : "O";
            TextView piece = new TextView(token, _tokenStyle, Color.White);
            piece.Bounds = new Rectangle(_gridX + x * 100, _gridY + y * 100, 100, 100);
            _piecesView.Children.Add(piece);
            return piece;
        }
    }
    //class TicTacToe : IDisposable
    //{
    //    HmeServer _server = new HmeServer("Tic Tac Toe", new Uri("http://localhost:7688/TicTacToe"));

    //    public TicTacToe()
    //    {
    //        Application.Images.Add("background", Properties.Resources.bg, Properties.Resources.bg.RawFormat);
    //        Application.Images.Add("grid", Properties.Resources.grid, Properties.Resources.grid.RawFormat);
    //        _server.ApplicationConnected += new EventHandler<HmeApplicationConnectedEventArgs>(_server_ApplicationConnected);
    //        _server.Start();
    //    }

    //    #region IDisposable Members

    //    public void Dispose()
    //    {
    //        _server.Stop();
    //    }

    //    #endregion

    //    void _server_ApplicationConnected(object sender, HmeApplicationConnectedEventArgs e)
    //    {
    //        new TicTacToeClientManager(e.Application);
    //    }

    //    private class TicTacToeClientManager
    //    {
    //        Application _app;
    //        View _piecesView;
    //        TextStyle _tokenStyle = new TextStyle("default", FontStyle.Bold, 72);
    //        TextView[,] _pieces = new TextView[3, 3];
    //        int _gridX;
    //        int _gridY;

    //        int movesElapsed = 0;

    //        public TicTacToeClientManager(Application app)
    //        {
    //            _app = app;

    //            _app.Root = new ImageView(_app.GetImageResource("background"));

    //            _piecesView = new View();
    //            _piecesView.Bounds = _app.Root.Bounds;
    //            _app.Root.Children.Add(_piecesView);

    //            _gridX = (_app.Root.Bounds.Width - 300) / 2;
    //            _gridY = 130;
    //            ImageView grid = new ImageView(_app.GetImageResource("grid"));
    //            grid.Bounds = new Rectangle(_gridX - 5, _gridY - 5, 310, 310);
    //            _app.Root.Children.Add(grid);

    //            _app.CreateTextStyle(_tokenStyle);

    //            _app.KeyPress += new EventHandler<KeyEventArgs>(_app_KeyPress);
    //        }
    
    //        void _app_KeyPress(object sender, KeyEventArgs e)
    //        {
    //            if (e.KeyCode >= KeyCode.Num1 && e.KeyCode <= KeyCode.Num9)
    //            {
    //                int movePosition = (int)e.KeyCode - (int)KeyCode.Num1;
    //                // convert pos to x/y and make a move
    //                MakeMove(movePosition % 3, movePosition / 3);
    //                e.Handled = true;
    //            }
    //            else if (e.KeyCode == KeyCode.Left)
    //            {
    //                e.Handled = true;
    //                _app.Dispose();
    //            }
    //            else
    //            {
    //                _app.GetSound("bonk").Play();
    //            }
    //        }

    //        private void MakeMove(int x, int y)
    //        {
    //            // is this a valid move?
    //            if (_pieces[x, y] != null)
    //            {
    //                _app.GetSound("bonk").Play();
    //                return;
    //            }

    //            int player = (movesElapsed++) % 2;
    //            // create the piece
    //            _pieces[x, y] = CreatePiece(player, x, y);

    //            bool victory = IsVictory();
    //            bool draw = !victory && movesElapsed == 9;
    //            if (victory || draw)
    //            {
    //                _app.GetSound(victory ? "tivo" : "thumbsdown").Play();

    //                System.Threading.Thread.Sleep(2000);

    //                TimeSpan sec1 = TimeSpan.FromSeconds(1);
    //                for (int i = 0; i < 3; ++i)
    //                {
    //                    for (int j = 0; j < 3; ++j)
    //                    {
    //                        View piece = _pieces[i, j];
    //                        if (piece != null)
    //                        {
    //                            Animation animation = new Animation();
    //                            if (victory)
    //                                animation.AddMove(piece.Location + new Size((i - 1) * 400, (j - 1) * 300), 0, sec1);
    //                            animation.AddFade(1, 0, sec1);
    //                            piece.Animate(animation);
    //                            _piecesView.Children.Remove(piece, sec1);
    //                            _pieces[i, j] = null;
    //                        }
    //                    }
    //                }

    //                movesElapsed = 0;
    //            }
    //        }

    //        private bool IsVictory()
    //        {
    //            for (int i = 0; i < 3; ++i)
    //            {
    //                if (IsVictoryRun(0, i, 1, 0) || IsVictoryRun(i, 0, 0, 1))
    //                    return true;
    //            }
    //            return IsVictoryRun(0, 0, 1, 1) || IsVictoryRun(0, 2, 1, -1);
    //        }

    //        private bool IsVictoryRun(int ox, int oy, int dx, int dy)
    //        {
    //            int x = ox;
    //            int y = oy;
    //            for (int i = 0; i < 3; ++i)
    //            {
    //                if (_pieces[x, y] == null)
    //                    return false;
    //                if (i > 0 && _pieces[x, y].Text != _pieces[x - dx, y - dy].Text)
    //                    return false;
    //                x += dx;
    //                y += dy;
    //            }

    //            // win - highlight pieces
    //            x = ox;
    //            y = oy;
    //            for (int i = 0; i < 3; ++i)
    //            {
    //                _pieces[x, y].Update(_pieces[x, y].Text, _tokenStyle, Color.FromArgb(0xff, 0xa0, 0x00));
    //                x += dx;
    //                y += dy;
    //            }

    //            return true;
    //        }

    //        private TextView CreatePiece(int player, int x, int y)
    //        {
    //            string token = (player == 0) ? "X" : "O";
    //            TextView piece = new TextView(token, _tokenStyle, Color.White);
    //            piece.Bounds = new Rectangle(_gridX + x * 100, _gridY + y * 100, 100, 100);
    //            _piecesView.Children.Add(piece);
    //            return piece;
    //        }
    //    }
    //}
}
