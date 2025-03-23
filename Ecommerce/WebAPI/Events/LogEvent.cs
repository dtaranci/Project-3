using System;

namespace WebAPI.Events
{
    public delegate void OnLogCreatedDelegate(object sender, OnLogCreatedArgs args);
    public class OnLogCreatedArgs
    {
        public DateTime DateTime { get; set; }
        public string Content { get; set; }
    }


    public delegate void OnLogErrorDelegate(object sender, OnLogErrorArgs args);
    public class OnLogErrorArgs
    {
        public DateTime DateTime { get; set; }
        public string Content { get; set; }
        public Exception Exception { get; set; }
    }

}
