using System.Diagnostics;
using Foundation;
using Newtonsoft.Json;

namespace BackgroundPing.iOS
{
    public class PingOperation : NSOperation
    {
        private bool? result = null;
        private bool isExecuting = false;

        private double longitude = 0;
        private double latitude = 0;
        private NSUrlSession session = null;
        private NSUrlSessionDataTask dataTask = null;
        private string userId = null;

        private string baseUrl = "https://fd-falcon.azurewebsites.net";
        private string code = "OahMqpTnWi6VHfSDH9bU442hVozo4Qksjl36mPsrcVKodK5NaHfFVQ==";

        public PingOperation(double latitude, double longitude, string userId, NSUrlSession session)
        {
            this.session = session;
            this.latitude = latitude;
            this.longitude = longitude;
            this.userId = userId;
        }

        public override bool Asynchronous => true;

        public override bool IsExecuting => isExecuting;

        public override bool IsFinished => result.HasValue;

        public override void Cancel()
        {
            base.Cancel();
            dataTask?.Cancel();
        }

        public override void Start()
        {
            Debug.WriteLine("Ping operation started");
            WillChangeValue("IsExecuting");
            isExecuting = true;
            DidChangeValue("IsExecuting");

            if (IsCancelled)
            {
                Finish(false);
                return;
            }

            Debug.WriteLine("UrlSession request created");
            var request = CreatePingRequest();
            dataTask = session.CreateDataTask(request, new NSUrlSessionResponse((data, response, error) => {
                if (error != null)
                {
                    Debug.WriteLine($"Error: {error.DebugDescription}");
                    Finish(false);
                }

                Debug.WriteLine("UrlSession request completed");
                Finish(true);
            }));
        }

        public void Finish(bool result)
        {
            if (IsExecuting)
            {
                return;
            }

            WillChangeValue("IsExecuting");
            WillChangeValue("IsFinished");

            isExecuting = false;
            this.result = result;
            dataTask.Dispose();
            dataTask = null;

            DidChangeValue("IsExecuting");
            DidChangeValue("IsFinished");
        }

        private NSUrlRequest CreatePingRequest()
        {
            var url = GeneratePingUrl(userId);
            var req = new NSMutableUrlRequest(NSUrl.FromString(url));
            req.HttpMethod = "POST";
            req.CachePolicy = NSUrlRequestCachePolicy.ReloadIgnoringCacheData;
            var content = JsonConvert.SerializeObject(new
            {
                Long = longitude,
                Lat = latitude
            });
            req.Body = content;

            return req;
        }

        private string GeneratePingUrl(string userId)
        {
            return $"{baseUrl}/api/user/{userId}?code={code}";
        }
    }
}