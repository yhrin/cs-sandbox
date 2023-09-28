using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace cammy_gomoku
{

    //private record PlayerInfo(Color color);

    /// <summary>
    /// Page2.xaml の相互作用ロジック
    /// </summary>
    public partial class Page2 : Page
    {
        private int ColumnNum = 15;
        private int RowNum = 15;
        private GomokuManager gm = new GomokuManager();

        public Page2()
        {
            InitializeComponent();
            /*

            Viewer.HorizontalAlignment = HorizontalAlignment.Center;
            Viewer.VerticalAlignment = VerticalAlignment.Center;

            // scalaのforEachのように一行で書けないものか
            for (int i = 0; i < ColumnNum; i++)
            {
                var col = new ColumnDefinition();
                Viewer.ColumnDefinitions.Add(col);
            }

            for (int i = 0; i < RowNum; i++)
            {
                var col = new RowDefinition();
                Viewer.RowDefinitions.Add(col);
            }

            for (int i = 0; i < ColumnNum; i++)
            {
                for (int j = 0; j < RowNum; j++)
                {
                    var button = new Button();
                    // スコープ　クロージャ関係で
                    var x = i;
                    var y = j;
                    button.Width = 40;
                    button.Height = 40;
                    button.Margin = new Thickness(0);
                    button.Click += (s, e) =>
                    {
                        switch (gm.SelectPosition((x, y)))
                        {
                            case GomokuManager.ActionResult.Win:
                                HeaderLabel.Content = $"Win Player {gm.getPlayer((x, y))}";
                                button.Content = (gm.getPlayer((x, y)));
                                break;
                            case GomokuManager.ActionResult.Continue:
                                HeaderLabel.Content = $"Tern : Player {gm.getTernPlayer()}";
                                button.Content = (gm.getPlayer((x, y)));
                                break;
                            case GomokuManager.ActionResult.FailPosition:
                                //MessageBox.Show("Fail");
                                break;
                        }
                    };
                    Grid.SetColumn(button, i);
                    Grid.SetRow(button, j);
                    Viewer.Children.Add(button);
                }
            }
            */
            InitializeCanvas();
        }

        GeometryGroup[] geometryGroups;

        /* record にて設定したいが.Net Coreへの移行がわからないので現状直打ち
        PlayerInfo[] playerInfo = { new PlayerInfo(
        */

        private Image image;


        private void InitializeCanvas()
        {
            var playerColor = new List<Color> { Colors.White, Colors.Black };

            var drawGroup = new DrawingGroup();

            // draw board
            var boardGeometryGroup = new GeometryGroup();
            var boardGeometryDrawing = new GeometryDrawing();
            boardGeometryDrawing.Geometry = boardGeometryGroup;
            boardGeometryDrawing.Pen = new Pen(Brushes.Black, 2);
            drawGroup.Children.Add(boardGeometryDrawing);
            for(var i = 0; i <= 15; i++)
            {
                boardGeometryGroup.Children.Add(new LineGeometry(new Point(i * 40, 0), new Point(i * 40, 15 * 40)));
                boardGeometryGroup.Children.Add(new LineGeometry(new Point(0, i * 40), new Point(15 * 40, i * 40)));
            }

            geometryGroups = Enumerable.Range(1, 2).Select((v,i) => new GeometryGroup()).ToArray();
            foreach (var (geometryGroup, i) in geometryGroups.Select( (v,x) => (v,x)) ) {
                var geometryDrawing = new GeometryDrawing();
                geometryDrawing.Geometry = geometryGroup;
                geometryDrawing.Brush = new SolidColorBrush(playerColor[i]);
                geometryDrawing.Pen = new Pen(Brushes.Black, 5);
                drawGroup.Children.Add(geometryDrawing);
            }
            var drawingImage = new DrawingImage(drawGroup);
            image = new Image { Source = drawingImage, Stretch = Stretch.None };
            //this.Content = image;
            Viewer.Children.Add(image);
        }

        private int roundPosition(double x)
        {
            return (int)Math.Floor((x+20) / 40);
        }

        private void Viewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(image);
            var x = roundPosition(pos.X);
            var y = roundPosition(pos.Y);


            //geometryGroups[0].Children.Add(new EllipseGeometry(new Point(pos.X, pos.Y), 20, 20));
            switch (gm.SelectPosition((x, y)))
            {
                case GomokuManager.ActionResult.Win:
                    HeaderLabel.Content = $"Win Player {gm.getPlayer((x, y))}";
                    geometryGroups[gm.getPlayerIndex((x, y))].Children.Add(new EllipseGeometry(new Point(x * 40, y * 40), 15,15));
                    break;
                case GomokuManager.ActionResult.Continue:
                    HeaderLabel.Content = $"Tern : Player {gm.getTernPlayer()}";
                    geometryGroups[gm.getPlayerIndex((x, y))].Children.Add(new EllipseGeometry(new Point(x * 40, y * 40), 15,15));
                    break;
                case GomokuManager.ActionResult.FailPosition:
                    //MessageBox.Show("Fail");
                    break;
            }

        }
    }

    public class GomokuManager
    {

        private List<HashSet<(int, int)>> positions;
        private int ternPlayer;
        private bool finished = false;

        public GomokuManager()
        {
            positions = new List<HashSet<(int, int)>> { new HashSet<(int, int)>(), new HashSet<(int, int)>() };
            ternPlayer = 0;
        }

        public void initialize()
        {
            positions = new List<HashSet<(int, int)>> { new HashSet<(int, int)>(), new HashSet<(int, int)>() };
            ternPlayer = 0;
        }

        public string getPlayer((int, int) position)
        {
            //MessageBox.Show(String.Join(";", positions.Select(v => String.Join(",", v))));
            var p = positions.TakeWhile(x => !x.Contains(position)).Count();
            return p >= 2 ? "" : p.ToString();
        }
        public int getPlayerIndex((int, int) position)
        {
            return positions.TakeWhile(x => !x.Contains(position)).Count();
        }

        public string getTernPlayer()
        {
            return ternPlayer.ToString();
        }

        public enum ActionResult { Win, Continue, FailPosition };

        public ActionResult SelectPosition((int, int) position)
        {
            if (finished || positions.Any(p => p.Contains(position)))
            // Any or Exists?
            {
                return ActionResult.FailPosition;
            }
            positions[ternPlayer].Add(position);

            // winCheck(ternPlayer++%2, position) ? ActionResult.Win, ActionResult.Continue だと勝利時のternPlayerが変わっているのでダメ
            if (winCheck(ternPlayer, position))
            {
                finished = true;
                return ActionResult.Win;
            }
            else
            {
                ternPlayer = (ternPlayer + 1) % 2;
                return ActionResult.Continue;
            }

        }

        private bool winCheck(int ternPlayer, (int, int) position)
        {
            var vec = new List<(int, int)> { (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1), (1, -1) };
            var targetPositions = positions[ternPlayer];
            var chainLength = vec.Select((v, index) => new
            {
                index = index,
                count = Enumerable.Range(1, 4).TakeWhile((d, _) =>
                     targetPositions.Contains((position.Item1 + v.Item1 * d, position.Item2 + v.Item2 * d))).Count()
            });
            return chainLength.GroupBy(_ => _.index % 4, _ => _.count, (key, counts) => counts.Sum() >= 4).Any(_ => _);
        }
    }
}
