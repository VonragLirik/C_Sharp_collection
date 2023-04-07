using System;
using System.Text;

// Описание и реализация интерфейсов

//Интерфейсы:
//  IUsbBus(шина USB)
//	ISata(шина SATA)
//	INetwork(сеть)
//	IInnerBus(внутренняя шина компьютера)
//Классы:
//  MotherBoard(материнская плата с процессором)
//	RamMemory(оперативная память)
//	HardDisk(жесткий диск)
//	Printer(принтер)
//	Scanner(сканер изображений)
//	NetworkCard(сетевая карта)
//	Keyboard(клавиатура)


namespace Lab3 {
    class Program {
        static void Main() {
            MotherBoard motherBoard = new MotherBoard();
            RamMemory ramMemory = new RamMemory();
            HardDisk hardDisk = new HardDisk();
            HardDisk hardDisk2 = new HardDisk();
            Printer printer = new Printer();
            Scanner scanner = new Scanner();
            Scanner scannerForTest = new Scanner();
            NetworkCard networkCard = new NetworkCard();
            Keyboard keyboard = new Keyboard();

            // Connect devices
            motherBoard.ConnectSata(hardDisk);
            // Can be only one sata port
            motherBoard.ConnectSata(hardDisk2);
            motherBoard.ConnectNetwork(networkCard);
            motherBoard.ConnectInnerBus(ramMemory);

            // Can be 3 USB devices
            motherBoard.ConnectUSb(keyboard);
            motherBoard.ConnectUSb(printer);
            motherBoard.ConnectUSb(scanner);
            motherBoard.ConnectUSb(scannerForTest);

            keyboard.SendBufferData();
            motherBoard.getLastCommandInBuffer();
            scanner.SendBufferData();
            motherBoard.getLastCommandInBuffer();
            scannerForTest.SendBufferData();
            motherBoard.getLastCommandInBuffer();
        }
    }

    public delegate void Buffer(string newBufferValue);

    ///////////////////////////////// Interfaces
    public interface IUsbBus {
        event Buffer OnChangeBuffer;

        int SpeedInfo { get; }

        void SendBufferData();
    }

    public interface ISata {
        event Buffer OnChangeBuffer;
        int SpeedInfo { get; }
        void SendBufferData();
    }
    public interface INetwork {
        event Buffer OnChangeBuffer;
        int SpeedInfo { get; }

        void SendBufferData();
    }

    public interface IInnerBus {
        event Buffer OnChangeBuffer;
        int SpeedInfo { get; }

        void SendBufferData();
    }

    ///////////////////////////////// Classes
    public class MotherBoard {
        public byte[] buffer;

        IInnerBus innerBusPort;

        ISata sataPort;

        INetwork networkPort;
        readonly IUsbBus[] UsbPorts = new IUsbBus[3];

        public void getLastCommandInBuffer() {
            var utf8 = new UTF8Encoding();
            try {
                Console.WriteLine(utf8.GetString(buffer));
            } catch (System.ArgumentNullException) {
                Console.WriteLine("Device is not connected!");
            }
        }

        private void SetBufferHelper(string command) {
            var utf8 = new UTF8Encoding();
            buffer = utf8.GetBytes(command);
        }

        public void ConnectUSb(IUsbBus usbBus) {
            for (int i = 0; i < UsbPorts.Length; i++) {
                if (UsbPorts[i] == null) {
                    UsbPorts[i] = usbBus;
                    UsbPorts[i].OnChangeBuffer += SetBufferHelper;
                    break;
                }

                if (i == UsbPorts.Length - 1) {
                    Console.WriteLine("All USB ports is already exist!");
                }
            }
        }

        public void ConnectNetwork(INetwork network) {
            if (networkPort == null) {
                networkPort = network;
                networkPort.OnChangeBuffer += SetBufferHelper;
            } else {
                Console.WriteLine("Network is already exist!");
            }
        }

        public void ConnectSata(ISata sata) {
            if (sataPort == null) {
                sataPort = sata;
                sataPort.OnChangeBuffer += SetBufferHelper;
            } else {
                Console.WriteLine("Sata is already exist");
            }
        }

        public void ConnectInnerBus(IInnerBus innerBus) {
            if (innerBusPort == null) {
                innerBusPort = innerBus;
                innerBusPort.OnChangeBuffer += SetBufferHelper;
            } else {
                Console.WriteLine("InnerBus is already exist!");
            }
        }
    }

    public class RamMemory : IInnerBus {
        const int maxTransportSpeed = 400;
        public int SpeedInfo => maxTransportSpeed;

        public event Buffer OnChangeBuffer;

        public void SendBufferData() {
            Console.WriteLine("Input in buffer from RAM:");
            string inputData = Console.ReadLine();
            OnChangeBuffer?.Invoke(inputData);
        }
    }

    public class HardDisk : ISata {
        const int maxTransportSpeed = 150;
        public int SpeedInfo => maxTransportSpeed;

        public event Buffer OnChangeBuffer;

        public void SendBufferData() {
            string inputData = Console.ReadLine();
            OnChangeBuffer?.Invoke(inputData);
        }
    }

    public class Printer : IUsbBus {
        const int maxTransportSpeed = 200;
        public int SpeedInfo => maxTransportSpeed;

        public event Buffer OnChangeBuffer;

        public void SendBufferData() {
            Console.WriteLine("Input in buffer from printer:");
            string inputData = Console.ReadLine();
            OnChangeBuffer?.Invoke(inputData);
        }
    }

    public class Scanner : IUsbBus {
        const int maxTransportSpeed = 200;
        public int SpeedInfo => maxTransportSpeed;

        public event Buffer OnChangeBuffer;

        public void SendBufferData() {
            Console.WriteLine("Input in buffer from scanner:");
            string inputData = Console.ReadLine();
            OnChangeBuffer?.Invoke(inputData);
        }
    }

    public class Keyboard : IUsbBus {
        const int maxTransportSpeed = 200;
        public int SpeedInfo => maxTransportSpeed;

        public event Buffer OnChangeBuffer;

        public void SendBufferData() {
            Console.WriteLine("Input in buffer from keyboard:");
            string inputData = Console.ReadLine();
            OnChangeBuffer?.Invoke(inputData);
        }
    }

    public class NetworkCard : INetwork {
        const int maxTransportSpeed = 500;
        public int SpeedInfo => maxTransportSpeed;

        public event Buffer OnChangeBuffer;

        public void SendBufferData() {
            Console.WriteLine("Input in buffer from network card:");
            string inputData = Console.ReadLine();
            OnChangeBuffer?.Invoke(inputData);
        }
    }
}
