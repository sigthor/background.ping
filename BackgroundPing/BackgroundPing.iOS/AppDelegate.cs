using System;
using System.Diagnostics;
using BackgroundTasks;
using Foundation;
using UIKit;

namespace BackgroundPing.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public readonly NSUrlSessionConfiguration urlSessionConfiguration = null;
        private readonly NSUrlSession urlSession = null;
        private readonly string RefreshTaskId = "com.fivedegrees.ios.background-test.refresh";

        public AppDelegate()
        {
            urlSessionConfiguration = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration("com.fivedegrees.ios.background-test");
            urlSession = NSUrlSession.FromConfiguration(urlSessionConfiguration);
        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            BGTaskScheduler.Shared.Register(RefreshTaskId, null, task => HandleAppRefresh(task as BGAppRefreshTask));

            return base.FinishedLaunching(app, options);
        }

        public override void DidEnterBackground(UIApplication uiApplication)
        {
            ScheduleAppRefresh();

            base.DidEnterBackground(uiApplication);
        }

        private void HandleAppRefresh(BGAppRefreshTask task)
        {
            Debug.WriteLine("HandleAppRefresh started");
            ScheduleAppRefresh();

            var queue = new NSOperationQueue();
            queue.MaxConcurrentOperationCount = 1;

            double latitude = 0;
            double longitude = 0;
            var n = new Random().Next(0, 2);
            if (n == 0)
            {
                // H12
                latitude = 64.096501;
                longitude = -21.890178;
            }
            else
            {
                // Arion
                latitude = 64.145549;
                longitude = -21.905449;
            }

            var pingOperation = new PingOperation(latitude, longitude, "205558e0-f226-446d-a769-aaceb3b2a880", urlSession);

            task.ExpirationHandler = () =>
            {
                queue.CancelAllOperations();
            };

            pingOperation.CompletionBlock = () =>
            {
                Debug.WriteLine("Ping operation completed");
                task.SetTaskCompleted(!pingOperation.IsCancelled);
            };

            queue.AddOperation(pingOperation);
        }

        private void ScheduleAppRefresh()
        {
            Debug.WriteLine("Scheduling ping task");
            var request = new BGAppRefreshTaskRequest(RefreshTaskId)
            {
                EarliestBeginDate = (NSDate)DateTime.Now.AddSeconds(30)
            };

            BGTaskScheduler.Shared.Submit(request, out NSError error);

            if (error != null)
            {
                throw new Exception(error.LocalizedDescription);
            }
        }
    }
}
