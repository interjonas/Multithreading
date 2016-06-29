using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        public static HotelInfo[] database = new HotelInfo[10];
        public static MultiBuffer mb = new MultiBuffer();
        public static Input input = new Input();
        static void Main(string[] args)
        {
            int hotelID = 1;
            for (int i = 0; i < 10; i++)
            {
                database[i] = new HotelInfo(("Hotel " + hotelID), (hotelID * 40), (hotelID * 60));
                hotelID++;
            }
            Thread[] hotelThreads = new Thread[10];
            Thread[] agentThreads = new Thread[10];
            for (int i = 0; i < 10; i++)
            {
                hotelThreads[i] = new Thread(HotelThread.runHotel);
                hotelThreads[i].Name = "Hotel " + i;
                hotelThreads[i].Start();
            }
            Console.WriteLine("NEW");
            Program.mb.getElement();
            Program.mb.getElement();
            Program.input.setInput();
            for (int i = 0; i < 10; i++)
            {
                agentThreads[i] = new Thread(TravelAgentThread.runTravelAgent);
                agentThreads[i].Name = "Agent " + i;
                agentThreads[i].Start();
            }
        }
    }
    public class Input
    {
        double price;
        int roomnum;
        public void setInput()
        {
            Console.Write("What Price:\t\t");
            string input1 = Console.ReadLine();
            price = Convert.ToDouble(input1);
            Console.Write("How Many Rooms:\t\t");
            string input2 = Console.ReadLine();
            roomnum = Int32.Parse(input2);
        }
        public double getPrice()
        {
            return price;
        }
        public int getRoomNum()
        {
            return roomnum;
        }
    }
    public class HotelThread
    {
        public static int x = 0;
        public static void runHotel()
        {
                addToBuffer();
        }
        public static void addToBuffer()
        {
            x = 10;
            Program.mb.addElement(Program.database[x]);
            Console.WriteLine(Program.database[x].name + "\t\t" + "Price: " + Program.database[x].price + "\t\t" + "Rooms Remaining: " + Program.database[x].roomnum);
            x++;
        }
    }
    public class HotelInfo
    {
        public string name;
        public double price;
        public int roomnum;
        public HotelInfo(string name, double price, int roomnum)
        {
           this.name = name;
           this.price = price;
           this.roomnum = roomnum;
        }
    }
    public class TravelAgentThread
    {
        static Reservation reserve;
        public static void runTravelAgent()
        {
            for (int i = 0; i < 10; i++)
            {
                reserve = new Reservation((Program.input.getPrice()), (Program.input.getRoomNum()));
                readFromBuffer();
                Thread.Sleep(500);
            }
        }
        public static void readFromBuffer()
        {
            HotelInfo x = Program.mb.getElement();
            Console.WriteLine(x.name);
            Console.WriteLine(x.price);
            Console.WriteLine(x.roomnum);
        }
    }
    public class Reservation
    {
        public double price;
        public int roomnum;
        public Reservation(double price, int roomnum)
        {
            this.price = price;
            this.roomnum = roomnum;
        }
    }

    class MultiBuffer
    {
        const int SIZE = 5;
        int head = 0, tail = 0, nElements = 0;
        HotelInfo[] buffer = new HotelInfo[SIZE];
        private Object BufferLock = new Object();

        Semaphore write = new Semaphore(5, 5);
        Semaphore read = new Semaphore(5, 5);

        public void addElement(HotelInfo n)
        {
            lock (BufferLock)
            {
                while (nElements == SIZE)
                {
                    Monitor.Wait(BufferLock);
                }
                buffer[tail] = n;
                tail = (tail + 1) % SIZE;
                nElements++;
                Monitor.Pulse(BufferLock);   
            }
        }

        public HotelInfo getElement()
        {
            lock (BufferLock)
            {
                HotelInfo element;
                while (nElements == 0)
                {
                    Monitor.Wait(BufferLock);
                }
                element = buffer[head];
                head = (head + 1) % SIZE;
                nElements--;
                Monitor.Pulse(BufferLock);
                return element;
            }
        }
    }
}