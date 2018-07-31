using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqService.Attributes
{
    /// <summary>
    /// This enum is used for horizontal scaling.
    /// Application - messages will be delivered one by one to duplicate applications round-robing way.(NOTE: This selection can not be used with 'Route' variable)
    /// All - messages will be delivered to all intances of same application.
    /// </summary>
    public enum BroadcastTarget
    {
        Application, // same/duplicate applications will have same queue name - messages will be delivered one by one
        All // same/duplication applicationss will have different queue name - messages will be delivered to all
    }
    public class BroadcastMessageAttribute : MessageAttribute
    {
        public override bool IsBroadcast { get => true; }

        public BroadcastTarget Target { get; set; } = BroadcastTarget.All;
    }
}
