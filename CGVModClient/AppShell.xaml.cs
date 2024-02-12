namespace CGVModClient
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            foreach(var item in Constants.PageRoutes) {
                Routing.RegisterRoute(item.Value, item.Key);
            }
        }
    }
}
