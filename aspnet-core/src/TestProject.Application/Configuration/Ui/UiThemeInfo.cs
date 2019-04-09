namespace TestProject.Configuration.Ui
{
    public class UiThemeInfo
    {
        public UiThemeInfo(string name, string cssClass)
        {
            Name = name;
            CssClass = cssClass;
        }

        public string Name { get; }
        public string CssClass { get; }
    }
}