using static SmartHomeAlarmSystem_Task3.Sensor;

namespace SmartHomeAlarmSystem_Task3
{
    internal class Program
    {
        public static List<AlarmRecord> alarms = new List<AlarmRecord>();

        static void Main(string[] args)
        {
            DoorSensor doorSensor = new DoorSensor("Door");
            WindowSensor windowSensor = new WindowSensor("Window");
            MotionSensor motionSensor = new MotionSensor("Motion");
            doorSensor.AlarmTriggered += AlarmTriggeredMethod;
            windowSensor.AlarmTriggered += AlarmTriggeredMethod;
            motionSensor.AlarmTriggered += AlarmTriggeredMethod;
            doorSensor.TriggerAlarm();
            windowSensor.TriggerAlarm();
            motionSensor.TriggerAlarm();
            Console.WriteLine("----Alarm Records----");
            foreach (var alarm in alarms)
            {
                Console.WriteLine(alarm.ToString());
            }
        }
        public static void AlarmTriggeredMethod(string sensorType, DateTime triggeredAt)
        {
            if(sensorType== "Door"|| sensorType == "Window")
            Console.WriteLine($"{sensorType} is opened at {triggeredAt}");
            if (sensorType == "Motion")
            Console.WriteLine($"{sensorType} is detected at {triggeredAt}");
            alarms.Add(new AlarmRecord(sensorType, triggeredAt));
            

        }
    }

    public class Sensor
    {
        public string SensorType { get; set; }

        public Sensor(string sensorType)
        {
            SensorType = sensorType;
        }

        public delegate void AlarmTriggeredHandler(string sensorType, DateTime triggeredAt);

        public event AlarmTriggeredHandler? AlarmTriggered;

        public virtual void TriggerAlarm()
        {
            AlarmTriggered?.Invoke(SensorType, DateTime.Now);
        }

    }

    public class DoorSensor : Sensor
    {
        public DoorSensor(string sensorType) : base(sensorType)
        {
        }
       
    }
    public class WindowSensor : Sensor
    {
        public WindowSensor(string sensorType) : base(sensorType)
        {

        }
        
    }
    public class MotionSensor : Sensor
    {
        public MotionSensor(string sensorType) : base(sensorType)
        {
        }
        
    }

    public class AlarmRecord
    {
        public AlarmRecord(string sensorType, DateTime triggeredAt)
        {
            SensorType = sensorType;
            DateTime = triggeredAt;
        }

        public string SensorType { get; set; }
        public DateTime DateTime { get; set; }
        public override string ToString()
        {
            return $"{SensorType} sensor alarmed at {DateTime}";
        }
    }

}
