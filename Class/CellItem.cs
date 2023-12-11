namespace Oxcel.Class
{
    public class CellItem
    {
        public string Content { get; set; }
        public int[,] Position = new int[0, 0];

        public string Name { get; set; }

        public static int count = 0;

        public string CellExpression;
        public int CellValue;

        public CellItem(string content, string name)
        {
            Content = content;
            Name = name;
        }
    }

}