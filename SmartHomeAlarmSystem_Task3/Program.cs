using System.Net.Mail;

namespace SmartHomeAlarmSystem_Task3
{
    internal class Program
    {
        public static List<AlarmRecord> alarms = new List<AlarmRecord>();

        static void Main(string[] args)
        {
            var eventAggregator = new EventAggregator();

            DoorSensor doorSensor = new DoorSensor("Door");
            WindowSensor windowSensor = new WindowSensor("Window");
            MotionSensor motionSensor = new MotionSensor("Motion");

            eventAggregator.Subscribe<AlarmTriggeredEventArgs>(AlarmTriggeredMethod);

            doorSensor.TriggerAlarm(eventAggregator);
            windowSensor.TriggerAlarm(eventAggregator);
            motionSensor.TriggerAlarm(eventAggregator);

            Console.WriteLine("---- Alarm Records ----");
            foreach (var alarm in alarms)
            {
                Console.WriteLine(alarm.ToString());
            }

            eventAggregator.UnSubscribe<AlarmTriggeredEventArgs>(AlarmTriggeredMethod);
            Console.WriteLine("\nUnsubscribed from event.\n");

            doorSensor.TriggerAlarm(eventAggregator);

            SendEmailNotification("Alarm triggered!", "An alarm was triggered on your smart home system.");
        }

        public static void AlarmTriggeredMethod(AlarmTriggeredEventArgs args)
        {
            alarms.Add(new AlarmRecord(args.SensorType, args.TriggeredAt));

            Console.WriteLine($"{args.SensorType} sensor alarmed at {args.TriggeredAt}");

            SendEmailNotification($"{args.SensorType} Alarm", $"{args.SensorType} sensor triggered at {args.TriggeredAt}");
        }

        public static void SendEmailNotification(string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new System.Net.NetworkCredential("xetayi1988@gmail.com", "ypny pxqi jler uqnd"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("xetayi1988@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false,
                };
                mailMessage.To.Add("khatayirm@code.edu.az");

                smtpClient.Send(mailMessage);

                Console.WriteLine($"Email sent successfully: {subject}");
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        public class AlarmTriggeredEventArgs : EventArgs
        {
            public string SensorType { get; }
            public DateTime TriggeredAt { get; }

            public AlarmTriggeredEventArgs(string sensorType, DateTime triggeredAt)
            {
                SensorType = sensorType;
                TriggeredAt = triggeredAt;
            }
        }

        public class Sensor
        {
            public string SensorType { get; set; }

            public Sensor(string sensorType)
            {
                SensorType = sensorType;
            }

            public virtual void TriggerAlarm(EventAggregator eventAggregator)
            {
                eventAggregator.Publish(new AlarmTriggeredEventArgs(SensorType, DateTime.Now));
            }
        }

        public class DoorSensor : Sensor
        {
            public DoorSensor(string sensorType) : base(sensorType) { }
        }

        public class WindowSensor : Sensor
        {
            public WindowSensor(string sensorType) : base(sensorType) { }
        }

        public class MotionSensor : Sensor
        {
            public MotionSensor(string sensorType) : base(sensorType) { }
        }
        public class AlarmRecord
        {
            public string SensorType { get; set; }
            public DateTime DateTime { get; set; }

            public AlarmRecord(string sensorType, DateTime triggeredAt)
            {
                SensorType = sensorType;
                DateTime = triggeredAt;
            }

            public override string ToString()
            {
                return $"{SensorType} sensor alarmed at {DateTime}";
            }
        }

        public class EventAggregator
        {
            private readonly Dictionary<Type, List<Delegate>> _eventHandlers = new();

            public void Subscribe<TEvent>(Action<TEvent> eventHandler)
            {
                var eventType = typeof(TEvent);
                if (!_eventHandlers.ContainsKey(eventType))
                {
                    _eventHandlers[eventType] = new List<Delegate>();
                }
                _eventHandlers[eventType].Add(eventHandler);
            }

            public void UnSubscribe<TEvent>(Action<TEvent> eventHandler)
            {
                var eventType = typeof(TEvent);
                if (_eventHandlers.ContainsKey(eventType))
                {
                    _eventHandlers[eventType].Remove(eventHandler);
                }
            }

            public void Publish<TEvent>(TEvent eventData)
            {
                var eventType = typeof(TEvent);
                if (_eventHandlers.ContainsKey(eventType))
                {
                    foreach (var handler in _eventHandlers[eventType])
                    {
                        ((Action<TEvent>)handler)(eventData);
                    }
                }
            }
        }
    }
}
