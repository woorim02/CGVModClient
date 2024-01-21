﻿namespace CGVModClient
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("GiveawayEventDetailPage", typeof(GiveawayEventDetailPage));
            Routing.RegisterRoute("GiveawayEventListPage", typeof(GiveawayEventListPage));
            Routing.RegisterRoute("BookingOpenNotificationSettingPage", typeof(BookingOpenNotificationSettingPage));
            Routing.RegisterRoute("BookingOpenNotificationAddPage", typeof(BookingOpenNotificationAddPage));
            Routing.RegisterRoute("YongsanIMAXOpenNotificationPage", typeof(YongsanIMAXOpenNotificationPage));
        }
    }
}
